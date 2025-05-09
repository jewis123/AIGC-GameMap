using MapGenerator.Wnds;

namespace MapGenerator
{
    public partial class Enter : Form
    {

        public Enter()
        {
            InitializeComponent();

            this.StartPosition = FormStartPosition.CenterScreen;
            // 添加窗体关闭事件
            this.FormClosing += Enter_FormClosing;
        }

        // 添加窗体关闭事件处理
        private void Enter_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing || e.CloseReason == CloseReason.ApplicationExitCall)
            {
                // 清理临时文件
                CleanupTempFiles();
            }
        }

        // 清理临时文件的静态方法
        public static void CleanupTempFiles()
        {
            try
            {
                string tempPath = AppSettings.AssetTempDir;

                if (Directory.Exists(tempPath))
                {
                    // 获取目录下的所有文件
                    string[] files = Directory.GetFiles(tempPath, "*", SearchOption.AllDirectories);

                    foreach (string file in files)
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch (Exception ex)
                        {
                            // 忽略单个文件删除失败的错误，继续删除其他文件
                            Console.WriteLine($"无法删除文件 {file}: {ex.Message}");
                        }
                    }

                    // 显示成功消息
                    Console.WriteLine($"已清理 {files.Length} 个临时文件");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"清理临时文件时出错: {ex.Message}");
            }
        }

        private void Enter_Load(object sender, EventArgs e)
        {
            InitializeCreationMapListView();
            InitializeStyleChangeRecordListView();
        }

        #region Createion Mode
        private int selectMapxIdx;
        private void InitializeCreationMapListView()
        {
            listView.View = View.LargeIcon;


            ImageList imageList = new ImageList();
            imageList.ImageSize = new Size(128, 128);


            imageList.Images.Add(Image.FromFile(AppSettings.GetImagePath("map_icon/2dRPG.png")));
            imageList.Images.Add(Image.FromFile(AppSettings.GetImagePath("map_icon/2dTopDown.png")));
            imageList.Images.Add(Image.FromFile(AppSettings.GetImagePath("map_icon/coming.png")));
            listView.LargeImageList = imageList;

            ListViewItem item1 = new ListViewItem("2dRPG");
            item1.ImageIndex = 0;
            listView.Items.Add(item1);

            ListViewItem item2 = new ListViewItem("2dTopDown");
            item2.ImageIndex = 1;
            listView.Items.Add(item2);

            ListViewItem item3 = new ListViewItem("coming soon");
            item3.ImageIndex = 2;
            listView.Items.Add(item3);


            listView.ItemSelectionChanged += ListView_ItemSelectionChanged;
        }

        private void ListView_ItemSelectionChanged(object? sender, ListViewItemSelectionChangedEventArgs e)
        {
            selectMapxIdx = e.ItemIndex;
        }

        /// <summary>
        /// 创作模式新建
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreate_Click(object sender, EventArgs e)
        {
            MainEditor win = new MainEditor();
            win.SetMapIndex(selectMapxIdx);
            win.FormClosed += (s, args) =>
            {
                this.Show();
            };

            win.Show();
            this.Hide();
        }
        #endregion

        #region Style Mode
        private int selectRecordIdx = -1;

        private void InitializeStyleChangeRecordListView()
        {
            projectList.Columns.Clear();
            projectList.Items.Clear();
            var records = File.ReadAllLines(Path.Combine(AppSettings.ArtChangesDirectory, "records.txt"));
            projectList.Columns.Add("历史记录", projectList.ClientSize.Width); // 列名和宽度
            projectList.FullRowSelect = true;
            foreach (var record in records)
            {
                projectList.Items.Add(record);
            }
            projectList.ItemSelectionChanged += ProjectList_SelectedIndexChanged;

            // 添加右键菜单
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem deleteItem = new ToolStripMenuItem("删除记录");
            deleteItem.Click += DeleteRecordMenuItem_Click;
            contextMenu.Items.Add(deleteItem);
            projectList.ContextMenuStrip = contextMenu;
        }

        private void DeleteRecordMenuItem_Click(object? sender, EventArgs e)
        {
            if (projectList.SelectedItems.Count == 0) return;
            var item = projectList.SelectedItems[0];
            var recordName = item.Text;
            var result = MessageBox.Show($"确定要删除记录：{recordName} 吗？\n此操作不可恢复。", "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                // 删除records.txt中的对应行
                string recordsPath = Path.Combine(AppSettings.ArtChangesDirectory, "records.txt");
                var allRecords = File.ReadAllLines(recordsPath).ToList();
                allRecords.RemoveAll(r => r.Trim() == recordName.Trim());
                File.WriteAllLines(recordsPath, allRecords);
                // 删除对应文件夹
                string dir = Path.Combine(AppSettings.ArtChangesDirectory, recordName);
                if (Directory.Exists(dir))
                {
                    try { Directory.Delete(dir, true); } catch { }
                }
                InitializeStyleChangeRecordListView();
            }
        }

        private void ProjectList_SelectedIndexChanged(object? sender, ListViewItemSelectionChangedEventArgs e)
        {
            selectRecordIdx = e.ItemIndex;
        }

        /// <summary>
        /// 换皮模式新建
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newBtn_Click(object sender, EventArgs e)
        {
            InputStringDialog dialog = new InputStringDialog();
            // 显示对话框
            Application.EnableVisualStyles();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                //写入records.txt
                File.AppendAllText(Path.Combine(AppSettings.ArtChangesDirectory, "records.txt"), dialog.RecordName + "\r\n");
                Directory.CreateDirectory(Path.Combine(AppSettings.ArtChangesDirectory, dialog.RecordName));
                InitializeStyleChangeRecordListView();

                StyleChangeWnd win = new StyleChangeWnd();
                win.RecordDirName = dialog.RecordName;
                win.FormClosed += (s, args) =>
                {
                    this.Show();
                };
                win.Show();
                this.Hide();
            }
        }

        /// <summary>
        /// 换皮模式打开历史记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (selectRecordIdx == -1)
            {
                MessageBox.Show(this, "先选择记录");
                return;
            }

            StyleChangeWnd win = new StyleChangeWnd();
            win.RecordDirName = projectList.Items[selectRecordIdx].Text;
            win.FormClosed += (s, args) =>
            {
                this.Show();
            };

            win.Show();
            this.Hide();
        }
        #endregion


    }
}
