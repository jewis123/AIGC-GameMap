using System.Drawing.Imaging;
using MapGenerator.Components;
using MapGenerator.Request.ComfyUI;
using MapGenerator.Utils;

namespace MapGenerator.Wnds
{
    public partial class StyleChangeWnd : Form
    {
        private ComfyUIClient _comfyUIClient;
        private FluxStyleChangeProcessor _changeFluxStyleProcessor;
        private RemoveProcessor _removeProcessor;
        private RulerPainting? curPaintCav => MainViewTab.SelectedTab.Controls[0] as RulerPainting;
        private TabPage? curTab => MainViewTab.SelectedTab;

        internal enum OperationType
        {
            None = 0,
            StyleChange = 1,
            RegionRepaint = 2,
            RegionRemove = 3,
        }
        public string RecordDirName { get; set; }
        private OperationType operationType = OperationType.None;
        private ProgressForm _progressForm;
        private RulerPainting? lastPaintCav;
        private float _denoise = 1;

        private CheckableImageItem? SelectedRefItem { get; set; } = null;
        private CheckableImageItem? SelectStyleItem { get; set; } = null;
        private IconItemControl_V? selectRawImgItem { get; set; } = null;

        private string SelectedStyleKey
        {
            get
            {
                foreach (var item in templateLayout.Controls)
                {
                    if (item is CheckableImageItem refImage && refImage.Selected)
                    {
                        return Path.GetFileNameWithoutExtension(refImage.FilePath);
                    }
                }
                return string.Empty;
            }
        }

        public StyleChangeWnd()
        {
            InitializeComponent();

            InitProcessors();

            _progressForm = new Components.ProgressForm();

            // 添加窗体关闭事件
            this.FormClosing += StyleChangeWnd_FormClosing;

            this.StartPosition = FormStartPosition.CenterScreen;

            // 搜索框事件绑定
            // this.refSearch.TextChanged += (s, e) => FilterRefLayout();
            this.templateSearch.TextChanged += (s, e) => FilterTemplateLayout();
            this.rawSearch.TextChanged += (s, e) => FilterRawImageLayout();
            this.recordSearch.TextChanged += (s, e) => FilterRecordsLayout();

            // 监听tab切换，切换时所有RulerPainting退出遮罩模式
            this.MainViewTab.SelectedIndexChanged += MainViewTab_SelectedIndexChanged_MaskReset;
        }

        // protected override void OnHandleCreated(EventArgs e)
        // {
        //     base.OnHandleCreated(e);
        //     // 只对refLayout开启双缓冲
        //     typeof(Panel).InvokeMember("DoubleBuffered",
        //         System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
        //         null, refLayout, new object[] { true });
        // }

        // 筛选特征参考图片
        // private void FilterRefLayout()
        // {
        //     string keyword = refSearch.Text.Trim().ToLower();
        //     foreach (var ctrl in refLayout.Controls)
        //     {
        //         if (ctrl is CheckableImageItem item)
        //         {
        //             string name = Path.GetFileNameWithoutExtension(item.FilePath).ToLower();
        //             item.Visible = string.IsNullOrEmpty(keyword) || name.Contains(keyword);
        //         }
        //     }
        // }

        // 筛选风格模板图片
        private void FilterTemplateLayout()
        {
            string keyword = templateSearch.Text.Trim().ToLower();
            foreach (var ctrl in templateLayout.Controls)
            {
                if (ctrl is CheckableImageItem item)
                {
                    string name = Path.GetFileNameWithoutExtension(item.FilePath).ToLower();
                    item.Visible = string.IsNullOrEmpty(keyword) || name.Contains(keyword);
                }
            }
        }

        // 筛选原始图片
        private void FilterRawImageLayout()
        {
            string keyword = rawSearch.Text.Trim().ToLower();
            foreach (var ctrl in rawImageLayout.Controls)
            {
                if (ctrl is IconItemControl_V item)
                {
                    string name = Path.GetFileNameWithoutExtension(item.FilePath).ToLower();
                    item.Visible = string.IsNullOrEmpty(keyword) || name.Contains(keyword);
                }
            }
        }

        // 筛选记录图片
        private void FilterRecordsLayout()
        {
            string keyword = recordSearch.Text.Trim().ToLower();
            foreach (var ctrl in recordsLayout.Controls)
            {
                if (ctrl is IconItemControl_V item)
                {
                    string name = Path.GetFileNameWithoutExtension(item.FilePath).ToLower();
                    item.Visible = string.IsNullOrEmpty(keyword) || name.Contains(keyword);
                }
            }
        }

        private void Enter_Load(object sender, EventArgs e)
        {
            // InitializeReferenceListView();
            InitModelImageList();
            InitializeProjectRawListView();
            InitializeChangedListView();
            Utility.NewPaintingTab(ref MainViewTab, true);
            lastPaintCav = curPaintCav;

            MainViewTab.SelectedIndexChanged += (sender, e) =>
            {
                if (lastPaintCav != null)
                {
                    lastPaintCav.OnMaskModeExited -= RulerPainting_MaskModeExited;
                }

                lastPaintCav = curPaintCav;
                lastPaintCav.OnMaskModeExited += RulerPainting_MaskModeExited;

                zoomBar.SetZoomValue(curPaintCav?.GetCanvasZoomScale() ?? 100);
            };

            MainViewTab.MouseUp += MainViewTab_MouseUp;

            zoomBar.LabelText = "缩放";
            zoomBar.OnZoomChanged += (sender, e) =>
            {
                curPaintCav?.SetZoomScale(e);
            };

            zoomBar.OnZoomReset += (sender, e) =>
            {
                curPaintCav?.SetZoomScale(100);
            };

            denoiseBar.LabelText = "重绘幅度";
            denoiseBar.OnZoomChanged += (sender, e) =>
            {
                _denoise = e;
            };
            denoiseBar.OnZoomReset += (sender, e) =>
            {
                _denoise = 1;
            };

            //只有需要涂抹区域时才解锁
            if (curPaintCav != null)
            {
                curPaintCav.LockCanvas();
            }

            // this.tabs.SelectedIndexChanged += tabs_SelectedIndexChanged;
            this.PromptBox.Hint = "输入提示词(可不填)，对生图有20%影响力....";
            // this.refSearch.Hint = "图片筛选..";
            this.templateSearch.Hint = "图片筛选..";
            this.rawSearch.Hint = "图片筛选..";
            this.recordSearch.Hint = "图片筛选..";
        }

        // 只在TabPage标签区域右键时显示菜单
        private void MainViewTab_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                for (int i = 0; i < MainViewTab.TabCount; i++)
                {
                    Rectangle tabRect = MainViewTab.GetTabRect(i);
                    if (tabRect.Contains(e.Location))
                    {
                        MainViewTab.SelectedIndex = i;
                        curMapContextMenu.Show(MainViewTab, e.Location);
                        return;
                    }
                }
            }
        }


        private void RulerPainting_MaskModeExited(object? sender, EventArgs e)
        {
            curPaintCav?.LockCanvas();
        }

        private void RulerPainting_DecoratorModeExited(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tabs_SelectedIndexChanged(object? sender, EventArgs e)
        {
            switch (this.tabs.SelectedIndex)
            {
                case 2:
                    InitializeReferenceListView();
                    break;
                default:
                    break;
            }
        }

        private void InitProcessors()
        {
            _comfyUIClient = new ComfyUIClient();
            _changeFluxStyleProcessor = new FluxStyleChangeProcessor(ref _comfyUIClient);
            _removeProcessor = new RemoveProcessor(ref _comfyUIClient);
        }

        private void InitializeProjectRawListView()
        {
            Task.Run(() => Utility.LoadIconItemControl(true, Utility.GetImagePathsFromFolder(AppSettings.ProjectPreloadImgDir).ToArray(), ref rawImageLayout, OnClickRawImage, [120, 120]));
        }

        private void InitializeReferenceListView()
        {
            List<string> imagePaths = [.. Utility.GetImagePathsFromFolder(AppSettings.RefImageDir)];
            Utility.LoadCheckableImageToFlowLayout(imagePaths.ToArray(), StyleRefImage_Click, ref refLayout);
            if (SelectedRefItem != null)
            {
                SelectedRefItem.Selected = true;
            }
        }

        private void InitModelImageList()
        {
            Task.Run(() => Utility.LoadCheckableImageToFlowLayout(Utility.GetImagePathsFromFolder(AppSettings.StyleTemplateDir).ToArray(), StyleStyleImage_Click, ref templateLayout));
        }

        private void InitializeChangedListView()
        {
            Task.Run(() => Utility.LoadIconItemControl(true, ((List<string>)[.. Utility.GetImagePathsFromFolder(Path.Combine(AppSettings.ArtChangesDirectory, RecordDirName))]).ToArray(), ref recordsLayout, null, size: [120, 120], menuStrip: imgMenu));
        }

        private void ResetControl()
        {
            // 恢复按钮状态
            btnGen.Enabled = true;
            btnGen.Text = "生成";
            operationType = OperationType.None;
            _progressForm.Hide();

        }

        #region Event
        private void btnGen_Click(object sender, EventArgs e)
        {
            curPaintCav?.LockCanvas();

            switch (operationType)
            {
                case OperationType.StyleChange:
                    RquestGen();
                    break;
                case OperationType.RegionRemove:
                    RequestRemoval();
                    break;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                StopGen();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"中止请求出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // 解锁画布，允许用户继续绘制
                curPaintCav?.UnlockCanvas();
                curPaintCav?.SetMaskMode(false);

                // 恢复按钮状态
                btnGen.Enabled = true;
                btnGen.Text = "生成";
            }
        }

        private void OnClickRawImage(object? sender, EventArgs e)
        {
            if (curPaintCav == null)
            { return; }

            if (sender is IconItemControl_V rawImage)
            {
                var filePath = rawImage.FilePath.Replace("preload", "project_raw");
                var newTabPage = Utility.NewPaintingTab(ref MainViewTab, true);
                curPaintCav.LockCanvas();
                newTabPage.Text = Path.GetFileNameWithoutExtension(filePath);

                // 加载图像
                using (Bitmap loadedImage = new Bitmap(filePath))
                {
                    curPaintCav.DisplayImageOnCanvas(loadedImage);
                    this.curTab.Text = rawImage.GetImgName();
                    selectRawImgItem = rawImage;
                }

                operationType = OperationType.StyleChange;
            }
        }

        private void OnClickRecordImage(object? sender, EventArgs e)
        {
            if (sender is IconItemControl_V recordImage)
            {
                var newTabPage = Utility.NewPaintingTab(ref MainViewTab, true);
                newTabPage.Text = Path.GetFileNameWithoutExtension(recordImage.FilePath);
                curPaintCav.LockCanvas();
                // 加载图像
                using (Bitmap loadedImage = new Bitmap(recordImage.FilePath))
                {
                    // 显示图像到画布
                    curPaintCav.DisplayImageOnCanvas(loadedImage);
                }
            }
        }

        private void StyleRefImage_Click(object? sender, EventArgs e)
        {
            if (sender is CheckableImageItem item)
            {
                for (int i = 0; i < refLayout.Controls.Count; i++)
                {
                    CheckableImageItem it = (CheckableImageItem)refLayout.Controls[i];

                    if (item == it)
                    {
                        it.Selected = !it.Selected;

                        if (it.Selected)
                        {
                            SelectedRefItem = item;
                        }
                        else
                        {
                            SelectedRefItem = null;
                        }
                        continue;
                    }
                    it.Selected = false;
                }
            }
        }

        private void StyleStyleImage_Click(object? sender, EventArgs e)
        {
            if (sender is CheckableImageItem refItem)
            {
                for (int i = 0; i < templateLayout.Controls.Count; i++)
                {
                    CheckableImageItem it = (CheckableImageItem)templateLayout.Controls[i];
                    if (it == refItem)
                    {
                        it.Selected = !it.Selected;
                        if (it.Selected)
                        {
                            SelectStyleItem = refItem;
                        }
                        else
                        {
                            SelectStyleItem = null;
                        }
                        continue;
                    }
                    it.Selected = false;
                }
            }
        }

        private void MainViewTab_SelectedIndexChanged_MaskReset(object? sender, EventArgs e)
        {
            foreach (TabPage tab in MainViewTab.TabPages)
            {
                if (tab.Controls.Count > 0 && tab.Controls[0] is RulerPainting painting)
                {
                    painting.SetMaskMode(false);
                    painting.CurserIconCanChange(false);
                }
            }
        }

        #endregion

        #region 涂抹工具

        private void remove_Click(object sender, EventArgs e)
        {
            if (!btnGen.Enabled)
                return;

            if (curPaintCav != null && curPaintCav.IsCanvasEmpty())
            {
                MessageBox.Show("重绘完成后才能使用区域移除功能。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            operationType = OperationType.RegionRemove;

            // 进入遮罩模式
            curPaintCav?.UnlockCanvas();
            curPaintCav?.SetMaskMode(true);
        }

        #endregion

        #region 右键菜单事件处理
        private void saveMapMenuItem_Click(object sender, EventArgs e)
        {
            if (curPaintCav == null)
                return;
            try
            {
                // 检查画布是否为空
                if (curPaintCav.IsCanvasEmpty())
                {
                    MessageBox.Show("画布没有任何内容，无需保存。", "画布为空", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 保存图片
                string filename = Path.Combine(AppSettings.ArtChangesDirectory, RecordDirName, $"{this.curTab.Text}.png");

                // 获取画布图像
                Bitmap canvasImage = curPaintCav.GetCanvasImage();

                // 保存
                var dir = Path.GetDirectoryName(filename);
                if (!string.IsNullOrEmpty(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                canvasImage.Save(filename, ImageFormat.Png);

                // 释放资源
                canvasImage.Dispose();

                MessageBox.Show(this, $"地图已保存至:\n{filename}", "保存成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"保存地图时出错: {ex.Message}", "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void closeMapMenuItem_Click(object sender, EventArgs e)
        {
            // 如果有未保存的内容，询问用户是否需要保存
            DialogResult result = MessageBox.Show(this, "是否需要保存当前地图？", "关闭确认",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (result == DialogResult.Cancel)
            {
                return; // 用户取消关闭操作
            }

            if (this.MainViewTab.TabCount == 1)
            {
                //关闭当前界面
                this.Close();
            }
            else
            {
                this.MainViewTab.TabPages.RemoveAt(this.MainViewTab.SelectedIndex);
            }
        }

        private void renameMapMenuItem_Click(object sender, EventArgs e)
        {
            // 创建一个输入对话框
            string currentName = this.curTab != null ? this.curTab.Text : string.Empty;

            // 使用InputBox获取用户输入的新名称
            using (Form inputForm = new Form())
            {
                inputForm.Width = 300;
                inputForm.Height = 150;
                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                inputForm.Text = "重命名地图";
                inputForm.StartPosition = FormStartPosition.CenterParent;
                inputForm.MaximizeBox = false;
                inputForm.MinimizeBox = false;

                Label textLabel = new Label() { Left = 20, Top = 20, Text = "请输入新地图名称:" };
                TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 240, Text = currentName };
                Button confirmButton = new Button() { Text = "确定", Left = 110, Width = 80, Top = 80, DialogResult = DialogResult.OK };

                // 为确认按钮添加事件处理
                confirmButton.Click += (s, e) => { inputForm.Close(); };

                inputForm.Controls.Add(textBox);
                inputForm.Controls.Add(confirmButton);
                inputForm.Controls.Add(textLabel);
                inputForm.AcceptButton = confirmButton;

                // 显示窗口并获取结果
                if (inputForm.ShowDialog() == DialogResult.OK)
                {
                    string newName = textBox.Text.Trim();

                    // 确保输入的名称不为空
                    if (!string.IsNullOrEmpty(newName))
                    {
                        // 如果名称已更改，则同时更新所有相关文件名称
                        if (newName != currentName)
                        {
                            // 更新标签页名称
                            if (this.curTab != null)
                            {
                                this.curTab.Text = newName;
                            }

                            // 检查是否需要重命名已存在的文件
                            string oldFilePath = AppSettings.GetTmpDrawPath($"{currentName}.png");
                            string newFilePath = AppSettings.GetTmpDrawPath($"{newName}.png");

                            try
                            {
                                // 如果原文件存在，则重命名文件
                                if (File.Exists(oldFilePath))
                                {
                                    File.Move(oldFilePath, newFilePath, true); // true参数表示如果目标已存在则覆盖
                                }

                                // 检查并重命名可能存在的ComfyUI输出文件
                                string oldOutputPath = AppSettings.GetMapOutputPath($"{currentName}.png");
                                string newOutputPath = AppSettings.GetMapOutputPath($"{newName}.png");

                                if (File.Exists(oldOutputPath))
                                {
                                    File.Move(oldOutputPath, newOutputPath, true);
                                }

                                // 检查并重命名可能存在的遮罩文件
                                string oldMaskPath = AppSettings.GetTmpMaskPath($"{currentName}_mask.png");
                                string newMaskPath = AppSettings.GetTmpMaskPath($"{newName}_mask.png");

                                if (File.Exists(oldMaskPath))
                                {
                                    File.Move(oldMaskPath, newMaskPath, true);
                                }

                                MessageBox.Show(this, $"地图已重命名为: {newName}", "重命名成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (Exception ex)
                            {
                                // 重命名失败时仍然更新UI上的名称，但显示警告
                                MessageBox.Show(this, $"文件重命名过程中出错: {ex.Message}\n但UI已更新显示新名称。",
                                    "部分完成", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show(this, "地图名称不能为空", "重命名失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void deleteImg_Click(object sender, EventArgs e)
        {
            if (imgMenu.SourceControl is IconItemControl_V itemV)
            {
                // 删除图片 itemV.FilePath
                if (MessageBox.Show(this, $"确定要删除：{itemV.FilePath}?", "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // 删除文件
                    File.Delete(itemV.FilePath);

                    // 删除控件
                    itemV.Dispose();
                }
            }
        }

        private void openImg_Click(object sender, EventArgs e)
        {
            if (imgMenu.SourceControl is IconItemControl_V itemV)
            {
                var newTab = Utility.NewPaintingTab(ref MainViewTab);
                newTab.Text = Path.GetFileNameWithoutExtension(itemV.FilePath);

                //加载图像到画布
                using (Bitmap loadedImage = new Bitmap(itemV.FilePath))
                {
                    curPaintCav?.DisplayImageOnCanvas(loadedImage);
                }
            }
        }

        #endregion

        #region AIGC
        private async void StopGen()
        {
            await _comfyUIClient.CancelCurrentExecution();
            MessageBox.Show(this, "已取消");
            _progressForm.Hide();
        }

        private async void RquestGen()
        {
            if (curPaintCav == null)
            { return; }


            // 禁用按钮，防止用户重复点击
            btnGen.Enabled = false;
            btnGen.Text = "生成中...";

            // linq获取rawImgLayout中选中元素的FilePath
            string? selectedRawImagePath = selectRawImgItem?.FilePath;

            if (string.IsNullOrEmpty(selectedRawImagePath))
                return;

            selectedRawImagePath = selectedRawImagePath.Replace("preload\\", "project_raw\\");
            string outputFile = this.curTab != null ? AppSettings.GetMapOutputPath($"{this.curTab.Text}.png") : string.Empty;


            string selectLoraNameKey = SelectStyleItem == null? string.Empty : Path.GetFileNameWithoutExtension(SelectStyleItem.FilePath);

            string selectRefPath = SelectedRefItem == null ? string.Empty : SelectedRefItem.FilePath;

            _progressForm.Show(this);

            int batchSize = string.IsNullOrEmpty(this.genCnt.Text) ? 1 : int.Parse(this.genCnt.Text);

            float denoise = _denoise;

            // 使用DrawToImgProcessor处理图像
            var resultImagePath = await _changeFluxStyleProcessor.Process(selectedRawImagePath, selectRefPath, batchSize, SelectedStyleKey,denoise, _progressForm);


            if (!string.IsNullOrEmpty(resultImagePath))
            {
                // 显示在画布上
                try
                {
                    using (Bitmap generatedImage = new Bitmap(resultImagePath))
                    {
                        // 显示生成的图像在当前画布上
                        curPaintCav.DisplayImageOnCanvas(generatedImage);

                        generatedImage.Save(outputFile, ImageFormat.Png);
                    }
                    _progressForm.SetProgress(100, "生成完毕....");

                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"无法显示生成的图像: {ex.Message}", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                finally
                {
                    ResetControl();
                }
            }
            else
            {
                ResetControl();
            }
        }

        private async void RequestRemoval()
        {
            if (curPaintCav == null)
                return;

            btnGen.Enabled = false;
            btnGen.Text = "移除中...";

            //打开假进度条
            _progressForm.Show(this);

            Directory.CreateDirectory(AppSettings.TempMaskDirectory);

            string outputFile = string.Empty;
            if (this.curTab != null)
            {
                outputFile = AppSettings.GetMapOutputPath($"{this.curTab.Text}.png");
            }
            else
            {
                MessageBox.Show("未找到当前标签页，无法保存输出文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetControl();
                return;
            }

            if (!Path.Exists(outputFile))
            {
                Bitmap canvasImage = curPaintCav.GetCanvasImage();
                canvasImage.Save(outputFile, ImageFormat.Png);
            }

            string maskPath = Path.Combine(AppSettings.TempMaskDirectory, $"remove_mask_{this.curTab.Text}.png");
            using (var maskBmp = curPaintCav.GetMaskImage())
            {
                maskBmp.Save(maskPath, System.Drawing.Imaging.ImageFormat.Png);
            }

            curPaintCav.SetMaskMode(false);


            string resultImagePath = "";
            // 3. 启动实际处理
            await _removeProcessor.Process(outputFile, maskPath, _progressForm);
            resultImagePath = await _comfyUIClient.PollForResult(_removeProcessor.LastPromptId, _removeProcessor.LastNodeId);
            if (string.IsNullOrEmpty(resultImagePath))
            {
                MessageBox.Show("无法获取生成的图片");
            }

            // 5. 处理最终结果
            if (!string.IsNullOrEmpty(resultImagePath))
            {
                try
                {
                    using (var generatedImage = new Bitmap(resultImagePath))
                    {
                        curPaintCav.DisplayImageOnCanvas(generatedImage);
                        generatedImage.Save(outputFile, ImageFormat.Png);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"无法显示生成的图像: {ex.Message}", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                finally
                {
                    ResetControl();
                    curPaintCav?.SetMaskMode(false);
                }
            }
            else
            {
                ResetControl();
            }
        }

        #endregion

        #region 窗体关闭事件处理
        private void StyleChangeWnd_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 检查是否有打开的标签页
            if (MainViewTab.TabCount > 0)
            {
                // 询问用户是否需要保存所有标签页
                DialogResult result = MessageBox.Show(this, "是否保存所有已打开的标签页？",
                    "关闭确认", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Cancel)
                {
                    // 用户取消关闭操作
                    e.Cancel = true;
                    return;
                }
                else if (result == DialogResult.Yes)
                {
                    // 保存所有打开的标签页
                    SaveAllTabs();
                }
            }
        }

        private void SaveAllTabs()
        {
            try
            {
                // 遍历所有标签页
                for (int i = 0; i < MainViewTab.TabCount; i++)
                {
                    TabPage tab = MainViewTab.TabPages[i];
                    if (tab.Controls.Count > 0 && tab.Controls[0] is RulerPainting canvas)
                    {
                        // 检查画布是否为空
                        if (!canvas.IsCanvasEmpty())
                        {
                            // 获取标签页名称作为文件名
                            string mapName = tab.Text;

                            // 保存图片
                            string filename = Path.Combine(AppSettings.ArtChangesDirectory, RecordDirName, $"{mapName}.png");

                            // 获取画布图像
                            Bitmap canvasImage = canvas.GetCanvasImage();

                            // 确保目录存在
                            var dir = Path.GetDirectoryName(filename);
                            if (!string.IsNullOrEmpty(dir))
                            {
                                Directory.CreateDirectory(dir);
                            }

                            // 保存
                            canvasImage.Save(filename, ImageFormat.Png);

                            // 释放资源
                            canvasImage.Dispose();
                        }
                    }
                }

                MessageBox.Show(this, "所有标签页已保存", "保存成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"保存标签页时出错: {ex.Message}", "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}
