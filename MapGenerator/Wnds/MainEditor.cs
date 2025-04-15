using MapGenerator.Components;
using MapGenerator.Enums;
using MapGenerator.Request.ComfyUI;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MapGenerator.Wnds
{
    public partial class MainEditor : Form
    {
        private bool isRepaint = false;
        private ComfyUIClient _comfyUIClient;
        private DrawToImgProcessor _drawToImgProcessor;

        public MainEditor()
        {
            InitializeComponent();

            _comfyUIClient = new ComfyUIClient();

            // 初始化ComfyUI相关组件
            _drawToImgProcessor = new DrawToImgProcessor(ref _comfyUIClient);

            InitView();

            this.FormClosing += new FormClosingEventHandler(MyForm_FormClosing);
            this.toolTab.SelectedIndexChanged += onToolTabChanged;

            // 添加mapStyle标签页切换事件处理
            this.mapStyle.SelectedIndexChanged += mapStyle_SelectedIndexChanged;

            // 添加装饰模式退出事件处理
            this.rulerPainting.DecoratorModeExited += RulerPainting_DecoratorModeExited;
        }

        internal void SetMapIndex(int selectMapxIdx)
        {

        }

        private void MyForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        void InitView()
        {
            //填充笔刷tab
            BrushListInit();

            // 初始化参考图显示 - 默认加载全部参考图
            RefImgListInit(0);
        }

        #region 笔刷

        private void BrushListInit()
        {
            // 创建 FlowLayoutPanel 用于显示笔刷列表
            this.brushList.FlowDirection = FlowDirection.LeftToRight;
            this.brushList.WrapContents = true;
            this.brushList.AutoScroll = true;

            // 清空之前的笔刷控件
            this.brushList.Controls.Clear();

            for (int i = 0; i < AppSettings.BrushCnt; i++)
            {
                MapBrush brush = AppSettings.GetBrush(i);
                // 创建自定义笔刷控件实例
                IconItemControl brushItem = new IconItemControl();
                brushItem.SetBrushType(brush.Color, brush.Name, i);

                brushItem.Click += BrushItemSelected;
                // 将笔刷控件添加到 FlowLayoutPanel 中
                this.brushList.Controls.Add(brushItem);

                if (i == 0)
                {
                    brushItem.SetBackground(true);
                }
            }
        }

        private void BrushItemSelected(object? sender, EventArgs e)
        {
            if (sender is IconItemControl brushItem)
            {
                this.rulerPainting.SetBrush(brushItem.Idx);

                foreach (var item in this.brushList.Controls)
                {
                    if (item is IconItemControl brushitem)
                    {
                        brushitem.SetBackground(brushitem.Idx == brushItem.Idx);
                    }
                }
            }
        }

        private void InitDecoratorList()
        {
            // 清空装饰列表
            this.decoratorList.Controls.Clear();

            // 设置FlowLayoutPanel的属性
            this.decoratorList.FlowDirection = FlowDirection.LeftToRight;
            this.decoratorList.WrapContents = true;
            this.decoratorList.AutoScroll = true;

            try
            {
                // 获取装饰图片文件夹路径
                string decoratorPath = Path.Combine(AppSettings.AssetsDirectory, "reference", "decorator");

                // 检查目录是否存在
                if (Directory.Exists(decoratorPath))
                {
                    // 获取所有图片文件
                    string[] imageFiles = Directory.GetFiles(decoratorPath, "*.png")
                        .Concat(Directory.GetFiles(decoratorPath, "*.jpg"))
                        .Concat(Directory.GetFiles(decoratorPath, "*.jpeg")).ToArray();

                    // 记录当前装饰图片的索引
                    int decoratorIndex = 0;

                    foreach (string imagePath in imageFiles)
                    {
                        try
                        {
                            // 获取文件名（不含扩展名）
                            string fileName = Path.GetFileNameWithoutExtension(imagePath);

                            IconItemControl decoratorItem = new IconItemControl();
                            // 直接将图片传递给控件而不保存为临时文件
                            decoratorItem.SetImg(imagePath, fileName, decoratorIndex);

                            // 添加点击事件
                            decoratorItem.Click += DecoratorItemSelected;

                            // 将控件添加到列表
                            this.decoratorList.Controls.Add(decoratorItem);

                            // 递增索引
                            decoratorIndex++;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"加载图片时出错: {ex.Message}");
                            MessageBox.Show($"加载图片 {Path.GetFileName(imagePath)} 时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            // 继续处理下一个图片
                        }
                    }

                    // 如果没有找到任何图片
                    if (decoratorIndex == 0)
                    {
                        MessageBox.Show($"在目录 {decoratorPath} 中没有找到任何图片文件。\n请确保有PNG、JPG或JPEG格式的图片。",
                            "没有找到图片", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show($"装饰图片文件夹不存在: {decoratorPath}", "路径错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"初始化装饰列表时出错: {ex.Message}");
                MessageBox.Show($"加载装饰图片时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DecoratorItemSelected(object? sender, EventArgs e)
        {
            if (sender is IconItemControl decoratorItem)
            {
                // 设置选中状态
                foreach (var item in this.decoratorList.Controls)
                {
                    if (item is IconItemControl ctrl)
                    {
                        ctrl.SetBackground(ctrl.Idx == decoratorItem.Idx);
                    }
                }

                // 获取当前选中的装饰图像
                Image? decoratorImage = decoratorItem.GetImage();
                string decoratorName = decoratorItem.GetImgName();

                if (decoratorImage != null)
                {
                    // 启用装饰模式，将选中的装饰图传递给rulerPainting
                    rulerPainting.SetDecoratorMode(true, decoratorImage, decoratorName);

                    // 通知用户装饰模式已激活
                    ShowStatusMessage($"装饰模式已激活：{decoratorName}。使用鼠标滚轮调整大小，点击添加装饰。【单次只能融入一种装饰物】");

                    this.toolTab.Enabled = false;
                }
            }
        }
        
        // 处理装饰模式退出事件
        private void RulerPainting_DecoratorModeExited(object? sender, EventArgs e)
        {
            // 重置工具栏状态
            this.toolTab.Enabled = true;

            // 重置装饰列表选中状态
            foreach (var item in this.decoratorList.Controls)
            {
                if (item is IconItemControl ctrl)
                {
                    ctrl.SetBackground(false);
                }
            }
        }
    
        #endregion

        // 在UI上显示状态消息的辅助方法
        private void ShowStatusMessage(string message, bool isError = false)
        {
            // 这里可以实现状态栏消息或临时显示提示
            // 临时使用MessageBox作为状态提示
            MessageBox.Show(message, "装饰模式", MessageBoxButtons.OK,
                isError ? MessageBoxIcon.Error : MessageBoxIcon.Information);
        }

        private void ResetControl()
        {
            // 解锁画布，允许用户继续绘制
            rulerPainting.UnlockCanvas();
            rulerPainting.SetDecoratorMode(false);

            // 恢复按钮状态
            btnGen.Enabled = true;
            btnGen.Text = "生成";
            this.toolTab.Enabled = true;
        }

        #region Events

        private void onToolTabChanged(object? sender, EventArgs e)
        {
            if (this.toolTab.SelectedIndex == 1)
            {
                InitDecoratorList();
            }
        }

        private void MainEditor_Load(object sender, EventArgs e)
        {
            this.CurMap.Text = "New UnTitled";
        }

        private void paintSize_Load(object sender, EventArgs e)
        {
            this.paintSize.SetLabel("画板像素");
            this.paintSize.OnSizeChanged += OnPaintingSizeChanged;
        }

        private void genSize_Load(object sender, EventArgs e)
        {
            this.genSize.SetLabel("生成像素");
        }

        private void OnPaintingSizeChanged(object? sender, SizeSetting.SizeSettingEventArgs e)
        {
            this.rulerPainting.SetDrawingSize(e.iWidthVal, e.iHeightVal);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                StopGen();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"中止请求出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void btnGen_Click(object sender, EventArgs e)
        {
            try
            {
                if (isRepaint)
                {
                    RequestRepaint();
                    return;
                }

                if (this.rulerPainting.IsDecorateMode)
                {
                    RequestDecoratorInPaint();
                    return;
                }

                if (!rulerPainting.IsCanvasEmpty())
                {
                    if (curRefImgIdx == -1)
                        RequestGen();
                    else
                    {
                        RequestIPSegGen();
                    }
                }
                else
                {
                    if (curRefImgIdx != -1)
                    {
                        RequestIPGen();
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show($"处理图像时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ResetControl();
            }
        }

        #endregion

        #region 右键菜单事件处理

        private void saveMapMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // 检查画布是否为空
                if (rulerPainting.IsCanvasEmpty())
                {
                    MessageBox.Show("画布没有任何内容，无需保存。", "画布为空", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 使用与btnGen_Click相同的保存逻辑
                string filename = AppSettings.GetMapDrawOutputPath($"{this.CurMap.Text}.png");

                // 获取画布图像
                Bitmap canvasImage = rulerPainting.GetCanvasImage();

                // 保存
                canvasImage.Save(filename, ImageFormat.Png);

                // 释放资源
                canvasImage.Dispose();

                MessageBox.Show($"地图已保存至:\n{filename}", "保存成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存地图时出错: {ex.Message}", "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void clearMapMenuItem_Click(object sender, EventArgs e)
        {
            // 询问用户是否确定要清空地图
            DialogResult result = MessageBox.Show("确定要清空当前地图吗？此操作不可撤销。", "确认清空",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // 调用PaintCanvas的清空方法
                rulerPainting.ClearCanvas();

                // 清空描述文本
                this.PromptBox.Text = "";

                MessageBox.Show("地图已清空", "操作完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void closeMapMenuItem_Click(object sender, EventArgs e)
        {
            // 如果有未保存的内容，询问用户是否需要保存
            DialogResult result = MessageBox.Show("是否需要保存当前地图？", "关闭确认",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (result == DialogResult.Cancel)
            {
                return; // 用户取消关闭操作
            }

            if (result == DialogResult.Yes)
            {
                // 保存当前地图
                saveMapMenuItem_Click(sender, e);
            }

            // 关闭当前页或返回到开始界面
            // 作为示例，这里仅重置内容
            rulerPainting.ClearCanvas();
            this.PromptBox.Text = "";
            this.CurMap.Text = "New Untitled";
        }

        private void partialRedrawMenuItem_Click(object sender, EventArgs e)
        {
            isRepaint = true;

            // 启用遮罩模式以标记要重绘的区域
            rulerPainting.SetMaskMode(true);

            // 更新按钮文本以指示当前的动作
            btnGen.Text = "重绘";

            // 显示说明信息
            MessageBox.Show("请在画布上涂抹需要重绘的区域，然后点击'重绘'按钮。", "局部重绘模式",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void loadDrawingMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // 创建文件选择对话框
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    // 设置对话框属性
                    openFileDialog.InitialDirectory = AppSettings.TempDrawDirectory;
                    openFileDialog.Filter = "PNG图像 (*.png)|*.png|所有文件 (*.*)|*.*";
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.Title = "选择要加载的涂鸦";

                    // 显示对话框并获取结果
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // 获取选择的文件路径
                        string selectedFilePath = openFileDialog.FileName;

                        // 获取文件名（不含扩展名）作为新的标签页名称
                        string fileName = Path.GetFileNameWithoutExtension(selectedFilePath);
                        this.CurMap.Text = fileName;

                        // 加载图像
                        using (Bitmap loadedImage = new Bitmap(selectedFilePath))
                        {
                            // 设置画布尺寸为图像尺寸
                            rulerPainting.SetDrawingSize(loadedImage.Width, loadedImage.Height);

                            // 更新尺寸设置控件的值
                            paintSize.GetSize(out int _, out int _); // 忽略当前值

                            // 显示图像到画布
                            rulerPainting.DisplayImageOnCanvas(loadedImage);

                            MessageBox.Show($"成功加载涂鸦：{fileName}", "加载成功",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载涂鸦时出错: {ex.Message}", "加载失败",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void renameMapMenuItem_Click(object sender, EventArgs e)
        {
            // 创建一个输入对话框
            string currentName = this.CurMap.Text;

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
                            this.CurMap.Text = newName;

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

                                MessageBox.Show($"地图已重命名为: {newName}", "重命名成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (Exception ex)
                            {
                                // 重命名失败时仍然更新UI上的名称，但显示警告
                                MessageBox.Show($"文件重命名过程中出错: {ex.Message}\n但UI已更新显示新名称。",
                                    "部分完成", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("地图名称不能为空", "重命名失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        #endregion

        #region 参考图列表
        private int curRefImgIdx = -1;

        // 处理mapStyle切换事件
        private void mapStyle_SelectedIndexChanged(object? sender, EventArgs e)
        {
            RefImgListInit(mapStyle.SelectedIndex);
        }

        // 根据选择的标签页加载不同位置的参考图片
        private void RefImgListInit(int selectedIndex)
        {
            // 清空当前的参考图片
            mapRefLayout.Controls.Clear();

            // 设置FlowLayoutPanel的属性
            mapRefLayout.FlowDirection = FlowDirection.LeftToRight;
            mapRefLayout.WrapContents = true;
            mapRefLayout.AutoScroll = true;

            try
            {
                List<string> imagePaths = new List<string>();

                // 根据所选标签，确定要加载的文件夹路径
                switch (selectedIndex)
                {
                    case 0: // allRefTab - 加载所有参考图
                        imagePaths.AddRange(GetImagePathsFromFolder(Path.Combine(AppSettings.AssetRefPath, "ref")));
                        imagePaths.AddRange(GetImagePathsFromFolder(Path.Combine(AppSettings.AssetRefPath, "template")));
                        break;
                    case 1: // refTab - 只加载ref文件夹中的图片
                        imagePaths.AddRange(GetImagePathsFromFolder(Path.Combine(AppSettings.AssetRefPath, "ref")));
                        break;
                    case 2: // templateRefTab - 只加载template文件夹中的图片
                        imagePaths.AddRange(GetImagePathsFromFolder(Path.Combine(AppSettings.AssetRefPath, "template")));
                        break;
                }

                // 如果没有找到图片，显示提示信息
                if (imagePaths.Count == 0)
                {
                    Label noImageLabel = new Label
                    {
                        Text = "没有找到参考图片",
                        AutoSize = true,
                        Font = new Font(Font.FontFamily, 10)
                    };
                    mapRefLayout.Controls.Add(noImageLabel);
                }
                else
                {
                    // 加载并显示找到的图片
                    int imageIndex = 0;
                    foreach (string imagePath in imagePaths)
                    {
                        try
                        {
                            // 创建PictureBox控件来显示图片
                            RefImageItem refItem = new RefImageItem();
                            refItem.BackgroundImage = Image.FromFile(imagePath);
                            refItem.FilePath = imagePath;

                            // 添加点击事件，方便用户点击查看大图
                            refItem.Click += ReferenceImage_Click;
                            refItem.DoubleClick += ReferenceImage_DBClick;
                            refItem.Index = imageIndex;

                            // 将图片添加到布局中
                            mapRefLayout.Controls.Add(refItem);
                            imageIndex++;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"加载参考图片时出错: {ex.Message}");
                            // 继续处理下一个图片，不中断流程
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载参考图片时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReferenceImage_Click(object? sender, EventArgs e)
        {
            if (sender is RefImageItem refItem)
            {
                if (curRefImgIdx == refItem.Index)
                {
                    curRefImgIdx = -1;
                    refItem.SetCheckBox(false);
                    return;
                }

                curRefImgIdx = refItem.Index;
                foreach (var item in mapRefLayout.Controls)
                {
                    RefImageItem it = (RefImageItem)item;
                    it.SetCheckBox(refItem.Index == it.Index);
                }
            }

        }

        // 双击参考图片事件处理
        private void ReferenceImage_DBClick(object? sender, EventArgs e)
        {
            if (sender is RefImageItem refItem && refItem.FilePath is string imagePath)
            {
                // 创建一个新窗口来显示大图
                Form imageViewerForm = new Form
                {
                    Text = Path.GetFileNameWithoutExtension(imagePath),
                    Size = new Size(800, 600),
                    StartPosition = FormStartPosition.CenterParent
                };

                // 创建一个大的PictureBox来显示图片
                PictureBox largeImageBox = new PictureBox
                {
                    Dock = DockStyle.Fill,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Image = Image.FromFile(imagePath)
                };

                // 添加关闭按钮
                Button closeButton = new Button
                {
                    Text = "关闭",
                    Dock = DockStyle.Bottom,
                    DialogResult = DialogResult.OK
                };

                closeButton.Click += (s, args) => imageViewerForm.Close();

                // 将控件添加到窗口
                imageViewerForm.Controls.Add(largeImageBox);
                imageViewerForm.Controls.Add(closeButton);

                // 显示窗口
                imageViewerForm.ShowDialog();
            }
        }

        // 获取指定文件夹中所有图片的路径
        private List<string> GetImagePathsFromFolder(string folderPath)
        {
            List<string> imagePaths = new List<string>();

            if (Directory.Exists(folderPath))
            {
                // 获取支持的图片文件
                imagePaths.AddRange(Directory.GetFiles(folderPath, "*.png"));
                imagePaths.AddRange(Directory.GetFiles(folderPath, "*.jpg"));
                imagePaths.AddRange(Directory.GetFiles(folderPath, "*.jpeg"));
                imagePaths.AddRange(Directory.GetFiles(folderPath, "*.bmp"));
            }

            return imagePaths;
        }
        #endregion

        #region AIGC请求
        private async void StopGen()
        {
            await _comfyUIClient.CancelCurrentExecution();
            ShowStatusMessage("已取消", true);
        }

        private async void RequestRepaint()
        {
            // 获取绘图区域
            Bitmap canvasImage = rulerPainting.GetCanvasImage();
            // 获取涂抹的遮罩
            Bitmap maskImage = rulerPainting.GetMaskImage();

            // 确保有有效遮罩
            if (canvasImage == null || maskImage == null)
            {
                MessageBox.Show("无法获取绘图或遮罩图像，请确保已选择要重绘的区域。", "操作失败",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 获取提示文本
            string promptText = this.PromptBox.Text.Trim();
            if (string.IsNullOrEmpty(promptText))
            {
                DialogResult result = MessageBox.Show("你没有提供任何提示词。是否要使用空提示词继续？",
                    "提示词为空", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    return;
                }
            }

            string canvasPath = AppSettings.GetComfyUIOutputPath($"{this.CurMap.Text}.png");
            string maskPath = AppSettings.GetMaskDrawOutputPath($"{this.CurMap.Text}_mask.png");

            maskImage.Save(maskPath, System.Drawing.Imaging.ImageFormat.Png);

            // 更新界面状态
            this.btnGen.Enabled = false;
            this.btnGen.Text = "处理中...";

            // 锁定画布，防止用户在重绘过程中继续绘制
            rulerPainting.LockCanvas();

            // 使用异步方法处理生成过程
            InpaintProcessor inpaintProcessor = new InpaintProcessor(ref _comfyUIClient);
            string resultImagePath = await inpaintProcessor.ProcessInpainting(promptText, canvasPath, maskPath);

            if (!string.IsNullOrEmpty(resultImagePath))
            {
                try
                {
                    // 加载生成的图像
                    using (Bitmap resultImage = new Bitmap(resultImagePath))
                    {
                        // 显示在画布上
                        rulerPainting.DisplayImageOnCanvas(resultImage);

                        resultImage.Save(canvasPath, ImageFormat.Png);

                        // 清除遮罩模式
                        rulerPainting.SetMaskMode(false);

                        // 更新状态
                        isRepaint = false;
                        btnGen.Text = "生成";
                        MessageBox.Show("局部重绘完成！", "操作成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"显示结果图像时出错: {ex.Message}", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    ResetControl();
                }
            }
            else
            {
                MessageBox.Show("生成图像失败，请重试。", "操作失败",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void RequestGen()
        {
            // 禁用按钮，防止用户重复点击
            btnGen.Enabled = false;
            btnGen.Text = "生成中...";

            // 锁定画布，防止用户在生成过程中继续绘制
            rulerPainting.LockCanvas();

            // 使用AppSettings中定义的路径获取文件名
            string filename = AppSettings.GetMapDrawOutputPath($"{this.CurMap.Text}.png");
            string outputFile = AppSettings.GetComfyUIOutputPath($"{this.CurMap.Text}.png");

            // 获取画布图像
            Bitmap canvasImage = rulerPainting.GetCanvasImage();

            // 保存为PNG
            canvasImage.Save(filename, ImageFormat.Png);

            // 使用DrawToImgProcessor处理图像
            var resultImagePath = await _drawToImgProcessor.ProcessDrawToImage(this.PromptBox.Text, filename, this.genSize.PixelSize);

            if (!string.IsNullOrEmpty(resultImagePath))
            {
                // 显示在画布上
                try
                {
                    this.genSize.GetSize(out var width, out var height);
                    rulerPainting.SetDrawingSize(width, height);
                    using (Bitmap generatedImage = new Bitmap(resultImagePath))
                    {
                        // 显示生成的图像在当前画布上
                        rulerPainting.DisplayImageOnCanvas(generatedImage);
                        generatedImage.Save(outputFile, ImageFormat.Png);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"无法显示生成的图像: {ex.Message}", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                finally
                {
                    ResetControl();
                }
            }

            // 释放资源
            canvasImage.Dispose();
        }

        private async void RequestIPSegGen()
        {
            // 禁用按钮，防止用户重复点击
            btnGen.Enabled = false;
            btnGen.Text = "生成中...";

            // 锁定画布，防止用户在生成过程中继续绘制
            rulerPainting.LockCanvas();

            // 获取当前选中的参考图
            RefImageItem? selectedRefImage = null;
            foreach (var item in mapRefLayout.Controls)
            {
                if (item is RefImageItem refItem && refItem.Index == curRefImgIdx)
                {
                    selectedRefImage = refItem;
                    break;
                }
            }

            if (selectedRefImage == null || string.IsNullOrEmpty(selectedRefImage.FilePath))
            {
                MessageBox.Show("请选择一个参考图片", "参考图不存在", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnGen.Enabled = true;
                btnGen.Text = "生成";
                rulerPainting.UnlockCanvas(); // 解锁画布
                return;
            }

            // 使用AppSettings中定义的路径获取文件名
            string filename = AppSettings.GetMapDrawOutputPath($"{this.CurMap.Text}.png");
            string outputFile = AppSettings.GetComfyUIOutputPath($"{this.CurMap.Text}.png");
            string refImagePath = selectedRefImage.FilePath;

            // 获取画布图像
            Bitmap canvasImage = rulerPainting.GetCanvasImage();

            // 保存为PNG
            canvasImage.Save(filename, ImageFormat.Png);

            // 创建DrawToImgIPSegProcessor实例
            DrawToImgIPSegProcessor ipSegProcessor = new DrawToImgIPSegProcessor(ref _comfyUIClient);

            // 调用ProcessDrawToImage方法进行处理
            var resultImagePath = await ipSegProcessor.ProcessDrawToImage(this.PromptBox.Text, filename, refImagePath, this.genSize.PixelSize);

            if (!string.IsNullOrEmpty(resultImagePath))
            {
                // 显示在画布上
                try
                {
                    this.genSize.GetSize(out var width, out var height);
                    rulerPainting.SetDrawingSize(width, height);
                    using (Bitmap generatedImage = new Bitmap(resultImagePath))
                    {
                        // 显示生成的图像在当前画布上
                        rulerPainting.DisplayImageOnCanvas(generatedImage);
                        generatedImage.Save(outputFile, ImageFormat.Png);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"无法显示生成的图像: {ex.Message}", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                finally
                {

                    ResetControl();
                }
            }

            // 释放资源
            canvasImage.Dispose();
        }

        private async void RequestIPGen()
        {
            // 禁用按钮，防止用户重复点击
            btnGen.Enabled = false;
            btnGen.Text = "生成中...";

            // 锁定画布，防止用户在生成过程中继续绘制
            rulerPainting.LockCanvas();

            // 获取当前选中的参考图
            RefImageItem? selectedRefImage = null;
            foreach (var item in mapRefLayout.Controls)
            {
                if (item is RefImageItem refItem && refItem.Index == curRefImgIdx)
                {
                    selectedRefImage = refItem;
                    break;
                }
            }

            if (selectedRefImage == null || string.IsNullOrEmpty(selectedRefImage.FilePath))
            {
                MessageBox.Show("请选择一个参考图片", "参考图不存在", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnGen.Enabled = true;
                btnGen.Text = "生成";
                rulerPainting.UnlockCanvas(); // 解锁画布
                return;
            }

            string refImagePath = selectedRefImage.FilePath;
            string outputFile = AppSettings.GetComfyUIOutputPath($"{this.CurMap.Text}.png");

            // 创建ImgIPAdapterProcessor实例
            ImgIPAdapterProcessor ipAdapterProcessor = new ImgIPAdapterProcessor(ref _comfyUIClient);

            // 调用ProcessDrawToImage方法进行处理
            var resultImagePath = await ipAdapterProcessor.ProcessDrawToImage(this.PromptBox.Text, refImagePath, this.genSize.PixelSize);

            if (!string.IsNullOrEmpty(resultImagePath))
            {
                // 显示在画布上
                try
                {
                    this.genSize.GetSize(out var width, out var height);
                    rulerPainting.SetDrawingSize(width, height);
                    using (Bitmap generatedImage = new Bitmap(resultImagePath))
                    {
                        // 显示生成的图像在当前画布上
                        rulerPainting.DisplayImageOnCanvas(generatedImage);
                        generatedImage.Save(outputFile, ImageFormat.Png);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"无法显示生成的图像: {ex.Message}", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                finally
                {
                    ResetControl();
                }
            }
        }

        private async void RequestDecoratorInPaint()
        {
            // 获取绘图区域
            Bitmap canvasImage = rulerPainting.GetCanvasImage();
            
            // 获取所有装饰物项
            var decoratorItems = rulerPainting.GetDecorators();
            
            // 确保有有效的装饰物
            if (canvasImage == null || decoratorItems.Count == 0)
            {
                MessageBox.Show("未添加任何装饰物，无法进行融合。", "操作失败",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            // 创建装饰物区域的遮罩
            Bitmap maskImage = new Bitmap(canvasImage.Width, canvasImage.Height);
            
            // 使用Graphics绘制装饰物的遮罩
            using (Graphics g = Graphics.FromImage(maskImage))
            {
                // 先将整个遮罩涂黑（黑色表示不重绘区域）
                g.Clear(Color.Black);
                
                // 设置绘图品质
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                
                // 遍历所有装饰物，在对应位置绘制白色区域（白色表示重绘区域）
                foreach (var item in decoratorItems)
                {
                    // 计算装饰物在画布上的位置和大小
                    int x = item.Location.X - item.Size / 2;
                    int y = item.Location.Y - item.Size / 2;
                    int size = item.Size;
                    
                    // 绘制白色填充区域
                    using (Brush brush = new SolidBrush(Color.White))
                    {
                        g.FillEllipse(brush, x, y, size, size);
                    }
                }
            }
            
            // 保存文件路径
            string canvasPath = AppSettings.GetComfyUIOutputPath($"{this.CurMap.Text}.png");
            string maskPath = AppSettings.GetMaskDrawOutputPath($"{this.CurMap.Text}_decorator_mask.png");

            // 保存遮罩图像
            maskImage.Save(maskPath, System.Drawing.Imaging.ImageFormat.Png);
            
            // // 更新界面状态
            // this.btnGen.Enabled = false;
            // this.btnGen.Text = "处理中...";

            // // 锁定画布，防止用户在重绘过程中继续绘制
            // rulerPainting.LockCanvas();

            // // 使用异步方法处理生成过程
            // DecoratorInpaintProcessor inpaintProcessor = new(ref _comfyUIClient);
            // string resultImagePath = await inpaintProcessor.ProcessInpainting(promptText, canvasPath, maskPath);

            // if (!string.IsNullOrEmpty(resultImagePath))
            // {
            //     try
            //     {
            //         // 加载生成的图像
            //         using (Bitmap resultImage = new Bitmap(resultImagePath))
            //         {
            //             // 显示在画布上
            //             rulerPainting.DisplayImageOnCanvas(resultImage);

            //             // 保存结果
            //             resultImage.Save(canvasPath, ImageFormat.Png);

            //             // 清除装饰模式
            //             rulerPainting.SetDecoratorMode(false);

            //             MessageBox.Show($"装饰物融合完成！", "操作成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //         }
            //     }
            //     catch (Exception ex)
            //     {
            //         MessageBox.Show($"显示结果图像时出错: {ex.Message}", "错误",
            //             MessageBoxButtons.OK, MessageBoxIcon.Error);
            //     }
            //     finally
            //     {
            //         ResetControl();
            //     }
            // }
            // else
            // {
            //     MessageBox.Show("生成图像失败，请重试。", "操作失败",
            //             MessageBoxButtons.OK, MessageBoxIcon.Error);
            //     ResetControl();
            // }
        }

        #endregion


    }
}
