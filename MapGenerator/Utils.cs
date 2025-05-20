using MapGenerator.Components;

namespace MapGenerator.Utils
{

    public class Utility
    {
        // 获取指定文件夹中所有图片的路径
        public static List<string> GetImagePathsFromFolder(string folderPath)
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

        public static void LoadCheckableImageToFlowLayout(string[] imagePaths, EventHandler ReferenceImage_Click, ref FlowLayoutPanel layout, int[]? size = null)
        {
            layout.Controls.Clear();
            layout.FlowDirection = FlowDirection.LeftToRight;
            layout.WrapContents = true;
            layout.AutoScroll = true;

            // 如果没有找到图片，显示提示信息
            if (imagePaths.Length == 0)
            {
                Label noImageLabel = new Label
                {
                    Text = $"没有图片",
                    AutoSize = true,
                    Font = new Font("Arial", 10)
                };
                layout.Controls.Add(noImageLabel);
            }
            else
            {
                try
                {
                    for (int i = 0; i < imagePaths.Length; i++)
                    {
                        // 创建PictureBox控件来显示图片
                        CheckableImageItem refItem = new CheckableImageItem();

                        refItem.SetContent(Path.GetFileNameWithoutExtension(imagePaths[i]), imagePaths[i], size);

                        // 添加点击事件，方便用户点击查看大图
                        if (ReferenceImage_Click != null)
                            refItem.Click += ReferenceImage_Click;

                        refItem.DoubleClick += ReferenceImage_DBClick;

                        // 将图片添加到布局中
                        layout.Controls.Add(refItem);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"加载图片时出错: {ex.Message}");
                }
            }
        }

        // 双击参考图片事件处理
        private static void ReferenceImage_DBClick(object? sender, EventArgs e)
        {
            if (sender is CheckableImageItem refItem && refItem.FilePath is string imagePath)
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

        public static void LoadIconItemControl(bool isVertical, string[] imagePaths, ref FlowLayoutPanel layout, EventHandler? clickFunc = null, int[]? size = null, ContextMenuStrip? menuStrip = null)
        {
            layout.Controls.Clear();

            for (int i = 0; i < imagePaths.Length; i++)
            {
                if (isVertical)
                {
                    IconItemControl_V iconItemControl = new IconItemControl_V();
                    if (menuStrip != null)
                        iconItemControl.ContextMenuStrip = menuStrip;
                    string fileName = Path.GetFileNameWithoutExtension(imagePaths[i]);
                    iconItemControl.SetImg(imagePaths[i], fileName, i, size);
                    if (clickFunc != null)
                        iconItemControl.Click += clickFunc;
                    layout.Controls.Add(iconItemControl);
                }
                else
                {
                    IconItemControl_H iconItemControl = new IconItemControl_H();
                    if (menuStrip != null)
                        iconItemControl.ContextMenuStrip = menuStrip;
                    string fileName = Path.GetFileNameWithoutExtension(imagePaths[i]);
                    iconItemControl.SetImg(imagePaths[i], fileName, i, size);
                    if (clickFunc != null)
                        iconItemControl.Click += clickFunc;
                    layout.Controls.Add(iconItemControl);
                }

            }
        }

        public static TabPage NewPaintingTab(ref TabControl tabControl, bool lockCursorIcon = false)
        {
            //MainViewTab新增一个tab
            TabPage newTabPage = new TabPage();
            newTabPage.AutoScroll = false;
            tabControl.TabPages.Add(newTabPage);
            newTabPage.Text = "New";

            RulerPainting rulerPainting = new RulerPainting();
            if (lockCursorIcon)
                rulerPainting.CurserIconCanChange(false);
            newTabPage.Controls.Add(rulerPainting);
            rulerPainting.Dock = DockStyle.Fill;

            tabControl.SelectedIndex = tabControl.Controls.Count - 1;

            return newTabPage;
        }


        public static void LogStr(string str)
        {
            System.Diagnostics.Debug.WriteLine(str);
        }
    }
}