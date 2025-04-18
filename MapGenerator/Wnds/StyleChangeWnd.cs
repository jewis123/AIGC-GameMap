using System.Drawing.Imaging;
using MapGenerator.Components;
using MapGenerator.Request.ComfyUI;
using MapGenerator.Utils;

namespace MapGenerator.Wnds
{
    public partial class StyleChangeWnd : Form
    {
        private ComfyUIClient _comfyUIClient;
        private StyleChangeProcessor _changeStyleProcessor;
        private int selectRawImgIdx;
        private RulerPainting? curPaintCav => MainViewTab.SelectedTab.Controls[0] as RulerPainting;
        private TabPage? curTab => MainViewTab.SelectedTab;

        internal enum OperationType
        {
            StyleChange = 0,
            RegionRepaint = 1,
            RegionRemove = 2,
            RegionPick = 3
        }
        public string RecordDirName { get; set; }
        private OperationType operationType = OperationType.StyleChange;

        private int SelectedRefIdx { get; set; } = -1;

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

            // 添加窗体关闭事件
            this.FormClosing += StyleChangeWnd_FormClosing;

            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void Enter_Load(object sender, EventArgs e)
        {
            InitializeTemplateListView();
            InitializeReferenceListView();
            InitializeProjectRawListView();
            InitializeChangedListView();

            //只有需要涂抹区域时才解锁
            if (curPaintCav != null)
            {
                curPaintCav.LockCanvas();
            }


            this.tabs.SelectedIndexChanged += tabs_SelectedIndexChanged;
            this.PromptBox.Hint = "输入提示词(可不填)，对生图有20%影响力....";

            genSize.SetLabel("生成尺寸");
        }

        private void tabs_SelectedIndexChanged(object? sender, EventArgs e)
        {
            switch (this.tabs.SelectedIndex)
            {
                case 0:
                    InitializeReferenceListView();
                    break;
                case 1:
                    curPaintCav?.UnlockCanvas();
                    break;
                default:
                    break;
            }
        }

        private void InitProcessors()
        {
            _comfyUIClient = new ComfyUIClient();

            _changeStyleProcessor = new StyleChangeProcessor(ref _comfyUIClient);
        }

        private void InitializeProjectRawListView()
        {
            Utility.LoadIconItemControl(true,
            ((List<string>)[.. Utility.GetImagePathsFromFolder(AppSettings.ProjectRawImgPath)]).ToArray(), OnClickRawImage, ref rawImageLayout, [120, 120]);

        }

        private void InitializeReferenceListView()
        {
            List<string> imagePaths = [.. Utility.GetImagePathsFromFolder(AppSettings.RefImagePath)];
            Utility.LoadCheckableImageToFlowLayout(imagePaths.ToArray(), StyleRefImage_Click, ref refLayout);
            if (SelectedRefIdx != -1)
            {
                (refLayout.Controls[SelectedRefIdx] as CheckableImageItem).Selected = true;
            }
        }

        private void InitializeTemplateListView()
        {
            List<string> imagePaths = [.. Utility.GetImagePathsFromFolder(AppSettings.StyleTemplatePath)];
            Utility.LoadCheckableImageToFlowLayout(imagePaths.ToArray(), StyleStyleImage_Click, ref templateLayout);

            (templateLayout.Controls[0] as CheckableImageItem).Selected = true;
        }

        private void InitializeChangedListView()
        {
            Utility.LoadIconItemControl(true, ((List<string>)[.. Utility.GetImagePathsFromFolder(Path.Combine(AppSettings.ArtChangesDirectory, RecordDirName))]).ToArray(), OnClickRecordImage, ref recordsLayout, [120, 120]);
        }

        private void ResetControl()
        {
            // 恢复按钮状态
            btnGen.Enabled = true;
            btnGen.Text = "生成";
        }

        #region Event
        private void btnGen_Click(object sender, EventArgs e)
        {
            switch (operationType)
            {
                case OperationType.StyleChange:
                    RquestGen();
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
                rulerPainting.UnlockCanvas();
                rulerPainting.SetDecoratorMode(false);

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
                // 加载图像
                using (Bitmap loadedImage = new Bitmap(rawImage.FilePath))
                {
                    // 更新尺寸设置控件的值
                    genSize.SetSize(loadedImage.Width, loadedImage.Height);

                    // 显示图像到画布

                    curPaintCav.DisplayImageOnCanvas(loadedImage);

                    this.curTab.Text = rawImage.GetImgName();

                    selectRawImgIdx = rawImage.Idx;
                }
            }
        }

        private void OnClickRecordImage(object? sender, EventArgs e)
        {
            if (sender is IconItemControl_V recordImage)
            {
                //MainViewTab新增一个tab
                TabPage newTabPage = new TabPage();
                newTabPage.AutoScroll = true;
                RulerPainting rulerPainting = new RulerPainting();
                rulerPainting.AutoScroll = true;
                PaintCanvas canvas = new PaintCanvas();
                rulerPainting.Controls.Add(canvas);
                newTabPage.Controls.Add(rulerPainting);
                newTabPage.Text = Path.GetFileNameWithoutExtension(recordImage.FilePath);
                MainViewTab.TabPages.Add(newTabPage);


                MainViewTab.SelectedTab = newTabPage;

                // 加载图像
                using (Bitmap loadedImage = new Bitmap(recordImage.FilePath))
                {
                    // 更新尺寸设置控件的值
                    genSize.SetSize(loadedImage.Width, loadedImage.Height);

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
                        if (it.Selected)
                        {
                            SelectedRefIdx = -1;
                        }
                        else
                        {
                            SelectedRefIdx = i;
                        }

                        it.Selected = !it.Selected;
                    }
                    else
                    {
                        it.Selected = false;
                    }
                }
            }
        }

        private void StyleStyleImage_Click(object? sender, EventArgs e)
        {
            if (sender is CheckableImageItem refItem)
            {
                foreach (var item in templateLayout.Controls)
                {
                    CheckableImageItem it = (CheckableImageItem)item;
                    if (it == refItem && it.Selected)
                        continue;
                    it.Selected = false;
                }

                refItem.Selected = !refItem.Selected;
            }
        }


        #endregion

        #region 涂抹工具

        private void pick_Click(object sender, EventArgs e)
        {
            if (!btnGen.Enabled)
                return;

            operationType = OperationType.RegionPick;
        }

        private void remove_Click(object sender, EventArgs e)
        {
            if (!btnGen.Enabled)
                return;

            operationType = OperationType.RegionRemove;
        }

        private void repaint_Click(object sender, EventArgs e)
        {
            if (!btnGen.Enabled)
                return;


            operationType = OperationType.RegionRepaint;
        }

        private void resume_Click(object sender, EventArgs e)
        {
            if (!btnGen.Enabled)
                return;

            operationType = OperationType.StyleChange;
            //退出mask涂抹模式
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
            string currentName = this.curTab.Text;

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
                            this.curTab.Text = newName;

                            // 检查是否需要重命名已存在的文件
                            string oldFilePath = AppSettings.GetMapDrawOutputPath($"{currentName}.png");
                            string newFilePath = AppSettings.GetMapDrawOutputPath($"{newName}.png");

                            try
                            {
                                // 如果原文件存在，则重命名文件
                                if (File.Exists(oldFilePath))
                                {
                                    File.Move(oldFilePath, newFilePath, true); // true参数表示如果目标已存在则覆盖
                                }

                                // 检查并重命名可能存在的ComfyUI输出文件
                                string oldOutputPath = AppSettings.GetComfyUIOutputPath($"{currentName}.png");
                                string newOutputPath = AppSettings.GetComfyUIOutputPath($"{newName}.png");

                                if (File.Exists(oldOutputPath))
                                {
                                    File.Move(oldOutputPath, newOutputPath, true);
                                }

                                // 检查并重命名可能存在的遮罩文件
                                string oldMaskPath = AppSettings.GetMaskDrawOutputPath($"{currentName}_mask.png");
                                string newMaskPath = AppSettings.GetMaskDrawOutputPath($"{newName}_mask.png");

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

        #endregion

        #region AIGC
        private async void StopGen()
        {
            await _comfyUIClient.CancelCurrentExecution();
            MessageBox.Show(this, "已取消");
        }

        private async void RquestGen()
        {
            if (curPaintCav == null)
            { return; }


            // 禁用按钮，防止用户重复点击
            btnGen.Enabled = false;
            btnGen.Text = "生成中...";

            // linq获取rawImgLayout中选中元素的FilePath
            string? selectedRawImagePath = rawImageLayout.Controls.OfType<IconItemControl_V>()
                .FirstOrDefault(x => x.Idx == selectRawImgIdx)?.FilePath;

            if (string.IsNullOrEmpty(selectedRawImagePath))
                return;

            // 获取画布图像
            Bitmap canvasImage = curPaintCav.GetCanvasImage();

            // 使用DrawToImgProcessor处理图像
            int batchSize = string.IsNullOrEmpty(this.genCnt.Text) ? 1 : int.Parse(this.genCnt.Text);
            string selectPath = string.Empty;
            if (SelectedRefIdx != -1)
            {
                for (int i = 0; i < refLayout.Controls.Count; i++)
                {
                    if (refLayout.Controls[i] is CheckableImageItem item && i == SelectedRefIdx)
                    {
                        selectPath = item.FilePath;
                        break;
                    }
                }
            }

            var resultImagePath = await _changeStyleProcessor.Process(PromptBox.Text, selectedRawImagePath, selectPath, batchSize, this.genSize.PixelSize, SelectedStyleKey);

            if (!string.IsNullOrEmpty(resultImagePath))
            {
                // 显示在画布上
                try
                {
                    using (Bitmap generatedImage = new Bitmap(resultImagePath))
                    {
                        // 显示生成的图像在当前画布上
                        curPaintCav.DisplayImageOnCanvas(generatedImage);
                    }
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

            // 释放资源
            canvasImage.Dispose();
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
                            Directory.CreateDirectory(Path.GetDirectoryName(filename));

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
