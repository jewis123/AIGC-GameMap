using System.Text.Json;

namespace MapGenerator.Request.ComfyUI
{
    public class InpaintProcessor
    {
        private readonly ComfyUIClient _comfyClient;
        private string _workflowPath;

        public InpaintProcessor(ref ComfyUIClient comfyClient)
        {
            _comfyClient = comfyClient;
            _workflowPath = Path.Combine(AppSettings.AssetsDirectory, "workflow", "2dmap_inpaint.json");
        }

        /// <summary>
        /// 执行局部重绘处理
        /// </summary>
        /// <param name="prompt">用户输入的提示词</param>
        /// <param name="imagePath">原始图像路径</param>
        /// <param name="maskPath">遮罩图像路径</param>
        /// <returns>生成的图像路径</returns>
        public async Task<string> ProcessInpainting(string prompt, string imagePath, string maskPath)
        {
            try
            {
                Console.WriteLine($"开始局部重绘处理：提示词={prompt}, 图像={imagePath}, 遮罩={maskPath}");

                // 确保原始图像存在
                if (!File.Exists(imagePath))
                {
                    Console.WriteLine($"错误：原始图像不存在：{imagePath}");
                    return string.Empty;
                }

                // 确保遮罩图像存在
                if (!File.Exists(maskPath))
                {
                    Console.WriteLine($"错误：遮罩图像不存在：{maskPath}");
                    return string.Empty;
                }

                // 取消之前的任务
                await _comfyClient.CancelCurrentExecution();

                // 读取工作流模板
                string workflowJson = File.ReadAllText(_workflowPath);
                var workflow = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(workflowJson);

                if (workflow == null)
                {
                    throw new Exception("无法解析工作流JSON");
                }

                // 上传原始图像到ComfyUI
                string uploadedImageName = await _comfyClient.UploadImage(imagePath);
                if (string.IsNullOrEmpty(uploadedImageName))
                {
                    throw new Exception("上传原始图像失败");
                }
                Console.WriteLine($"原始图像上传成功：{uploadedImageName}");

                // 上传遮罩图像到ComfyUI
                string uploadedMaskName = await _comfyClient.UploadImage(maskPath);
                if (string.IsNullOrEmpty(uploadedMaskName))
                {
                    throw new Exception("上传遮罩图像失败");
                }
                Console.WriteLine($"遮罩图像上传成功：{uploadedMaskName}");

                // 创建新的工作流字典，可以安全修改
                var modifiedWorkflow = new Dictionary<string, object>();
                foreach (var node in workflow)
                {
                    // 复制所有节点数据
                    modifiedWorkflow[node.Key] = JsonSerializer.Deserialize<object>(node.Value.GetRawText());
                }

                // 修改节点21 - 原始图像
                if (modifiedWorkflow.TryGetValue("21", out var node21Obj))
                {
                    var node21 = JsonSerializer.Deserialize<Dictionary<string, object>>(
                        ((JsonElement)workflow["21"]).GetRawText());
                    
                    if (node21 != null)
                    {
                        // 替换输入参数
                        node21["inputs"] = new Dictionary<string, object>
                        {
                            { "image", uploadedImageName }
                        };
                        modifiedWorkflow["21"] = node21;
                    }
                }

                // 修改节点15 - 遮罩图像
                if (modifiedWorkflow.TryGetValue("15", out var node15Obj))
                {
                    var node15 = JsonSerializer.Deserialize<Dictionary<string, object>>(
                        ((JsonElement)workflow["15"]).GetRawText());
                    
                    if (node15 != null)
                    {
                        // 替换输入参数
                        node15["inputs"] = new Dictionary<string, object>
                        {
                            { "image", uploadedMaskName }
                        };
                        modifiedWorkflow["15"] = node15;
                    }
                }

                // 修改节点22 - 提示词
                if (modifiedWorkflow.TryGetValue("22", out var _))
                {
                    var node22 = JsonSerializer.Deserialize<Dictionary<string, object>>(
                        ((JsonElement)workflow["22"]).GetRawText());
                    
                    if (node22 != null && node22.ContainsKey("inputs"))
                    {
                        var inputs = JsonSerializer.Deserialize<Dictionary<string, object>>(
                            ((JsonElement)node22["inputs"]).GetRawText());
                            
                        if (inputs != null)
                        {
                            inputs["text"] = prompt;
                            node22["inputs"] = inputs;
                            modifiedWorkflow["22"] = node22;
                        }
                    }
                }

                // 创建完整的请求对象，包裹在prompt键下
                var requestData = new Dictionary<string, object>
                {
                    ["prompt"] = modifiedWorkflow
                };

                // 执行工作流
                string? promptId = await _comfyClient.ExecuteWorkflow(requestData);
                if (string.IsNullOrEmpty(promptId))
                {
                    throw new Exception("执行工作流失败");
                }
                Console.WriteLine($"工作流执行成功，promptId：{promptId}");

                // 轮询等待结果，获取节点18的输出
                string resultPath = await _comfyClient.PollForResult(promptId, "18");
                if (string.IsNullOrEmpty(resultPath))
                {
                    throw new Exception("获取结果图像失败");
                }
                Console.WriteLine($"重绘完成，结果路径：{resultPath}");

                return resultPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"局部重绘处理失败：{ex.Message}");
                return string.Empty;
            }
        }
    }
}