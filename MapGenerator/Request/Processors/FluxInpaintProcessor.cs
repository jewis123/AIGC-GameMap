using System.Text.Json;

namespace MapGenerator.Request.ComfyUI
{
    public class FluxInpaintProcessor : BaseProcessor
    {
        private string _workflowPath;

        public FluxInpaintProcessor(ref ComfyUIClient comfyClient) : base(ref comfyClient)
        {
            _workflowPath = Path.Combine(AppSettings.AssetsDirectory, "workflow", "2dmap_inpaint_flux_ip.json");
        }

        /// <summary>
        /// 执行局部重绘处理
        /// </summary>
        /// <param name="prompt">用户输入的提示词</param>
        /// <param name="imagePath">原始图像路径</param>
        /// <param name="maskPath">遮罩图像路径</param>
        /// <returns>生成的图像路径</returns>
        public async Task<string?> Process(string prompt, string imagePath, string maskPath, IProgress<int> progress)
        {
            Console.WriteLine($"开始局部重绘处理：提示词={prompt}, 图像={imagePath}, 遮罩={maskPath}");

            // 确保原始图像存在
            if (!File.Exists(imagePath))
            {
                MessageBox.Show($"错误：原始图像不存在：{imagePath}");
                return null;
            }

            // 确保遮罩图像存在
            if (!File.Exists(maskPath))
            {
                MessageBox.Show($"错误：遮罩图像不存在：{maskPath}");
                return null;
            }

            // 取消之前的任务
            await _comfyClient.CancelCurrentExecution();

            // 上传原始图像到ComfyUI
            progress.Report(10);
            string uploadedImageName = await _comfyClient.UploadImage(imagePath);
            if (string.IsNullOrEmpty(uploadedImageName))
            {
                MessageBox.Show("上传原始图像失败");
                return null;
            }
            Console.WriteLine($"原始图像上传成功：{uploadedImageName}");

            // 上传遮罩图像到ComfyUI
            progress.Report(20);
            string uploadedMaskName = await _comfyClient.UploadImage(maskPath);
            if (string.IsNullOrEmpty(uploadedMaskName))
            {
                MessageBox.Show("上传遮罩图像失败");
                return null;
            }
            Console.WriteLine($"遮罩图像上传成功：{uploadedMaskName}");

            // 读取工作流模板
            progress.Report(30);
            string workflowJson = File.ReadAllText(_workflowPath);
            var workflow = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(workflowJson);

            if (workflow == null)
            {
                MessageBox.Show("无法解析工作流JSON");
                return null;
            }

            // 创建新的工作流字典，可以安全修改
            var modifiedWorkflow = new Dictionary<string, object>();
            foreach (var node in workflow)
            {
                // 复制所有节点数据
                modifiedWorkflow[node.Key] = JsonSerializer.Deserialize<object>(node.Value.GetRawText());
            }

            // 修改节点 - 原始图像
            if (modifiedWorkflow.TryGetValue("17", out var node21Obj))
            {
                var node17 = JsonSerializer.Deserialize<Dictionary<string, object>>(
                    ((JsonElement)workflow["17"]).GetRawText());

                if (node17 != null)
                {
                    // 替换输入参数
                    node17["inputs"] = new Dictionary<string, object>
                        {
                            { "image", uploadedImageName }
                        };
                    modifiedWorkflow["17"] = node17;
                }
            }

            // 修改节点 - 遮罩图像
            if (modifiedWorkflow.TryGetValue("63", out var node15Obj))
            {
                var node63 = JsonSerializer.Deserialize<Dictionary<string, object>>(
                    ((JsonElement)workflow["63"]).GetRawText());

                if (node63 != null)
                {
                    // 替换输入参数
                    node63["inputs"] = new Dictionary<string, object>
                        {
                            { "image", uploadedMaskName }
                        };
                    modifiedWorkflow["63"] = node63;
                }
            }

            // 修改节点40 - 提示词
            if (modifiedWorkflow.TryGetValue("61", out var _))
            {
                var node61 = JsonSerializer.Deserialize<Dictionary<string, object>>(
                    ((JsonElement)workflow["61"]).GetRawText());

                if (node61 != null && node61.ContainsKey("inputs"))
                {
                    var inputs = JsonSerializer.Deserialize<Dictionary<string, object>>(
                        ((JsonElement)node61["inputs"]).GetRawText());

                    if (inputs != null)
                    {
                        inputs["Text_A"] = prompt;
                        node61["inputs"] = inputs;
                        modifiedWorkflow["61"] = node61;
                    }
                }
            }

            // 创建完整的请求对象，包裹在prompt键下
            var requestData = new Dictionary<string, object>
            {
                ["prompt"] = modifiedWorkflow
            };

            // 执行工作流
            progress.Report(40);
            string? promptId = await _comfyClient.ExecuteWorkflow(requestData);
            if (string.IsNullOrEmpty(promptId))
            {
                MessageBox.Show("执行工作流失败");
                return null;
            }
            Console.WriteLine($"工作流执行成功，promptId：{promptId}");

            LastPromptId = promptId;
            LastNodeId = "9";

            // 轮询等待结果
            string resultPath = await _comfyClient.PollForResult(LastPromptId, LastNodeId);
            if (string.IsNullOrEmpty(resultPath))
            {
                MessageBox.Show("获取结果图像失败");
                return null;
            }

            progress.Report(100);
            Console.WriteLine($"重绘完成，结果路径：{resultPath}");

            return resultPath;
        }
    }
}