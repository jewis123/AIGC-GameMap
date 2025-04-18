using System.Text;
using System.Text.Json;

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

        public ComfyUIClient(string serverUrl = "http://192.168.218.86:8089")
        {
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
        /// 轮询等待结果
        /// </summary>
        /// <param name="promptId">提示ID</param>
        /// <param name="nodeId">要获取图像的节点ID</param>
        /// <returns>生成的图片路径，如果未找到则返回空字符串</returns>
        public async Task<string> PollForResult(string promptId, string nodeId, float factor=1)
        {
            // 轮询最多60秒
            for (int i = 0; i < 60 * factor; i++)
            {
                try
                {
                    // 输出调试信息
                    Console.WriteLine($"正在轮询结果，尝试 {i + 1}/60...");

                    // 检查执行状态
                    var statusResponse = await _httpClient.GetAsync($"{_serverUrl}/history/{promptId}");

                    if (statusResponse.IsSuccessStatusCode)
                    {
                        string statusBody = await statusResponse.Content.ReadAsStringAsync();
                        Console.WriteLine($"获取到历史响应: {statusBody.Substring(0, Math.Min(statusBody.Length, 200))}...");

                        var statusJson = JsonSerializer.Deserialize<JsonElement>(statusBody);

                        // 检查是否有输出节点
                        if (statusJson.ValueKind == JsonValueKind.Object)
                        {
                            // 查找指定节点ID的输出
                            if (statusJson.TryGetProperty(promptId, out var promptData) &&
                                promptData.TryGetProperty("outputs", out var outputs))
                            {
                                // 查找指定ID的节点
                                if (outputs.TryGetProperty(nodeId, out var outputNode) &&
                                    outputNode.TryGetProperty("images", out var images) &&
                                    images.ValueKind == JsonValueKind.Array &&
                                    images.GetArrayLength() > 0)
                                {
                                    // 如果只有一个图像，直接下载并返回路径
                                    if (images.GetArrayLength() == 1)
                                    {
                                        var firstImage = images[0];
                                        if (firstImage.TryGetProperty("filename", out var filenameElement))
                                        {
                                            string filename = filenameElement.GetString() ?? string.Empty;
                                            if (!string.IsNullOrEmpty(filename))
                                            {
                                                Console.WriteLine($"找到节点{nodeId}的图像文件名: {filename}");

                                                string outputDir = Path.Combine(AppSettings.OutputDirectory, "temp");
                                                // 下载图片
                                                Directory.CreateDirectory(outputDir);

                                                string localPath = Path.Combine(outputDir, Path.GetFileName(filename));

                                                // 修正图像URL
                                                string imageUrl = $"{_serverUrl}/view?filename={Uri.EscapeDataString(filename)}";
                                                Console.WriteLine($"下载图像URL: {imageUrl}");

                                                bool downloadSuccess = await DownloadImage(imageUrl, localPath);
                                                if (downloadSuccess)
                                                {
                                                    Console.WriteLine($"图像已下载到: {localPath}");
                                                    return localPath;
                                                }
                                            }
                                        }
                                    }
                                    // 如果有多个图像，显示选择对话框
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
                                    Console.WriteLine($"工作流执行完成，但未找到节点{nodeId}的输出。");
                                    return string.Empty;
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"获取历史请求失败: {statusResponse.StatusCode}");
                    }

                    // 检查队列状态
                    await CheckQueueStatus(i);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"轮询过程中出错: {ex.Message}");
                }

                // 等待1秒后再次尝试
                await Task.Delay(1000);
            }

            Console.WriteLine($"轮询超时，无法获取节点{nodeId}的图像");
            return string.Empty;
        }

        /// <summary>
        /// 显示图像选择对话框，让用户从多个生成的图像中选择一个
        /// </summary>
        /// <param name="images">JSON 图像数组</param>
        /// <returns>选中图片的本地路径，如果未选择则返回空字符串</returns>
        private async Task<string> ShowImageSelectionDialog(JsonElement images)
        {
            // 在UI线程上运行
            string? selectedImagePath = null;
            await Task.Run(() =>
            {
                // 使用SynchronizationContext在UI线程上执行
                using (Form selectionDialog = new Form())
                {
                    selectionDialog.Text = $"请选择要保存的图像 (双击可放大查看)";
                    selectionDialog.Size = new Size(800, 600);
                    selectionDialog.StartPosition = FormStartPosition.CenterScreen;
                    selectionDialog.MinimizeBox = false;
                    selectionDialog.MaximizeBox = false;
                    selectionDialog.FormBorderStyle = FormBorderStyle.FixedDialog;

                    // 创建FlowLayoutPanel用于显示图像
                    FlowLayoutPanel imagePanel = new FlowLayoutPanel
                    {
                        Dock = DockStyle.Fill,
                        AutoScroll = true,
                        FlowDirection = FlowDirection.LeftToRight,
                        WrapContents = true,
                        Padding = new Padding(10),
                    };

                    // 创建底部面板
                    Panel bottomPanel = new Panel
                    {
                        Dock = DockStyle.Bottom,
                        Height = 50
                    };

                    // 创建按钮
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

                    // 存储图像项目和临时下载的文件
                    List<Components.CheckableImageItem> imageItems = new List<Components.CheckableImageItem>();
                    List<string> tempFiles = new List<string>();
                    Components.CheckableImageItem? selectedItem = null;

                    // 加载图像
                    for (int i = 0; i < images.GetArrayLength(); i++)
                    {
                        var image = images[i];
                        if (image.TryGetProperty("filename", out var filenameElement))
                        {
                            string filename = filenameElement.GetString() ?? string.Empty;
                            if (!string.IsNullOrEmpty(filename))
                            {
                                try
                                {
                                    // 临时下载图像
                                    string tempDir = Path.Combine(AppSettings.OutputDirectory, "temp");
                                    Directory.CreateDirectory(tempDir);
                                    string tempFile = Path.Combine(tempDir, $"preview_{i}_{Path.GetFileName(filename)}");
                                    tempFiles.Add(tempFile);

                                    // 异步下载图像
                                    string imageUrl = $"{_serverUrl}/view?filename={Uri.EscapeDataString(filename)}";
                                    Console.WriteLine($"临时下载预览图像: {imageUrl}");

                                    using (var webClient = new System.Net.WebClient())
                                    {
                                        webClient.DownloadFile(imageUrl, tempFile);
                                    }

                                    // 创建图像项目
                                    Components.CheckableImageItem imageItem = new Components.CheckableImageItem();
                                    imageItem.SetContent($"{i}", tempFile, [180, 180]);

                                    // 添加点击事件
                                    imageItem.Click += (sender, e) =>
                                    {
                                        // 先取消之前选中的
                                        if (selectedItem != null) selectedItem.Selected = false;

                                        // 设置当前选中
                                        var item = (Components.CheckableImageItem)sender;
                                        item.Selected = true;
                                        selectedItem = item;
                                        saveButton.Enabled = true;
                                    };

                                    // 添加双击事件以放大查看
                                    imageItem.DoubleClick += (sender, e) =>
                                    {
                                        var item = (Components.CheckableImageItem)sender;
                                        ShowLargeImagePreview(item.FilePath, tempFile);
                                    };

                                    imageItems.Add(imageItem);
                                    imagePanel.Controls.Add(imageItem);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"加载预览图像时出错: {ex.Message}");
                                }
                            }
                        }
                    }

                    // 设置保存按钮事件
                    saveButton.Click += (sender, e) =>
                    {
                        if (selectedItem != null)
                        {
                            selectedImagePath = selectedItem.FilePath;
                            selectionDialog.DialogResult = DialogResult.OK;
                        }
                    };


                    // 添加控件到表单
                    bottomPanel.Controls.Add(saveButton);
                    bottomPanel.Controls.Add(cancelButton);
                    selectionDialog.Controls.Add(imagePanel);
                    selectionDialog.Controls.Add(bottomPanel);

                    // 表单关闭事件，清理临时文件
                    selectionDialog.FormClosed += (sender, e) =>
                    {
                        // 清理临时下载的文件
                        foreach (var tempFile in tempFiles)
                        {
                            try
                            {
                                if (File.Exists(tempFile))
                                {
                                    File.Delete(tempFile);
                                }
                            }
                            catch
                            {
                                // 忽略删除临时文件的错误
                            }
                        }
                    };

                    // 显示对话框
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