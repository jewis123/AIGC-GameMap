using System.Text.Json;

namespace MapGenerator.Request.ComfyUI
{
    public class DrawToImgIPSegProcessor
    {
        private readonly ComfyUIClient _comfyClient;
        private readonly string _workflowPath;

        public DrawToImgIPSegProcessor(ref ComfyUIClient comfyClient)
        {
            _comfyClient = comfyClient;
            // 将工作流 JSON 文件路径设置为工作流 JSON 文件位置
            _workflowPath = Path.Combine(AppSettings.AssetsDirectory, "workflow", "2dmap_seg_ip.json");
        }

        /// <summary>
        /// 处理绘图+参考到图像的转换，将其发送到ComfyUI进行处理
        /// </summary>
        /// <param name="prompt">用于生成的提示词</param>
        /// <param name="drawImagePath">用户绘制的图片路径</param>
        /// <param name="refImgPath">用户参考的图片路径</param>
        /// <param name="pixcels">用户设置的生成尺寸</param>
        /// <returns>生成的图片路径，如果处理失败则返回null</returns>
        public async Task<string?> ProcessDrawToImage(string prompt, string drawImagePath, string refImgPath, int[] pixcels)
        {
            try
            {
                // 先取消当前正在执行的任务，避免排队
                await _comfyClient.CancelCurrentExecution();

                // 上传绘制的图片
                string uploadedImageName = string.Empty;
                if (!string.IsNullOrEmpty(drawImagePath) && File.Exists(drawImagePath))
                {
                    uploadedImageName = await _comfyClient.UploadImage(drawImagePath);
                    if (string.IsNullOrEmpty(uploadedImageName))
                    {
                        MessageBox.Show("手绘图片上传失败");
                        return null;
                    }
                }

                // 上传绘制的图片
                string uploadedRefImageName = string.Empty;
                if (!string.IsNullOrEmpty(refImgPath) && File.Exists(refImgPath))
                {
                    uploadedRefImageName = await _comfyClient.UploadImage(refImgPath);
                    if (string.IsNullOrEmpty(uploadedRefImageName))
                    {
                        MessageBox.Show("参考图片上传失败");
                        return null;
                    }
                }

                // 准备工作流
                var modifiedWorkflow = await PrepareWorkflow(prompt, uploadedImageName, uploadedRefImageName, pixcels);
                if (modifiedWorkflow == null)
                {
                    return null;
                }

                // 创建完整的请求对象
                var requestData = new Dictionary<string, object>
                {
                    ["prompt"] = modifiedWorkflow
                };

                // 发送工作流到ComfyUI并获取promptId
                string? promptId = await _comfyClient.ExecuteWorkflow(requestData);
                if (string.IsNullOrEmpty(promptId))
                {
                    MessageBox.Show("执行工作流失败");
                    return null;
                }

                // 轮询等待结果
                string resultImagePath = await _comfyClient.PollForResult(promptId, "25"); // 25是SaveImage节点ID
                
                if (string.IsNullOrEmpty(resultImagePath))
                {
                    MessageBox.Show("无法获取生成的图片");
                    return null;
                }

                return resultImagePath;
            }
            catch (Exception ex)
            {
                MessageBox.Show("处理图像时出错：" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 准备工作流，根据绘制的图片和提示词更新工作流
        /// </summary>
        private async Task<Dictionary<string, object>?> PrepareWorkflow(string prompt, string uploadedImageName, string uploadRefImgName, int[] pixcels)
        {
            try
            {
                // 加载工作流 JSON
                string workflowJson = string.Empty;
                
                // 如果工作流文件存在，读取它，否则使用默认的工作流
                if (File.Exists(_workflowPath))
                {
                    workflowJson = await File.ReadAllTextAsync(_workflowPath);
                }
                else
                {
                    // 使用硬编码的工作流 JSON 作为备份
                     throw new Exception("缺失工作流JSON");
                }

                // 解析工作流 JSON
                var workflow = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(workflowJson);
                if (workflow == null)
                {
                    MessageBox.Show("无法解析工作流数据");
                    return null;
                }

                // 创建一个新的工作流副本，以便修改
                var modifiedWorkflow = new Dictionary<string, object>();
                foreach (var node in workflow)
                {
                    modifiedWorkflow[node.Key] = JsonSerializer.Deserialize<object>(node.Value.GetRawText());
                }

                // 修改节点 17（LoadImage）使用我们上传的图片
                if (!string.IsNullOrEmpty(uploadedImageName))
                {
                    var node6 = JsonSerializer.Deserialize<Dictionary<string, object>>(
                        workflow["17"].GetRawText());
                        
                    if (node6 != null && node6.ContainsKey("inputs"))
                    {
                        var inputs = JsonSerializer.Deserialize<Dictionary<string, object>>(
                            ((JsonElement)node6["inputs"]).GetRawText());
                            
                        if (inputs != null)
                        {
                            inputs["image"] = uploadedImageName;
                            node6["inputs"] = inputs;
                            modifiedWorkflow["17"] = node6;
                        }
                    }
                }

                // 修改节点 19（LoadImage）使用我们上传的参考图片
                if (!string.IsNullOrEmpty(uploadRefImgName))
                {
                    var node19 = JsonSerializer.Deserialize<Dictionary<string, object>>(
                        workflow["19"].GetRawText());
                        
                    if (node19 != null && node19.ContainsKey("inputs"))
                    {
                        var inputs = JsonSerializer.Deserialize<Dictionary<string, object>>(
                            ((JsonElement)node19["inputs"]).GetRawText());
                            
                        if (inputs != null)
                        {
                            inputs["image"] = uploadRefImgName;
                            node19["inputs"] = inputs;
                            modifiedWorkflow["19"] = node19;
                        }
                    }
                }

                // 修改节点 13（LantentImage）使用我们设置的像素值
                    var node13 = JsonSerializer.Deserialize<Dictionary<string, object>>(
                    workflow["13"].GetRawText());
                    
                    if (node13 != null && node13.ContainsKey("inputs"))
                    {
                        var inputs = JsonSerializer.Deserialize<Dictionary<string, object>>(
                            ((JsonElement)node13["inputs"]).GetRawText());
                            
                        if (inputs != null)
                        {
                            inputs["width"] = pixcels[0];
                            inputs["height"] = pixcels[1];
                            node13["inputs"] = inputs;
                            modifiedWorkflow["13"] = node13;
                        }
                    }
                

                // 修改节点 22（DeepTranslatorCLIPTextEncode）使用我们的提示词
                var node22 = JsonSerializer.Deserialize<Dictionary<string, object>>(
                    workflow["22"].GetRawText());
                    
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

                return modifiedWorkflow;
            }
            catch (Exception ex)
            {
                MessageBox.Show("准备工作流时出错：" + ex.Message);
                return null;
            }
        }
    }
}