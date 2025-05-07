using System.Text.Json;

namespace MapGenerator.Request.ComfyUI
{
    public class ImgIPAdapterProcessor:BaseProcessor
    {
        private readonly string _workflowPath;

        public ImgIPAdapterProcessor(ref ComfyUIClient comfyClient):base(ref comfyClient)
        {
            // 将工作流 JSON 文件路径设置为工作流 JSON 文件位置
            _workflowPath = Path.Combine(AppSettings.AssetsDirectory, "workflow", "2dmapIPAdapter2.json");
        }

        /// <summary>
        /// 处理参考到图像的转换，上传用户参考的图片，将其发送到ComfyUI进行处理
        /// </summary>
        /// <param name="prompt">用于生成的提示词</param>
        /// <param name="imagePath">用户参考的图片路径</param>
        /// <returns>生成的图片路径，如果处理失败则返回null</returns>
        public async Task<string?> Process(string prompt, string imagePath, IProgress<int> progress)
        {
            try
            {
                // 先取消当前正在执行的任务，避免排队
                await _comfyClient.CancelCurrentExecution();

                // 上传原图片
                progress.Report(10);
                string uploadedImageName = string.Empty;
                if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                {
                    uploadedImageName = await _comfyClient.UploadImage(imagePath);
                    if (string.IsNullOrEmpty(uploadedImageName))
                    {
                        MessageBox.Show("图片上传失败");
                        return null;
                    }
                }

                // 准备工作流
                progress.Report(30);
                var modifiedWorkflow = await PrepareWorkflow(prompt, uploadedImageName);
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
                progress.Report(40);
                string? promptId = await _comfyClient.ExecuteWorkflow(requestData);
                if (string.IsNullOrEmpty(promptId))
                {
                    MessageBox.Show("执行工作流失败");
                    return null;
                }

                // 轮询等待结果
                string resultImagePath = await _comfyClient.PollForResult(promptId, "229"); // 229是SaveImage节点ID
                
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
        private async Task<Dictionary<string, object>?> PrepareWorkflow(string prompt, string uploadedImageName)
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

                // 修改节点 194（LoadImage）使用我们上传的图片
                if (!string.IsNullOrEmpty(uploadedImageName))
                {
                    var node194 = JsonSerializer.Deserialize<Dictionary<string, object>>(
                        workflow["194"].GetRawText());
                        
                    if (node194 != null && node194.ContainsKey("inputs"))
                    {
                        var inputs = JsonSerializer.Deserialize<Dictionary<string, object>>(
                            ((JsonElement)node194["inputs"]).GetRawText());
                            
                        if (inputs != null)
                        {
                            inputs["image"] = uploadedImageName;
                            node194["inputs"] = inputs;
                            modifiedWorkflow["194"] = node194;
                        }
                    }
                }

                // 修改节点 199使用我们的提示词
                var node199 = JsonSerializer.Deserialize<Dictionary<string, object>>(
                    workflow["199"].GetRawText());
                    
                if (node199 != null && node199.ContainsKey("inputs"))
                {
                    var inputs = JsonSerializer.Deserialize<Dictionary<string, object>>(
                        ((JsonElement)node199["inputs"]).GetRawText());
                        
                    if (inputs != null)
                    {
                        inputs["text"] = prompt;
                        node199["inputs"] = inputs;
                        modifiedWorkflow["199"] = node199;
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