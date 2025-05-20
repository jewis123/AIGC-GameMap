using System.Text.Json;

namespace MapGenerator.Request.ComfyUI
{
    public class FluxStyleChangeProcessor : BaseProcessor
    {

        private readonly string _workflowPath;

        public FluxStyleChangeProcessor(ref ComfyUIClient comfyClient) : base(ref comfyClient)
        {
            // 将工作流 JSON 文件路径设置为工作流 JSON 文件位置
            _workflowPath = Path.Combine(AppSettings.WorkflowPath, "2dmap_fluxchange_ip.json");
        }

        /// <summary>
        /// 执行风格替换
        /// </summary>
        /// <param name="rawImagePath"></param>
        /// <param name="refImagePath"></param>
        /// <param name="batchSize"></param>
        /// <param name="style_key"></param>
        /// <param name="denoise"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public async Task<string> Process(string rawImagePath, string refImagePath, int batchSize, string style_key, float denoise, IProgress<int> progress)
        {
            try
            {
                // 先取消当前正在执行的任务，避免排队
                await _comfyClient.CancelCurrentExecution();

                //上传原图
                progress.Report(10);
                string uploadedRawImageName = string.Empty;
                if (!string.IsNullOrEmpty(rawImagePath) && File.Exists(rawImagePath))
                {
                    uploadedRawImageName = await _comfyClient.UploadImage(rawImagePath);
                    if (string.IsNullOrEmpty(uploadedRawImageName))
                    {
                        MessageBox.Show("原始图片上传失败");
                        return string.Empty;
                    }
                }

                //上传参考图
                progress.Report(15);
                string uploadedRefImageName = string.Empty;
                if (!string.IsNullOrEmpty(refImagePath) && File.Exists(refImagePath))
                {
                    uploadedRefImageName = await _comfyClient.UploadImage(refImagePath);
                    if (string.IsNullOrEmpty(uploadedRefImageName))
                    {
                        MessageBox.Show("参考图片上传失败");
                        return string.Empty;
                    }
                }



                progress.Report(30);
                var modifiedWorkflow = await PrepareWorkflow(uploadedRawImageName, uploadedRefImageName, batchSize, denoise, style_key);
                if (modifiedWorkflow == null)
                {
                    return string.Empty;
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
                    return string.Empty;
                }

                LastPromptId = promptId;
                LastNodeId = "157";

                // 轮询等待结果
                progress.Report(40);
                string resultImagePath = await _comfyClient.PollForResult(promptId, LastNodeId, batchSize); // SaveImage节点ID

                if (string.IsNullOrEmpty(resultImagePath))
                {
                    MessageBox.Show("无法获取生成的图片");
                    return string.Empty;
                }


                return resultImagePath;
            }
            catch (Exception ex)
            {
                MessageBox.Show("处理图像时出错：" + ex.Message);
                return string.Empty;
            }
        }

        private async Task<Dictionary<string, object>?> PrepareWorkflow(string uploadedImageName,string uploadedRefImageName, int batchSize, float denoise, string style_key)
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

                // 修改节点 151（LoadImage）使用我们上传的图片
                if (!string.IsNullOrEmpty(uploadedImageName))
                {
                    var node151 = JsonSerializer.Deserialize<Dictionary<string, object>>(
                        workflow["151"].GetRawText());

                    if (node151 != null && node151.ContainsKey("inputs"))
                    {
                        var inputs = JsonSerializer.Deserialize<Dictionary<string, object>>(
                            ((JsonElement)node151["inputs"]).GetRawText());

                        if (inputs != null)
                        {
                            inputs["image"] = uploadedImageName;
                            node151["inputs"] = inputs;
                            modifiedWorkflow["151"] = node151;
                        }
                    }
                }

                // 修改节点 190（LoadImage）使用我们上传的参考图片
                if (!string.IsNullOrEmpty(uploadedImageName))
                {
                    var node190 = JsonSerializer.Deserialize<Dictionary<string, object>>(
                        workflow["190"].GetRawText());

                    if (node190 != null && node190.ContainsKey("inputs"))
                    {
                        var inputs = JsonSerializer.Deserialize<Dictionary<string, object>>(
                            ((JsonElement)node190["inputs"]).GetRawText());

                        if (inputs != null)
                        {
                            inputs["image"] = uploadedRefImageName;
                            node190["inputs"] = inputs;
                            modifiedWorkflow["190"] = node190;
                        }
                    }
                }


                // 修改节点 207（Denoise）使用我们设置的Denoise值
                var node207 = JsonSerializer.Deserialize<Dictionary<string, object>>(
                                workflow["207"].GetRawText());

                if (node207 != null && node207.ContainsKey("inputs"))
                {
                    var inputs = JsonSerializer.Deserialize<Dictionary<string, object>>(
                        ((JsonElement)node207["inputs"]).GetRawText());

                    if (inputs != null)
                    {
                        inputs["value"] = denoise;
                        node207["inputs"] = inputs;
                        modifiedWorkflow["207"] = node207;
                    }
                }


                // 修改节点 215（BatchSize）使用我们设置的批处理大小
                var node215 = JsonSerializer.Deserialize<Dictionary<string, object>>(
                workflow["215"].GetRawText());

                if (node215 != null && node215.ContainsKey("inputs"))
                {
                    var inputs = JsonSerializer.Deserialize<Dictionary<string, object>>(
                        ((JsonElement)node215["inputs"]).GetRawText());

                    if (inputs != null)
                    {
                        inputs["batch_size"] = batchSize;
                        node215["inputs"] = inputs;
                        modifiedWorkflow["215"] = node215;
                    }
                }


                var node154 = JsonSerializer.Deserialize<Dictionary<string, object>>(
                    workflow["154"].GetRawText());

                if (node154 != null && node154.ContainsKey("inputs"))
                {
                    var inputs = JsonSerializer.Deserialize<Dictionary<string, object>>(
                        ((JsonElement)node154["inputs"]).GetRawText());

                    if (inputs != null)
                    {
                        inputs["lora_name"] = AppSettings.GetLoraPath(style_key);
                        node154["inputs"] = inputs;
                        modifiedWorkflow["154"] = node154;
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