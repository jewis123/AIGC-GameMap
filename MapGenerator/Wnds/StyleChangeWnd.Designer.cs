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
            refTab = new TabPage();
            refLayout = new FlowLayoutPanel();
            templateRefTab = new TabPage();
            templateLayout = new FlowLayoutPanel();
            rawTab = new TabPage();
            rawImageLayout = new FlowLayoutPanel();
            recordTab = new TabPage();
            recordsLayout = new FlowLayoutPanel();
            Buttons = new Panel();
            flowLayoutPanel3 = new FlowLayoutPanel();
            label22 = new Label();
            genCnt = new TextBox();
            flowLayoutPanel2 = new FlowLayoutPanel();
            btnGen = new Button();
            btnStop = new Button();
            genSize = new Components.SizeSetting();
            ToolPanel = new Panel();
            flowLayoutPanel1 = new FlowLayoutPanel();
            btnResume = new Button();
            btnRepaint = new Button();
            btnRemove = new Button();
            btnPick = new Button();
            menuStrip = new MenuStrip();
            FolderStripMenuItem = new ToolStripMenuItem();
            loadStripMenuItem = new ToolStripMenuItem();
            HelpStripMenuItem = new ToolStripMenuItem();
            MainViewTab = new TabControl();
            curMapContextMenu = new ContextMenuStrip(components);
            saveMapMenuItem = new ToolStripMenuItem();
            renameMapMenuItem = new ToolStripMenuItem();
            closeMapMenuItem = new ToolStripMenuItem();
            CurMap = new TabPage();
            rulerPainting = new Components.RulerPainting();
            reloadDrawingMenuItem = new ToolStripMenuItem();
            clearMapMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            RightSideLayOut.SuspendLayout();
            tabs.SuspendLayout();
            refTab.SuspendLayout();
            templateRefTab.SuspendLayout();
            rawTab.SuspendLayout();
            recordTab.SuspendLayout();
            Buttons.SuspendLayout();
            flowLayoutPanel3.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            ToolPanel.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            menuStrip.SuspendLayout();
            MainViewTab.SuspendLayout();
            curMapContextMenu.SuspendLayout();
            CurMap.SuspendLayout();
            SuspendLayout();
            // 
            // RightSideLayOut
            // 
            RightSideLayOut.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            RightSideLayOut.ColumnCount = 1;
            RightSideLayOut.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            RightSideLayOut.Controls.Add(PromptBox, 0, 1);
            RightSideLayOut.Controls.Add(tabs, 0, 0);
            RightSideLayOut.Location = new Point(563, 54);
            RightSideLayOut.Name = "RightSideLayOut";
            RightSideLayOut.RowCount = 2;
            RightSideLayOut.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            RightSideLayOut.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
            RightSideLayOut.Size = new Size(304, 470);
            RightSideLayOut.TabIndex = 0;
            // 
            // PromptBox
            // 
            PromptBox.Dock = DockStyle.Fill;
            PromptBox.ForeColor = SystemColors.GrayText;
            PromptBox.Hint = null;
            PromptBox.Location = new Point(3, 373);
            PromptBox.Name = "PromptBox";
            PromptBox.Size = new Size(298, 94);
            PromptBox.TabIndex = 3;
            PromptBox.Text = "";
            // 
            // tabs
            // 
            tabs.Controls.Add(refTab);
            tabs.Controls.Add(templateRefTab);
            tabs.Controls.Add(rawTab);
            tabs.Controls.Add(recordTab);
            tabs.Dock = DockStyle.Fill;
            tabs.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            tabs.Location = new Point(3, 3);
            tabs.Name = "tabs";
            tabs.SelectedIndex = 0;
            tabs.Size = new Size(298, 364);
            tabs.TabIndex = 1;
            // 
            // refTab
            // 
            refTab.Controls.Add(refLayout);
            refTab.Location = new Point(4, 26);
            refTab.Name = "refTab";
            refTab.Size = new Size(290, 334);
            refTab.TabIndex = 1;
            refTab.Text = "特征参考";
            refTab.UseVisualStyleBackColor = true;
            // 
            // refLayout
            // 
            refLayout.AutoScroll = true;
            refLayout.Dock = DockStyle.Fill;
            refLayout.Location = new Point(0, 0);
            refLayout.Name = "refLayout";
            refLayout.Padding = new Padding(0, 0, 3, 3);
            refLayout.Size = new Size(290, 334);
            refLayout.TabIndex = 0;
            // 
            // templateRefTab
            // 
            templateRefTab.Controls.Add(templateLayout);
            templateRefTab.Location = new Point(4, 26);
            templateRefTab.Name = "templateRefTab";
            templateRefTab.Size = new Size(290, 334);
            templateRefTab.TabIndex = 2;
            templateRefTab.Text = "预设风格";
            templateRefTab.UseVisualStyleBackColor = true;
            // 
            // templateLayout
            // 
            templateLayout.AutoScroll = true;
            templateLayout.Dock = DockStyle.Fill;
            templateLayout.Location = new Point(0, 0);
            templateLayout.Name = "templateLayout";
            templateLayout.Padding = new Padding(0, 0, 3, 3);
            templateLayout.Size = new Size(290, 334);
            templateLayout.TabIndex = 0;
            // 
            // rawTab
            // 
            rawTab.Controls.Add(rawImageLayout);
            rawTab.Location = new Point(4, 26);
            rawTab.Name = "rawTab";
            rawTab.Padding = new Padding(3);
            rawTab.Size = new Size(290, 334);
            rawTab.TabIndex = 3;
            rawTab.Text = "原始";
            rawTab.UseVisualStyleBackColor = true;
            // 
            // rawImageLayout
            // 
            rawImageLayout.AutoScroll = true;
            rawImageLayout.Dock = DockStyle.Fill;
            rawImageLayout.Location = new Point(3, 3);
            rawImageLayout.Name = "rawImageLayout";
            rawImageLayout.Padding = new Padding(0, 0, 3, 3);
            rawImageLayout.Size = new Size(284, 328);
            rawImageLayout.TabIndex = 0;
            // 
            // recordTab
            // 
            recordTab.Controls.Add(recordsLayout);
            recordTab.Location = new Point(4, 26);
            recordTab.Name = "recordTab";
            recordTab.Padding = new Padding(3);
            recordTab.Size = new Size(290, 334);
            recordTab.TabIndex = 4;
            recordTab.Text = "替换记录";
            recordTab.UseVisualStyleBackColor = true;
            // 
            // recordsLayout
            // 
            recordsLayout.AutoScroll = true;
            recordsLayout.Dock = DockStyle.Fill;
            recordsLayout.Location = new Point(3, 3);
            recordsLayout.Name = "recordsLayout";
            recordsLayout.Padding = new Padding(0, 0, 3, 3);
            recordsLayout.Size = new Size(284, 328);
            recordsLayout.TabIndex = 0;
            // 
            // Buttons
            // 
            Buttons.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Buttons.AutoScroll = true;
            Buttons.Controls.Add(flowLayoutPanel3);
            Buttons.Controls.Add(flowLayoutPanel2);
            Buttons.Controls.Add(genSize);
            Buttons.Location = new Point(572, 530);
            Buttons.Name = "Buttons";
            Buttons.Size = new Size(301, 150);
            Buttons.TabIndex = 7;
            // 
            // flowLayoutPanel3
            // 
            flowLayoutPanel3.AutoSize = true;
            flowLayoutPanel3.Controls.Add(label22);
            flowLayoutPanel3.Controls.Add(genCnt);
            flowLayoutPanel3.Location = new Point(12, 38);
            flowLayoutPanel3.Name = "flowLayoutPanel3";
            flowLayoutPanel3.Size = new Size(240, 29);
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
            flowLayoutPanel2.Anchor = AnchorStyles.None;
            flowLayoutPanel2.AutoSize = true;
            flowLayoutPanel2.Controls.Add(btnGen);
            flowLayoutPanel2.Controls.Add(btnStop);
            flowLayoutPanel2.Location = new Point(9, 118);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new Size(180, 29);
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
            // genSize
            // 
            genSize.Location = new Point(3, 3);
            genSize.Name = "genSize";
            genSize.Size = new Size(286, 29);
            genSize.TabIndex = 1;
            // 
            // ToolPanel
            // 
            ToolPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ToolPanel.Controls.Add(flowLayoutPanel1);
            ToolPanel.Location = new Point(3, 652);
            ToolPanel.Name = "ToolPanel";
            ToolPanel.Size = new Size(567, 28);
            ToolPanel.TabIndex = 8;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(btnResume);
            flowLayoutPanel1.Controls.Add(btnRepaint);
            flowLayoutPanel1.Controls.Add(btnRemove);
            flowLayoutPanel1.Controls.Add(btnPick);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Location = new Point(0, 0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(567, 28);
            flowLayoutPanel1.TabIndex = 0;
            // 
            // btnResume
            // 
            btnResume.Location = new Point(3, 3);
            btnResume.Name = "btnResume";
            btnResume.Size = new Size(75, 23);
            btnResume.TabIndex = 3;
            btnResume.Text = "恢复指针";
            btnResume.UseVisualStyleBackColor = true;
            btnResume.Click += resume_Click;
            // 
            // btnRepaint
            // 
            btnRepaint.Location = new Point(84, 3);
            btnRepaint.Name = "btnRepaint";
            btnRepaint.Size = new Size(75, 23);
            btnRepaint.TabIndex = 0;
            btnRepaint.Text = "区域重绘";
            btnRepaint.UseVisualStyleBackColor = true;
            btnRepaint.Click += repaint_Click;
            // 
            // btnRemove
            // 
            btnRemove.Location = new Point(165, 3);
            btnRemove.Name = "btnRemove";
            btnRemove.Size = new Size(75, 23);
            btnRemove.TabIndex = 1;
            btnRemove.Text = "区域擦除";
            btnRemove.UseVisualStyleBackColor = true;
            btnRemove.Click += remove_Click;
            // 
            // btnPick
            // 
            btnPick.Location = new Point(246, 3);
            btnPick.Name = "btnPick";
            btnPick.Size = new Size(75, 23);
            btnPick.TabIndex = 2;
            btnPick.Text = "拾取装饰";
            btnPick.UseVisualStyleBackColor = true;
            btnPick.Click += pick_Click;
            // 
            // menuStrip
            // 
            menuStrip.Items.AddRange(new ToolStripItem[] { FolderStripMenuItem, HelpStripMenuItem });
            menuStrip.LayoutStyle = ToolStripLayoutStyle.Flow;
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(876, 25);
            menuStrip.TabIndex = 9;
            menuStrip.Text = "menuStrip1";
            // 
            // FolderStripMenuItem
            // 
            FolderStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { loadStripMenuItem });
            FolderStripMenuItem.Name = "FolderStripMenuItem";
            FolderStripMenuItem.Size = new Size(44, 21);
            FolderStripMenuItem.Text = "文件";
            // 
            // loadStripMenuItem
            // 
            loadStripMenuItem.Name = "loadStripMenuItem";
            loadStripMenuItem.Size = new Size(124, 22);
            loadStripMenuItem.Text = "加载记录";
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
            MainViewTab.Controls.Add(CurMap);
            MainViewTab.Location = new Point(0, 28);
            MainViewTab.Name = "MainViewTab";
            MainViewTab.SelectedIndex = 0;
            MainViewTab.Size = new Size(566, 618);
            MainViewTab.TabIndex = 10;
            // 
            // curMapContextMenu
            // 
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
            // CurMap
            // 
            CurMap.AutoScroll = true;
            CurMap.Controls.Add(rulerPainting);
            CurMap.Location = new Point(4, 26);
            CurMap.Name = "CurMap";
            CurMap.Padding = new Padding(3);
            CurMap.Size = new Size(558, 588);
            CurMap.TabIndex = 0;
            CurMap.Text = "tab1";
            CurMap.UseVisualStyleBackColor = true;
            // 
            // rulerPainting
            // 
            rulerPainting.AutoScroll = true;
            rulerPainting.BackColor = Color.LightGray;
            rulerPainting.Dock = DockStyle.Fill;
            rulerPainting.Location = new Point(3, 3);
            rulerPainting.Name = "rulerPainting";
            rulerPainting.Size = new Size(552, 582);
            rulerPainting.TabIndex = 0;
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
            // StyleChangeWnd
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(876, 682);
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
            refTab.ResumeLayout(false);
            templateRefTab.ResumeLayout(false);
            rawTab.ResumeLayout(false);
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
            MainViewTab.ResumeLayout(false);
            curMapContextMenu.ResumeLayout(false);
            CurMap.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel RightSideLayOut;
        private TabControl tabs;
        private TabPage refTab;
        private TabPage templateRefTab;
        private HintRichTextBox PromptBox;
        private Panel Buttons;
        private FlowLayoutPanel flowLayoutPanel2;
        private Button btnGen;
        private Button btnStop;
        private Components.SizeSetting genSize;
        private Panel ToolPanel;
        private MenuStrip menuStrip;
        private ToolStripMenuItem FolderStripMenuItem;
        private ToolStripMenuItem HelpStripMenuItem;
        private TabControl MainViewTab;
        private TabPage CurMap;
        private Components.RulerPainting rulerPainting;
        private FlowLayoutPanel flowLayoutPanel1;
        private Button btnRepaint;
        private Button btnRemove;
        private Button btnPick;
        private Button btnResume;
        private ContextMenuStrip curMapContextMenu;
        private ToolStripMenuItem reloadDrawingMenuItem;
        private ToolStripMenuItem saveMapMenuItem;
        private ToolStripMenuItem clearMapMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem renameMapMenuItem;
        private ToolStripMenuItem closeMapMenuItem;
        private ToolStripMenuItem loadStripMenuItem;
        private FlowLayoutPanel refLayout;
        private FlowLayoutPanel templateLayout;
        private TabPage rawTab;
        private FlowLayoutPanel flowLayoutPanel3;
        private Label label22;
        private TextBox genCnt;
        private FlowLayoutPanel rawImageLayout;
        private TabPage recordTab;
        private FlowLayoutPanel recordsLayout;
    }
}