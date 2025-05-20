using System.Text;
using System.Text.Json;
using System.Net.WebSockets;

namespace MapGenerator.Request.ComfyUI
{
    enum ProcessorType
    {
        DrawToImage,
        Inpainting,
        StyleComposition,
        DrawToImageWithRef,
    }
    public class ComfyUIClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _serverUrl;

        public ComfyUIClient(string serverUrl = "")
        {
            if (string.IsNullOrEmpty(serverUrl)) 
                serverUrl = AppSettings.comfyuiServerUrl;
            _httpClient = new HttpClient();
            _serverUrl = serverUrl;
        }

        #region Common

        /// <summary>
        /// 取消当前正在执行的任务
        /// </summary>
        public async Task CancelCurrentExecution()
        {
            try
            {
                // 调用ComfyUI的取消当前执行队列的API
                HttpResponseMessage response = await _httpClient.PostAsync($"{_serverUrl}/api/v1/queue/clear", null);

                // 稍微等待一下，确保队列已被清空
                await Task.Delay(500);

                // 可选：检查响应是否成功
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"清除队列失败: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                // 记录错误但不中断流程，即使清除队列失败也继续执行
                Console.WriteLine($"清除队列时出错: {ex.Message}");
            }
        }

        /// <summary>
        /// 上传图片到ComfyUI
        /// </summary>
        /// <param name="imagePath">要上传的图片路径</param>
        /// <returns>上传后的图片名称，如果上传失败则返回空字符串</returns>
        public async Task<string> UploadImage(string imagePath)
        {
            try
            {
                using var fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
                var fileContent = new StreamContent(fileStream);

                using var formData = new MultipartFormDataContent();
                formData.Add(fileContent, "image", Path.GetFileName(imagePath));

                var response = await _httpClient.PostAsync($"{_serverUrl}/upload/image", formData);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    // 从响应中提取图片名称
                    var responseJson = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody);
                    if (responseJson != null && responseJson.TryGetValue("name", out var nameElement))
                    {
                        return nameElement.GetString() ?? string.Empty;
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"上传图片时出错：{ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// 执行工作流，发送到ComfyUI
        /// </summary>
        /// <param name="workflowData">工作流数据</param>
        /// <returns>提示ID，用于后续轮询结果</returns>
        public async Task<string?> ExecuteWorkflow(Dictionary<string, object> workflowData)
        {
            try
            {
                // 序列化为 JSON
                string jsonContent = JsonSerializer.Serialize(workflowData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // 发送请求到 ComfyUI API
                HttpResponseMessage response = await _httpClient.PostAsync($"{_serverUrl}/prompt", content);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // 输出响应以便调试
                    Console.WriteLine($"ComfyUI 响应: {responseBody}");

                    // 获取提示 ID
                    var responseJson = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody);
                    if (responseJson != null && responseJson.TryGetValue("prompt_id", out var promptIdElement))
                    {
                        return promptIdElement.GetString();
                    }
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"服务器返回错误：{response.StatusCode} - {errorContent}");
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"执行工作流时出错：{ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 轮询等待结果，并通过回调同步进度
        /// </summary>
        /// <param name="promptId">提示ID</param>
        /// <param name="nodeId">要获取图像的节点ID</param>
        /// <returns>生成的图片路径，如果未找到则返回空字符串</returns>
        /// <exception cref="ComfyUIExecutionException">当ComfyUI执行失败时抛出</exception>
        public async Task<string> PollForResult(string promptId, string nodeId, int batch = 1)
        {
            int totalSeconds = 600 * batch; // 超时时长600秒
            for (int i = 0; i < totalSeconds; i++)
            {
                try
                {
                    var statusResponse = await _httpClient.GetAsync($"{_serverUrl}/history/{promptId}");
                    if (statusResponse.IsSuccessStatusCode)
                    {
                        string statusBody = await statusResponse.Content.ReadAsStringAsync();
                        var statusJson = JsonSerializer.Deserialize<JsonElement>(statusBody);
                        if (statusJson.ValueKind == JsonValueKind.Object && statusJson.TryGetProperty(promptId, out var promptData))
                        {
                            // 检查输出
                            if (promptData.TryGetProperty("outputs", out var outputs))
                            {
                                if (outputs.TryGetProperty(nodeId, out var outputNode) &&
                                    outputNode.TryGetProperty("images", out var images) &&
                                    images.ValueKind == JsonValueKind.Array &&
                                    images.GetArrayLength() > 0)
                                {
                                    if (images.GetArrayLength() == 1)
                                    {
                                        var firstImage = images[0];
                                        if (firstImage.TryGetProperty("filename", out var filenameElement))
                                        {
                                            string filename = filenameElement.GetString() ?? string.Empty;
                                            if (!string.IsNullOrEmpty(filename))
                                            {
                                                string outputDir = Path.Combine(AppSettings.OutputDirectory, "temp");
                                                Directory.CreateDirectory(outputDir);
                                                string localPath = Path.Combine(outputDir, Path.GetFileName(filename));
                                                string imageUrl = $"{_serverUrl}/view?filename={Uri.EscapeDataString(filename)}";
                                                bool downloadSuccess = await DownloadImage(imageUrl, localPath);
                                                if (downloadSuccess)
                                                {
                                                    return localPath;
                                                }
                                            }
                                        }
                                    }
                                    else if (images.GetArrayLength() > 1)
                                    {
                                        return await ShowImageSelectionDialog(images);
                                    }
                                }
                                // 检查是否执行完成
                                bool isExecutionComplete = false;
                                if (promptData.TryGetProperty("status", out var statusProp))
                                {
                                    string status = statusProp.GetString() ?? string.Empty;
                                    if (status == "complete" || status == "success")
                                    {
                                        isExecutionComplete = true;
                                    }
                                }
                                if (isExecutionComplete)
                                {
                                    return string.Empty;
                                }
                            }
                        }
                    }
                    else
                    {
                        if ((int)statusResponse.StatusCode >= 400 && i > 5)
                        {
                            throw new ComfyUIExecutionException($"ComfyUI服务器返回错误: {statusResponse.StatusCode}");
                        }
                    }
                }
                catch (ComfyUIExecutionException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    if (i > 5)
                    {
                        throw new ComfyUIExecutionException($"轮询过程中多次出错: {ex.Message}", ex);
                    }
                }
                await Task.Delay(1000);
            }
            throw new ComfyUIExecutionException($"轮询超时（{totalSeconds}秒），无法获取节点{nodeId}的图像");
        }

        /// <summary>
        /// ComfyUI执行异常
        /// </summary>
        public class ComfyUIExecutionException : Exception
        {
            public ComfyUIExecutionException(string message) : base(message) { }
            public ComfyUIExecutionException(string message, Exception innerException) : base(message, innerException) { }
        }


        /// <summary>
        /// 显示图像选择对话框，让用户从多个生成的图像中选择一个
        /// </summary>
        /// <param name="images">JSON 图像数组</param>
        /// <returns>选中图片的本地路径，如果未选择则返回空字符串</returns>
        private async Task<string> ShowImageSelectionDialog(JsonElement images)
        {
            // 1. 先异步批量下载所有图片
            var imageInfos = new List<(string filename, string tempFile)>();
            for (int i = 0; i < images.GetArrayLength(); i++)
            {
                var image = images[i];
                if (image.TryGetProperty("filename", out var filenameElement))
                {
                    string filename = filenameElement.GetString() ?? string.Empty;
                    if (!string.IsNullOrEmpty(filename))
                    {
                        string tempDir = Path.Combine(AppSettings.OutputDirectory, "temp");
                        Directory.CreateDirectory(tempDir);
                        string tempFile = Path.Combine(tempDir, $"preview_{i}_{Path.GetFileName(filename)}");
                        string imageUrl = $"{_serverUrl}/view?filename={Uri.EscapeDataString(filename)}";
                        Console.WriteLine($"临时下载预览图像: {imageUrl}");
                        try
                        {
                            var response = await _httpClient.GetAsync(imageUrl);
                            if (response.IsSuccessStatusCode)
                            {
                                using (var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
                                {
                                    await response.Content.CopyToAsync(fs);
                                }
                                imageInfos.Add((filename, tempFile));
                            }
                            else
                            {
                                Console.WriteLine($"下载预览图像失败: {response.StatusCode}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"下载预览图像时出错: {ex.Message}");
                        }
                    }
                }
            }

            // 2. 再同步构建UI并显示选择对话框
            string? selectedImagePath = null;
            await Task.Run(() =>
            {
                using (Form selectionDialog = new Form())
                {
                    selectionDialog.Text = $"请选择要保存的图像 (双击可放大查看)";
                    selectionDialog.Size = new Size(800, 600);
                    selectionDialog.StartPosition = FormStartPosition.CenterScreen;
                    selectionDialog.MinimizeBox = false;
                    selectionDialog.MaximizeBox = false;
                    selectionDialog.FormBorderStyle = FormBorderStyle.FixedDialog;

                    FlowLayoutPanel imagePanel = new FlowLayoutPanel
                    {
                        Dock = DockStyle.Fill,
                        AutoScroll = true,
                        FlowDirection = FlowDirection.LeftToRight,
                        WrapContents = true,
                        Padding = new Padding(10),
                    };

                    Panel bottomPanel = new Panel
                    {
                        Dock = DockStyle.Bottom,
                        Height = 50
                    };

                    Button saveButton = new Button
                    {
                        Text = "保存选中图片",
                        Width = 120,
                        Height = 30,
                        Location = new Point(selectionDialog.Width / 2 - 130, 10),
                        Enabled = false
                    };

                    Button cancelButton = new Button
                    {
                        Text = "取消",
                        Width = 120,
                        Height = 30,
                        Location = new Point(selectionDialog.Width / 2 + 10, 10),
                        DialogResult = DialogResult.Cancel
                    };

                    List<Components.CheckableImageItem> imageItems = new List<Components.CheckableImageItem>();
                    Components.CheckableImageItem? selectedItem = null;

                    for (int i = 0; i < imageInfos.Count; i++)
                    {
                        var (filename, tempFile) = imageInfos[i];
                        try
                        {
                            Components.CheckableImageItem imageItem = new Components.CheckableImageItem();
                            imageItem.SetContent($"{i}", tempFile, [180, 180]);
                            imageItem.Click += (sender, e) =>
                            {
                                if (selectedItem != null) selectedItem.Selected = false;
                                var item = sender as Components.CheckableImageItem;
                                if (item != null)
                                {
                                    item.Selected = true;
                                    selectedItem = item;
                                    saveButton.Enabled = true;
                                }
                            };
                            imageItem.DoubleClick += (sender, e) =>
                            {
                                var item = sender as Components.CheckableImageItem;
                                if (item != null)
                                {
                                    ShowLargeImagePreview(item.FilePath, tempFile);
                                }
                            };
                            imageItems.Add(imageItem);
                            imagePanel.Controls.Add(imageItem);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"加载预览图像时出错: {ex.Message}");
                        }
                    }

                    saveButton.Click += (sender, e) =>
                    {
                        if (selectedItem != null)
                        {
                            selectedImagePath = selectedItem.FilePath;
                            selectionDialog.DialogResult = DialogResult.OK;
                        }
                    };

                    bottomPanel.Controls.Add(saveButton);
                    bottomPanel.Controls.Add(cancelButton);
                    selectionDialog.Controls.Add(imagePanel);
                    selectionDialog.Controls.Add(bottomPanel);

                    selectionDialog.FormClosed += (sender, e) =>
                    {
                        foreach (var (_, tempFile) in imageInfos)
                        {
                            try
                            {
                                if (File.Exists(tempFile))
                                {
                                    File.Delete(tempFile);
                                }
                            }
                            catch { }
                        }
                    };

                    Application.EnableVisualStyles();
                    if (selectionDialog.ShowDialog() != DialogResult.OK)
                    {
                        selectedImagePath = null;
                    }
                }
            });
            return selectedImagePath ?? string.Empty;
        }

        /// <summary>
        /// 显示大图预览
        /// </summary>
        private void ShowLargeImagePreview(string filename, string localPath)
        {
            using (Form previewForm = new Form())
            {
                previewForm.Text = Path.GetFileName(filename);
                previewForm.Size = new Size(1024, 768);
                previewForm.StartPosition = FormStartPosition.CenterScreen;
                previewForm.Icon = SystemIcons.Information;

                PictureBox pictureBox = new PictureBox
                {
                    Dock = DockStyle.Fill,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Image = Image.FromFile(localPath)
                };

                Button closeButton = new Button
                {
                    Text = "关闭",
                    Dock = DockStyle.Bottom,
                    Height = 30
                };

                closeButton.Click += (s, e) => previewForm.Close();

                previewForm.Controls.Add(pictureBox);
                previewForm.Controls.Add(closeButton);
                previewForm.ShowDialog();
            }
        }

        /// <summary>
        /// 检查队列状态
        /// </summary>
        private async Task CheckQueueStatus(int pollAttempt)
        {
            try
            {
                var queueResponse = await _httpClient.GetAsync($"{_serverUrl}/queue");
                if (queueResponse.IsSuccessStatusCode)
                {
                    string queueBody = await queueResponse.Content.ReadAsStringAsync();
                    var queueJson = JsonSerializer.Deserialize<JsonElement>(queueBody);

                    bool isRunning = false;
                    bool isQueued = false;

                    // 检查是否有正在运行的任务
                    if (queueJson.TryGetProperty("running_items", out var runningItems) &&
                        runningItems.ValueKind == JsonValueKind.Array &&
                        runningItems.GetArrayLength() > 0)
                    {
                        isRunning = true;
                        Console.WriteLine("ComfyUI仍在处理请求...");
                    }

                    // 检查是否有排队的任务
                    if (queueJson.TryGetProperty("queue_items", out var pendingItems) &&
                        pendingItems.ValueKind == JsonValueKind.Array &&
                        pendingItems.GetArrayLength() > 0)
                    {
                        isQueued = true;
                        Console.WriteLine("请求正在队列中等待...");
                    }

                    // 如果没有正在运行或排队的任务，但历史记录中显示执行尚未完成，可能出现了问题
                    if (!isRunning && !isQueued && pollAttempt > 10)
                    {
                        Console.WriteLine("没有运行中或排队的任务，等待额外时间...");

                        // 等待更长时间，以防刚刚完成
                        await Task.Delay(2000);
                    }
                }
                else
                {
                    Console.WriteLine($"获取队列状态失败: {queueResponse.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"检查队列状态时出错: {ex.Message}");
            }
        }

        /// <summary>
        /// 下载图片
        /// </summary>
        private async Task<bool> DownloadImage(string url, string savePath)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    using var fs = new FileStream(savePath, FileMode.Create, FileAccess.Write);
                    await response.Content.CopyToAsync(fs);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"下载图片时出错: {ex.Message}");
                return false;
            }
        }

        #endregion
    }
}