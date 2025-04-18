using System.Text.Json;

namespace MapGenerator.Request.ComfyUI
{
    public class StyleChangeProcessor:BaseProcessor
    {
        
        private readonly string _workflowPath;

        private readonly Dictionary<string, string> _styleMapping = new Dictionary<string, string>(){
            {"欧美水彩", "SD1.5\\realisticVision V6.0 B1_V6.0 B1.safetensors"},
        }
;

        public StyleChangeProcessor(ref ComfyUIClient comfyClient):base(ref comfyClient)
        {
            // 将工作流 JSON 文件路径设置为工作流 JSON 文件位置
            _workflowPath = Path.Combine(AppSettings.WorkflowPath, "2dmap_changestyle.json");
        }

        /// <summary>
        /// 执行风格替换
        /// </summary>
        /// <param name="prompt">提示词</param>
        /// <param name="rawImagePath">原图</param>
        /// <param name="refImagePath">参考图</param>
        /// <param name="batchSize">生成数量</param>
        /// <param name="pixcels">生成尺寸</param>
        /// <param name="style_key">风格图key</param>
        /// <returns></returns>
        public async Task<string> Process(string prompt, string rawImagePath, string refImagePath, int batchSize, int[] pixcels, string style_key)
        {
            try
            {
                // 先取消当前正在执行的任务，避免排队
                await _comfyClient.CancelCurrentExecution();

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

                // 准备工作流
                var modifiedWorkflow = await PrepareWorkflow(prompt, uploadedRawImageName, uploadedRefImageName, pixcels, batchSize, style_key);
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

                // 轮询等待结果
                string resultImagePath = await _comfyClient.PollForResult(promptId, "149", pixcels[0]*pixcels[1]*batchSize/(512*512)); // SaveImage节点ID

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
               return null;
            }
        }

        private async Task<Dictionary<string, object>?> PrepareWorkflow(string prompt, string uploadedImageName, string uploadedRefImageName, int[] pixcelSize, int batchSize, string style_key)
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

                if (!string.IsNullOrEmpty(uploadedImageName))
                {
                    var node30 = JsonSerializer.Deserialize<Dictionary<string, object>>(
                        workflow["30"].GetRawText());

                    if (node30 != null && node30.ContainsKey("inputs"))
                    {
                        var inputs = JsonSerializer.Deserialize<Dictionary<string, object>>(
                            ((JsonElement)node30["inputs"]).GetRawText());

                        if (inputs != null)
                        {
                            inputs["image"] = uploadedImageName;
                            node30["inputs"] = inputs;
                            modifiedWorkflow["30"] = node30;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(uploadedRefImageName))
                {
                    var node165 = JsonSerializer.Deserialize<Dictionary<string, object>>(
                        workflow["165"].GetRawText());

                    if (node165 != null && node165.ContainsKey("inputs"))
                    {
                        var inputs = JsonSerializer.Deserialize<Dictionary<string, object>>(
                            ((JsonElement)node165["inputs"]).GetRawText());

                        if (inputs != null)
                        {
                            inputs["image"] = uploadedRefImageName;
                            node165["inputs"] = inputs;
                            modifiedWorkflow["165"] = node165;
                        }
                    }
                }
                else
                {
                    //避开style ipadapter
                    var node41 = JsonSerializer.Deserialize<Dictionary<string, object>>(workflow["41"].GetRawText());

                    if (node41 != null && node41.ContainsKey("inputs"))
                    {
                        var inputs41 = JsonSerializer.Deserialize<Dictionary<string, object>>(((JsonElement)node41["inputs"]).GetRawText());
                        if (inputs41 != null && inputs41["model"] is JsonElement modelElement)
                        {
                            var modelArray = new object[2];
                            modelArray[0] = "49"; // 例如 "165"
                            modelArray[1] = modelElement[1].GetInt32();
                            inputs41["model"] = modelArray;
                            node41["inputs"] = inputs41;
                            modifiedWorkflow["41"] = node41;
                        }
                    }
                }

                var node185 = JsonSerializer.Deserialize<Dictionary<string, object>>(
                workflow["185"].GetRawText());

                if (node185 != null && node185.ContainsKey("inputs"))
                {
                    var inputs = JsonSerializer.Deserialize<Dictionary<string, object>>(
                        ((JsonElement)node185["inputs"]).GetRawText());

                    if (inputs != null)
                    {
                        inputs["width"] = pixcelSize[0];
                        inputs["height"] = pixcelSize[1];
                        node185["inputs"] = inputs;
                        modifiedWorkflow["185"] = node185;
                    }
                }

                var node43 = JsonSerializer.Deserialize<Dictionary<string, object>>(
                workflow["43"].GetRawText());

                if (node43 != null && node43.ContainsKey("inputs"))
                {
                    var inputs = JsonSerializer.Deserialize<Dictionary<string, object>>(
                        ((JsonElement)node43["inputs"]).GetRawText());

                    if (inputs != null)
                    {
                        inputs["batch_size"] = batchSize;
                        node43["inputs"] = inputs;
                        modifiedWorkflow["43"] = node43;
                    }
                }


                var node168 = JsonSerializer.Deserialize<Dictionary<string, object>>(
                    workflow["168"].GetRawText());

                if (node168 != null && node168.ContainsKey("inputs"))
                {
                    var inputs = JsonSerializer.Deserialize<Dictionary<string, object>>(
                        ((JsonElement)node168["inputs"]).GetRawText());

                    if (inputs != null)
                    {
                        inputs["text"] = prompt;
                        node168["inputs"] = inputs;
                        modifiedWorkflow["168"] = node168;
                    }
                }

                var node31 = JsonSerializer.Deserialize<Dictionary<string, object>>(
                    workflow["31"].GetRawText());

                if (node31 != null && node31.ContainsKey("inputs"))
                {
                    var inputs = JsonSerializer.Deserialize<Dictionary<string, object>>(
                        ((JsonElement)node31["inputs"]).GetRawText());

                    if (inputs != null)
                    {
                        inputs["ckpt_name"] = _styleMapping[style_key];
                        node31["inputs"] = inputs;
                        modifiedWorkflow["31"] = node31;
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