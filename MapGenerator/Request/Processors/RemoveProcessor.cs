using MapGenerator.Components;
using System.Text.Json;

namespace MapGenerator.Request.ComfyUI
{
    public class RemoveProcessor : BaseProcessor
    {
        private readonly string _workflowPath;

        public RemoveProcessor(ref ComfyUIClient comfyClient) : base(ref comfyClient)
        {
            _workflowPath = Path.Combine(AppSettings.AssetsDirectory, "workflow", "2d_xl_removal.json");
        }

        /// <summary>
        /// 处理区域移除，上传画布和遮罩图像，执行ComfyUI工作流
        /// </summary>
        /// <param name="canvasImagePath">画布图像路径</param>
        /// <param name="maskImagePath">遮罩图像路径</param>
        /// <returns>生成的图片路径，如果处理失败则返回null</returns>
        public async Task Process(string canvasImagePath, string maskImagePath, IProgress<int> progress)
        {
            try
            {
                await _comfyClient.CancelCurrentExecution();

                // 上传画布图像（节点14）
                progress.Report(10);
                string uploadedCanvasName = string.Empty;
                if (!string.IsNullOrEmpty(canvasImagePath) && File.Exists(canvasImagePath))
                {
                    uploadedCanvasName = await _comfyClient.UploadImage(canvasImagePath);
                    if (string.IsNullOrEmpty(uploadedCanvasName))
                    {
                        MessageBox.Show("画布图片上传失败");
                        return ;
                    }
                }

                // 上传遮罩图像（节点22）
                progress.Report(15);
                string uploadedMaskName = string.Empty;
                if (!string.IsNullOrEmpty(maskImagePath) && File.Exists(maskImagePath))
                {
                    uploadedMaskName = await _comfyClient.UploadImage(maskImagePath);
                    if (string.IsNullOrEmpty(uploadedMaskName))
                    {
                        MessageBox.Show("遮罩图片上传失败");
                        return ;
                    }
                }

                // 加载并修改工作流
                progress.Report(30);
                string workflowJson = await File.ReadAllTextAsync(_workflowPath);
                var workflow = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(workflowJson);
                if (workflow == null)
                {
                    MessageBox.Show("无法解析工作流数据");
                    return ;
                }
                var modifiedWorkflow = new Dictionary<string, object>();
                foreach (var node in workflow)
                {
                    modifiedWorkflow[node.Key] = JsonSerializer.Deserialize<object>(node.Value.GetRawText());
                }

                // 替换节点14（画布）
                if (!string.IsNullOrEmpty(uploadedCanvasName))
                {
                    var node14 = JsonSerializer.Deserialize<Dictionary<string, object>>(workflow["14"].GetRawText());
                    if (node14 != null && node14.ContainsKey("inputs"))
                    {
                        var inputs = JsonSerializer.Deserialize<Dictionary<string, object>>(((JsonElement)node14["inputs"]).GetRawText());
                        if (inputs != null)
                        {
                            inputs["image"] = uploadedCanvasName;
                            node14["inputs"] = inputs;
                            modifiedWorkflow["14"] = node14;
                        }
                    }
                }
                // 替换节点22（遮罩）
                if (!string.IsNullOrEmpty(uploadedMaskName))
                {
                    var node22 = JsonSerializer.Deserialize<Dictionary<string, object>>(workflow["22"].GetRawText());
                    if (node22 != null && node22.ContainsKey("inputs"))
                    {
                        var inputs = JsonSerializer.Deserialize<Dictionary<string, object>>(((JsonElement)node22["inputs"]).GetRawText());
                        if (inputs != null)
                        {
                            inputs["image"] = uploadedMaskName;
                            node22["inputs"] = inputs;
                            modifiedWorkflow["22"] = node22;
                        }
                    }
                }

                // 组装请求
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
                    return ;
                }

                LastPromptId = promptId;
                LastNodeId = "20";
            }
            catch (Exception ex)
            {
                MessageBox.Show("处理区域移除时出错：" + ex.Message);
                return ;
            }
        }

    }
}
