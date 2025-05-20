namespace MapGenerator.Wnds
{
    partial class StyleChangeWnd
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            RightSideLayOut = new TableLayoutPanel();
            PromptBox = new HintRichTextBox();
            tabs = new TabControl();
            templateRefTab = new TabPage();
            templateLayout = new FlowLayoutPanel();
            templateSearch = new HintRichTextBox();
            rawTab = new TabPage();
            rawImageLayout = new FlowLayoutPanel();
            rawSearch = new HintRichTextBox();
            refTab = new TabPage();
            refLayout = new FlowLayoutPanel();
            hintRichTextBox1 = new HintRichTextBox();
            recordTab = new TabPage();
            recordsLayout = new FlowLayoutPanel();
            recordSearch = new HintRichTextBox();
            Buttons = new Panel();
            flowLayoutPanel3 = new FlowLayoutPanel();
            label22 = new Label();
            genCnt = new TextBox();
            flowLayoutPanel2 = new FlowLayoutPanel();
            btnGen = new Button();
            btnStop = new Button();
            ToolPanel = new Panel();
            flowLayoutPanel1 = new FlowLayoutPanel();
            btnRemove = new Button();
            menuStrip = new MenuStrip();
            FolderStripMenuItem = new ToolStripMenuItem();
            HelpStripMenuItem = new ToolStripMenuItem();
            MainViewTab = new TabControl();
            curMapContextMenu = new ContextMenuStrip(components);
            saveMapMenuItem = new ToolStripMenuItem();
            renameMapMenuItem = new ToolStripMenuItem();
            closeMapMenuItem = new ToolStripMenuItem();
            reloadDrawingMenuItem = new ToolStripMenuItem();
            clearMapMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            zoomBar = new Components.ZoomBar();
            imgMenu = new ContextMenuStrip(components);
            deleteImg = new ToolStripMenuItem();
            openImg = new ToolStripMenuItem();
            denoiseBar = new Components.ZoomBar();
            RightSideLayOut.SuspendLayout();
            tabs.SuspendLayout();
            templateRefTab.SuspendLayout();
            rawTab.SuspendLayout();
            refTab.SuspendLayout();
            recordTab.SuspendLayout();
            Buttons.SuspendLayout();
            flowLayoutPanel3.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            ToolPanel.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            menuStrip.SuspendLayout();
            curMapContextMenu.SuspendLayout();
            imgMenu.SuspendLayout();
            SuspendLayout();
            // 
            // RightSideLayOut
            // 
            RightSideLayOut.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            RightSideLayOut.ColumnCount = 1;
            RightSideLayOut.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            RightSideLayOut.Controls.Add(PromptBox, 0, 1);
            RightSideLayOut.Controls.Add(tabs, 0, 0);
            RightSideLayOut.Location = new Point(627, 54);
            RightSideLayOut.Name = "RightSideLayOut";
            RightSideLayOut.RowCount = 2;
            RightSideLayOut.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            RightSideLayOut.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
            RightSideLayOut.Size = new Size(304, 429);
            RightSideLayOut.TabIndex = 0;
            // 
            // PromptBox
            // 
            PromptBox.Dock = DockStyle.Fill;
            PromptBox.ForeColor = SystemColors.GrayText;
            PromptBox.Hint = null;
            PromptBox.Location = new Point(3, 332);
            PromptBox.Name = "PromptBox";
            PromptBox.Size = new Size(298, 94);
            PromptBox.TabIndex = 3;
            PromptBox.Text = "";
            // 
            // tabs
            // 
            tabs.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabs.Controls.Add(templateRefTab);
            tabs.Controls.Add(rawTab);
            tabs.Controls.Add(refTab);
            tabs.Controls.Add(recordTab);
            tabs.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            tabs.Location = new Point(3, 3);
            tabs.Name = "tabs";
            tabs.SelectedIndex = 0;
            tabs.Size = new Size(298, 323);
            tabs.TabIndex = 1;
            // 
            // templateRefTab
            // 
            templateRefTab.Controls.Add(templateLayout);
            templateRefTab.Controls.Add(templateSearch);
            templateRefTab.Location = new Point(4, 26);
            templateRefTab.Name = "templateRefTab";
            templateRefTab.Size = new Size(290, 293);
            templateRefTab.TabIndex = 2;
            templateRefTab.Text = "预设风格";
            templateRefTab.UseVisualStyleBackColor = true;
            // 
            // templateLayout
            // 
            templateLayout.Dock = DockStyle.Fill;
            templateLayout.Location = new Point(0, 26);
            templateLayout.Name = "templateLayout";
            templateLayout.Size = new Size(290, 267);
            templateLayout.TabIndex = 1;
            // 
            // templateSearch
            // 
            templateSearch.Dock = DockStyle.Top;
            templateSearch.ForeColor = SystemColors.GrayText;
            templateSearch.Hint = null;
            templateSearch.Location = new Point(0, 0);
            templateSearch.Margin = new Padding(2, 3, 2, 3);
            templateSearch.Name = "templateSearch";
            templateSearch.Size = new Size(290, 26);
            templateSearch.TabIndex = 12;
            templateSearch.Text = "";
            // 
            // rawTab
            // 
            rawTab.Controls.Add(rawImageLayout);
            rawTab.Controls.Add(rawSearch);
            rawTab.Location = new Point(4, 26);
            rawTab.Name = "rawTab";
            rawTab.Padding = new Padding(3);
            rawTab.Size = new Size(290, 293);
            rawTab.TabIndex = 3;
            rawTab.Text = "原始图";
            rawTab.UseVisualStyleBackColor = true;
            // 
            // rawImageLayout
            // 
            rawImageLayout.AutoScroll = true;
            rawImageLayout.Dock = DockStyle.Fill;
            rawImageLayout.Location = new Point(3, 29);
            rawImageLayout.Name = "rawImageLayout";
            rawImageLayout.Size = new Size(284, 261);
            rawImageLayout.TabIndex = 0;
            // 
            // rawSearch
            // 
            rawSearch.Dock = DockStyle.Top;
            rawSearch.ForeColor = SystemColors.GrayText;
            rawSearch.Hint = null;
            rawSearch.Location = new Point(3, 3);
            rawSearch.Margin = new Padding(2, 3, 2, 3);
            rawSearch.Name = "rawSearch";
            rawSearch.Size = new Size(284, 26);
            rawSearch.TabIndex = 12;
            rawSearch.Text = "";
            // 
            // refTab
            // 
            refTab.Controls.Add(refLayout);
            refTab.Controls.Add(hintRichTextBox1);
            refTab.Location = new Point(4, 26);
            refTab.Name = "refTab";
            refTab.Padding = new Padding(3);
            refTab.Size = new Size(290, 293);
            refTab.TabIndex = 5;
            refTab.Text = "特征图";
            refTab.UseVisualStyleBackColor = true;
            // 
            // refLayout
            // 
            refLayout.AutoScroll = true;
            refLayout.Dock = DockStyle.Fill;
            refLayout.Location = new Point(3, 32);
            refLayout.Name = "refLayout";
            refLayout.Size = new Size(284, 258);
            refLayout.TabIndex = 1;
            // 
            // hintRichTextBox1
            // 
            hintRichTextBox1.Dock = DockStyle.Top;
            hintRichTextBox1.ForeColor = SystemColors.GrayText;
            hintRichTextBox1.Hint = null;
            hintRichTextBox1.Location = new Point(3, 3);
            hintRichTextBox1.Name = "hintRichTextBox1";
            hintRichTextBox1.Size = new Size(284, 29);
            hintRichTextBox1.TabIndex = 0;
            hintRichTextBox1.Text = "";
            // 
            // recordTab
            // 
            recordTab.Controls.Add(recordsLayout);
            recordTab.Controls.Add(recordSearch);
            recordTab.Location = new Point(4, 26);
            recordTab.Name = "recordTab";
            recordTab.Padding = new Padding(3);
            recordTab.Size = new Size(290, 293);
            recordTab.TabIndex = 4;
            recordTab.Text = "替换记录";
            recordTab.UseVisualStyleBackColor = true;
            // 
            // recordsLayout
            // 
            recordsLayout.AutoScroll = true;
            recordsLayout.Dock = DockStyle.Fill;
            recordsLayout.Location = new Point(3, 29);
            recordsLayout.Name = "recordsLayout";
            recordsLayout.Size = new Size(284, 261);
            recordsLayout.TabIndex = 0;
            // 
            // recordSearch
            // 
            recordSearch.Dock = DockStyle.Top;
            recordSearch.ForeColor = SystemColors.GrayText;
            recordSearch.Hint = null;
            recordSearch.Location = new Point(3, 3);
            recordSearch.Margin = new Padding(2, 3, 2, 3);
            recordSearch.Name = "recordSearch";
            recordSearch.Size = new Size(284, 26);
            recordSearch.TabIndex = 12;
            recordSearch.Text = "";
            // 
            // Buttons
            // 
            Buttons.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Buttons.Controls.Add(denoiseBar);
            Buttons.Controls.Add(flowLayoutPanel3);
            Buttons.Controls.Add(flowLayoutPanel2);
            Buttons.Location = new Point(636, 489);
            Buttons.Name = "Buttons";
            Buttons.Size = new Size(301, 150);
            Buttons.TabIndex = 7;
            // 
            // flowLayoutPanel3
            // 
            flowLayoutPanel3.AutoSize = true;
            flowLayoutPanel3.Controls.Add(label22);
            flowLayoutPanel3.Controls.Add(genCnt);
            flowLayoutPanel3.Location = new Point(0, 57);
            flowLayoutPanel3.Name = "flowLayoutPanel3";
            flowLayoutPanel3.Size = new Size(301, 35);
            flowLayoutPanel3.TabIndex = 5;
            // 
            // label22
            // 
            label22.Anchor = AnchorStyles.Left;
            label22.AutoSize = true;
            label22.Location = new Point(3, 7);
            label22.Margin = new Padding(3, 3, 3, 0);
            label22.Name = "label22";
            label22.Size = new Size(128, 17);
            label22.TabIndex = 0;
            label22.Text = "一次性图片生成数量：";
            // 
            // genCnt
            // 
            genCnt.Location = new Point(137, 3);
            genCnt.Name = "genCnt";
            genCnt.Size = new Size(100, 23);
            genCnt.TabIndex = 1;
            genCnt.Text = "1";
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            flowLayoutPanel2.AutoSize = true;
            flowLayoutPanel2.Controls.Add(btnGen);
            flowLayoutPanel2.Controls.Add(btnStop);
            flowLayoutPanel2.Location = new Point(3, 118);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new Size(180, 30);
            flowLayoutPanel2.TabIndex = 4;
            flowLayoutPanel2.WrapContents = false;
            // 
            // btnGen
            // 
            btnGen.Anchor = AnchorStyles.None;
            btnGen.Location = new Point(3, 3);
            btnGen.Name = "btnGen";
            btnGen.Size = new Size(75, 23);
            btnGen.TabIndex = 0;
            btnGen.Text = "生成";
            btnGen.UseVisualStyleBackColor = true;
            btnGen.Click += btnGen_Click;
            // 
            // btnStop
            // 
            btnStop.Location = new Point(84, 3);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(87, 23);
            btnStop.TabIndex = 3;
            btnStop.Text = "中止";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // ToolPanel
            // 
            ToolPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ToolPanel.Controls.Add(flowLayoutPanel1);
            ToolPanel.Location = new Point(3, 611);
            ToolPanel.Name = "ToolPanel";
            ToolPanel.Size = new Size(341, 28);
            ToolPanel.TabIndex = 8;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(btnRemove);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Location = new Point(0, 0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(341, 28);
            flowLayoutPanel1.TabIndex = 0;
            // 
            // btnRemove
            // 
            btnRemove.Location = new Point(3, 3);
            btnRemove.Name = "btnRemove";
            btnRemove.Size = new Size(75, 23);
            btnRemove.TabIndex = 1;
            btnRemove.Text = "区域擦除";
            btnRemove.UseVisualStyleBackColor = true;
            btnRemove.Click += remove_Click;
            // 
            // menuStrip
            // 
            menuStrip.ImageScalingSize = new Size(20, 20);
            menuStrip.Items.AddRange(new ToolStripItem[] { FolderStripMenuItem, HelpStripMenuItem });
            menuStrip.LayoutStyle = ToolStripLayoutStyle.Flow;
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(940, 25);
            menuStrip.TabIndex = 9;
            menuStrip.Text = "menuStrip1";
            // 
            // FolderStripMenuItem
            // 
            FolderStripMenuItem.Name = "FolderStripMenuItem";
            FolderStripMenuItem.Size = new Size(44, 21);
            FolderStripMenuItem.Text = "文件";
            // 
            // HelpStripMenuItem
            // 
            HelpStripMenuItem.Name = "HelpStripMenuItem";
            HelpStripMenuItem.Size = new Size(44, 21);
            HelpStripMenuItem.Text = "帮助";
            // 
            // MainViewTab
            // 
            MainViewTab.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            MainViewTab.ContextMenuStrip = curMapContextMenu;
            MainViewTab.Location = new Point(0, 28);
            MainViewTab.Name = "MainViewTab";
            MainViewTab.SelectedIndex = 0;
            MainViewTab.Size = new Size(630, 577);
            MainViewTab.TabIndex = 10;
            // 
            // curMapContextMenu
            // 
            curMapContextMenu.ImageScalingSize = new Size(20, 20);
            curMapContextMenu.Items.AddRange(new ToolStripItem[] { saveMapMenuItem, renameMapMenuItem, closeMapMenuItem });
            curMapContextMenu.Name = "curMapContextMenu";
            curMapContextMenu.Size = new Size(125, 70);
            // 
            // saveMapMenuItem
            // 
            saveMapMenuItem.Name = "saveMapMenuItem";
            saveMapMenuItem.Size = new Size(124, 22);
            saveMapMenuItem.Text = "保存地图";
            saveMapMenuItem.Click += saveMapMenuItem_Click;
            // 
            // renameMapMenuItem
            // 
            renameMapMenuItem.Name = "renameMapMenuItem";
            renameMapMenuItem.Size = new Size(124, 22);
            renameMapMenuItem.Text = "重命名";
            renameMapMenuItem.Click += renameMapMenuItem_Click;
            // 
            // closeMapMenuItem
            // 
            closeMapMenuItem.Name = "closeMapMenuItem";
            closeMapMenuItem.Size = new Size(124, 22);
            closeMapMenuItem.Text = "关闭地图";
            closeMapMenuItem.Click += closeMapMenuItem_Click;
            // 
            // reloadDrawingMenuItem
            // 
            reloadDrawingMenuItem.Name = "reloadDrawingMenuItem";
            reloadDrawingMenuItem.Size = new Size(32, 19);
            // 
            // clearMapMenuItem
            // 
            clearMapMenuItem.Name = "clearMapMenuItem";
            clearMapMenuItem.Size = new Size(32, 19);
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 6);
            // 
            // zoomBar
            // 
            zoomBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            zoomBar.BackColor = SystemColors.Control;
            zoomBar.Location = new Point(375, 611);
            zoomBar.MinimumSize = new Size(0, 29);
            zoomBar.Name = "zoomBar";
            zoomBar.Size = new Size(255, 29);
            zoomBar.TabIndex = 11;
            // 
            // imgMenu
            // 
            imgMenu.Items.AddRange(new ToolStripItem[] { deleteImg, openImg });
            imgMenu.Name = "imgMenu";
            imgMenu.Size = new Size(137, 48);
            // 
            // deleteImg
            // 
            deleteImg.Name = "deleteImg";
            deleteImg.Size = new Size(136, 22);
            deleteImg.Text = "删除";
            deleteImg.Click += deleteImg_Click;
            // 
            // openImg
            // 
            openImg.Name = "openImg";
            openImg.Size = new Size(136, 22);
            openImg.Text = "新标签打开";
            openImg.Click += openImg_Click;
            // 
            // denoiseBar
            // 
            denoiseBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            denoiseBar.BackColor = SystemColors.Control;
            denoiseBar.Location = new Point(3, 3);
            denoiseBar.MinimumSize = new Size(0, 29);
            denoiseBar.Name = "denoiseBar";
            denoiseBar.Size = new Size(255, 29);
            denoiseBar.TabIndex = 12;
            // 
            // StyleChangeWnd
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(940, 641);
            Controls.Add(zoomBar);
            Controls.Add(MainViewTab);
            Controls.Add(menuStrip);
            Controls.Add(ToolPanel);
            Controls.Add(Buttons);
            Controls.Add(RightSideLayOut);
            Name = "StyleChangeWnd";
            Text = "StyleChangeWnd";
            Load += Enter_Load;
            RightSideLayOut.ResumeLayout(false);
            tabs.ResumeLayout(false);
            templateRefTab.ResumeLayout(false);
            rawTab.ResumeLayout(false);
            refTab.ResumeLayout(false);
            recordTab.ResumeLayout(false);
            Buttons.ResumeLayout(false);
            Buttons.PerformLayout();
            flowLayoutPanel3.ResumeLayout(false);
            flowLayoutPanel3.PerformLayout();
            flowLayoutPanel2.ResumeLayout(false);
            ToolPanel.ResumeLayout(false);
            flowLayoutPanel1.ResumeLayout(false);
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            curMapContextMenu.ResumeLayout(false);
            imgMenu.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel RightSideLayOut;
        private TabControl tabs;
        private TabPage templateRefTab;
        private HintRichTextBox PromptBox;
        private Panel Buttons;
        private FlowLayoutPanel flowLayoutPanel2;
        private Button btnGen;
        private Button btnStop;
        private Panel ToolPanel;
        private MenuStrip menuStrip;
        private ToolStripMenuItem FolderStripMenuItem;
        private ToolStripMenuItem HelpStripMenuItem;
        private TabControl MainViewTab;
        private FlowLayoutPanel flowLayoutPanel1;
        private Button btnRemove;
        private ContextMenuStrip curMapContextMenu;
        private ToolStripMenuItem reloadDrawingMenuItem;
        private ToolStripMenuItem saveMapMenuItem;
        private ToolStripMenuItem clearMapMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem renameMapMenuItem;
        private ToolStripMenuItem closeMapMenuItem;
        private TabPage rawTab;
        private FlowLayoutPanel flowLayoutPanel3;
        private Label label22;
        private TextBox genCnt;
        private FlowLayoutPanel rawImageLayout;
        private TabPage recordTab;
        private FlowLayoutPanel recordsLayout;
        private HintRichTextBox templateSearch;
        private HintRichTextBox rawSearch;
        private HintRichTextBox recordSearch;
        private Components.ZoomBar zoomBar;
        private ContextMenuStrip imgMenu;
        private ToolStripMenuItem deleteImg;
        private ToolStripMenuItem openImg;
        private FlowLayoutPanel templateLayout;
        private TabPage refTab;
        private HintRichTextBox hintRichTextBox1;
        private FlowLayoutPanel refLayout;
        private Components.ZoomBar denoiseBar;
    }
}