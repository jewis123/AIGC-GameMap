namespace MapGenerator.Wnds
{
    partial class MainEditor
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
            MainViewTab = new TabControl();
            curMapContextMenu = new ContextMenuStrip(components);
            reloadDrawingMenuItem = new ToolStripMenuItem();
            saveMapMenuItem = new ToolStripMenuItem();
            clearMapMenuItem = new ToolStripMenuItem();
            partialRedrawMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            renameMapMenuItem = new ToolStripMenuItem();
            closeMapMenuItem = new ToolStripMenuItem();
            CurMap = new TabPage();
            PaintingPanel = new Panel();
            rulerPainting = new Components.RulerPainting();
            FolderStripMenuItem = new ToolStripMenuItem();
            loadImageStripMenuItem = new ToolStripMenuItem();
            HelpStripMenuItem = new ToolStripMenuItem();
            menuStrip = new MenuStrip();
            ToolPanel = new Panel();
            toolTab = new TabControl();
            brushPanel = new TabPage();
            brushList = new FlowLayoutPanel();
            decoratorPanel = new TabPage();
            decoratorList = new FlowLayoutPanel();
            Buttons = new Panel();
            flowLayoutPanel2 = new FlowLayoutPanel();
            btnGen = new Button();
            btnStop = new Button();
            paintSize = new Components.SizeSetting();
            genSize = new Components.SizeSetting();
            RightSideLayOut = new TableLayoutPanel();
            StyleList = new TableLayoutPanel();
            flowLayoutPanel1 = new FlowLayoutPanel();
            styleLable = new Label();
            mapStyle = new TabControl();
            refTab = new TabPage();
            templateRefTab = new TabPage();
            mapRefLayout = new FlowLayoutPanel();
            PromptBox = new HintRichTextBox();
            MainViewTab.SuspendLayout();
            curMapContextMenu.SuspendLayout();
            CurMap.SuspendLayout();
            PaintingPanel.SuspendLayout();
            menuStrip.SuspendLayout();
            ToolPanel.SuspendLayout();
            toolTab.SuspendLayout();
            brushPanel.SuspendLayout();
            decoratorPanel.SuspendLayout();
            Buttons.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            RightSideLayOut.SuspendLayout();
            StyleList.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            mapStyle.SuspendLayout();
            SuspendLayout();
            // 
            // MainViewTab
            // 
            MainViewTab.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            MainViewTab.ContextMenuStrip = curMapContextMenu;
            MainViewTab.Controls.Add(CurMap);
            MainViewTab.Location = new Point(0, 25);
            MainViewTab.Name = "MainViewTab";
            MainViewTab.SelectedIndex = 0;
            MainViewTab.Size = new Size(590, 616);
            MainViewTab.TabIndex = 1;
            // 
            // curMapContextMenu
            // 
            curMapContextMenu.Items.AddRange(new ToolStripItem[] { reloadDrawingMenuItem, saveMapMenuItem, clearMapMenuItem, partialRedrawMenuItem, toolStripSeparator1, renameMapMenuItem, closeMapMenuItem });
            curMapContextMenu.Name = "curMapContextMenu";
            curMapContextMenu.Size = new Size(125, 142);
            // 
            // reloadDrawingMenuItem
            // 
            reloadDrawingMenuItem.Name = "reloadDrawingMenuItem";
            reloadDrawingMenuItem.Size = new Size(124, 22);
            reloadDrawingMenuItem.Text = "重载图片";
            reloadDrawingMenuItem.Click += reloadDrawingMenuItem_Click;
            // 
            // saveMapMenuItem
            // 
            saveMapMenuItem.Name = "saveMapMenuItem";
            saveMapMenuItem.Size = new Size(124, 22);
            saveMapMenuItem.Text = "保存地图";
            saveMapMenuItem.Click += saveMapMenuItem_Click;
            // 
            // clearMapMenuItem
            // 
            clearMapMenuItem.Name = "clearMapMenuItem";
            clearMapMenuItem.Size = new Size(124, 22);
            clearMapMenuItem.Text = "清空地图";
            clearMapMenuItem.Click += clearMapMenuItem_Click;
            // 
            // partialRedrawMenuItem
            // 
            partialRedrawMenuItem.Name = "partialRedrawMenuItem";
            partialRedrawMenuItem.Size = new Size(124, 22);
            partialRedrawMenuItem.Text = "局部重绘";
            partialRedrawMenuItem.Click += partialRedrawMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(121, 6);
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
            CurMap.Controls.Add(PaintingPanel);
            CurMap.Location = new Point(4, 26);
            CurMap.Name = "CurMap";
            CurMap.Padding = new Padding(3);
            CurMap.Size = new Size(582, 586);
            CurMap.TabIndex = 0;
            CurMap.Text = "tab1";
            CurMap.UseVisualStyleBackColor = true;
            // 
            // PaintingPanel
            // 
            PaintingPanel.BackColor = Color.WhiteSmoke;
            PaintingPanel.Controls.Add(rulerPainting);
            PaintingPanel.Dock = DockStyle.Fill;
            PaintingPanel.Location = new Point(3, 3);
            PaintingPanel.Name = "PaintingPanel";
            PaintingPanel.Size = new Size(576, 580);
            PaintingPanel.TabIndex = 1;
            // 
            // rulerPainting
            // 
            rulerPainting.BackColor = Color.LightGray;
            rulerPainting.Dock = DockStyle.Fill;
            rulerPainting.Location = new Point(0, 0);
            rulerPainting.Name = "rulerPainting";
            rulerPainting.Size = new Size(576, 580);
            rulerPainting.TabIndex = 0;
            // 
            // FolderStripMenuItem
            // 
            FolderStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { loadImageStripMenuItem });
            FolderStripMenuItem.Name = "FolderStripMenuItem";
            FolderStripMenuItem.Size = new Size(44, 21);
            FolderStripMenuItem.Text = "文件";
            // 
            // loadImageStripMenuItem
            // 
            loadImageStripMenuItem.Name = "loadImageStripMenuItem";
            loadImageStripMenuItem.Size = new Size(124, 22);
            loadImageStripMenuItem.Text = "加载图片";
            // 
            // HelpStripMenuItem
            // 
            HelpStripMenuItem.Name = "HelpStripMenuItem";
            HelpStripMenuItem.Size = new Size(44, 21);
            HelpStripMenuItem.Text = "帮助";
            // 
            // menuStrip
            // 
            menuStrip.Items.AddRange(new ToolStripItem[] { FolderStripMenuItem, HelpStripMenuItem });
            menuStrip.LayoutStyle = ToolStripLayoutStyle.Flow;
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(889, 25);
            menuStrip.TabIndex = 0;
            menuStrip.Text = "menuStrip1";
            // 
            // ToolPanel
            // 
            ToolPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ToolPanel.Controls.Add(toolTab);
            ToolPanel.Location = new Point(4, 620);
            ToolPanel.Name = "ToolPanel";
            ToolPanel.Size = new Size(583, 147);
            ToolPanel.TabIndex = 3;
            // 
            // toolTab
            // 
            toolTab.Controls.Add(brushPanel);
            toolTab.Controls.Add(decoratorPanel);
            toolTab.Dock = DockStyle.Fill;
            toolTab.Location = new Point(0, 0);
            toolTab.Name = "toolTab";
            toolTab.SelectedIndex = 0;
            toolTab.Size = new Size(583, 147);
            toolTab.TabIndex = 0;
            // 
            // brushPanel
            // 
            brushPanel.BackColor = Color.WhiteSmoke;
            brushPanel.Controls.Add(brushList);
            brushPanel.Location = new Point(4, 26);
            brushPanel.Name = "brushPanel";
            brushPanel.Padding = new Padding(3);
            brushPanel.Size = new Size(575, 117);
            brushPanel.TabIndex = 0;
            brushPanel.Text = "笔刷模式";
            // 
            // brushList
            // 
            brushList.Dock = DockStyle.Fill;
            brushList.Location = new Point(3, 3);
            brushList.Name = "brushList";
            brushList.Size = new Size(569, 111);
            brushList.TabIndex = 0;
            // 
            // decoratorPanel
            // 
            decoratorPanel.BackColor = Color.WhiteSmoke;
            decoratorPanel.Controls.Add(decoratorList);
            decoratorPanel.Location = new Point(4, 26);
            decoratorPanel.Name = "decoratorPanel";
            decoratorPanel.Padding = new Padding(3);
            decoratorPanel.Size = new Size(575, 117);
            decoratorPanel.TabIndex = 1;
            decoratorPanel.Text = "装饰模式";
            // 
            // decoratorList
            // 
            decoratorList.Dock = DockStyle.Fill;
            decoratorList.Location = new Point(3, 3);
            decoratorList.Name = "decoratorList";
            decoratorList.Size = new Size(569, 111);
            decoratorList.TabIndex = 0;
            // 
            // Buttons
            // 
            Buttons.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Buttons.AutoScroll = true;
            Buttons.Controls.Add(flowLayoutPanel2);
            Buttons.Controls.Add(paintSize);
            Buttons.Controls.Add(genSize);
            Buttons.Location = new Point(590, 620);
            Buttons.Name = "Buttons";
            Buttons.Size = new Size(295, 150);
            Buttons.TabIndex = 4;
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.Anchor = AnchorStyles.None;
            flowLayoutPanel2.Controls.Add(btnGen);
            flowLayoutPanel2.Controls.Add(btnStop);
            flowLayoutPanel2.Location = new Point(0, 88);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new Size(295, 50);
            flowLayoutPanel2.TabIndex = 4;
            flowLayoutPanel2.WrapContents = false;
            // 
            // btnGen
            // 
            btnGen.Location = new Point(3, 3);
            btnGen.Name = "btnGen";
            btnGen.Size = new Size(75, 23);
            btnGen.TabIndex = 0;
            btnGen.Text = "生成";
            btnGen.UseVisualStyleBackColor = true;
            // 
            // btnStop
            // 
            btnStop.Location = new Point(84, 3);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(87, 23);
            btnStop.TabIndex = 3;
            btnStop.Text = "中止";
            btnStop.UseVisualStyleBackColor = true;
            // 
            // paintSize
            // 
            paintSize.Location = new Point(4, 30);
            paintSize.Name = "paintSize";
            paintSize.Size = new Size(286, 29);
            paintSize.TabIndex = 2;
            // 
            // genSize
            // 
            genSize.Location = new Point(3, 3);
            genSize.Name = "genSize";
            genSize.Size = new Size(286, 29);
            genSize.TabIndex = 1;
            // 
            // RightSideLayOut
            // 
            RightSideLayOut.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            RightSideLayOut.ColumnCount = 1;
            RightSideLayOut.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            RightSideLayOut.Controls.Add(StyleList, 0, 0);
            RightSideLayOut.Controls.Add(PromptBox, 0, 1);
            RightSideLayOut.Location = new Point(590, 54);
            RightSideLayOut.Name = "RightSideLayOut";
            RightSideLayOut.RowCount = 2;
            RightSideLayOut.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            RightSideLayOut.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
            RightSideLayOut.Size = new Size(295, 563);
            RightSideLayOut.TabIndex = 5;
            // 
            // StyleList
            // 
            StyleList.ColumnCount = 1;
            StyleList.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            StyleList.Controls.Add(flowLayoutPanel1, 0, 0);
            StyleList.Controls.Add(mapRefLayout, 0, 1);
            StyleList.Dock = DockStyle.Fill;
            StyleList.Location = new Point(3, 3);
            StyleList.Name = "StyleList";
            StyleList.RowCount = 2;
            StyleList.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            StyleList.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            StyleList.Size = new Size(289, 457);
            StyleList.TabIndex = 2;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(styleLable);
            flowLayoutPanel1.Controls.Add(mapStyle);
            flowLayoutPanel1.Location = new Point(3, 3);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(274, 27);
            flowLayoutPanel1.TabIndex = 0;
            // 
            // styleLable
            // 
            styleLable.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            styleLable.AutoSize = true;
            styleLable.Location = new Point(3, 0);
            styleLable.Name = "styleLable";
            styleLable.Size = new Size(56, 30);
            styleLable.TabIndex = 0;
            styleLable.Text = "地图风格";
            styleLable.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // mapStyle
            // 
            mapStyle.Controls.Add(refTab);
            mapStyle.Controls.Add(templateRefTab);
            mapStyle.Location = new Point(65, 3);
            mapStyle.Name = "mapStyle";
            mapStyle.SelectedIndex = 0;
            mapStyle.Size = new Size(191, 24);
            mapStyle.TabIndex = 1;
            // 
            // refTab
            // 
            refTab.Location = new Point(4, 26);
            refTab.Name = "refTab";
            refTab.Size = new Size(183, 0);
            refTab.TabIndex = 1;
            refTab.Text = "自定义";
            refTab.UseVisualStyleBackColor = true;
            // 
            // templateRefTab
            // 
            templateRefTab.Location = new Point(4, 26);
            templateRefTab.Name = "templateRefTab";
            templateRefTab.Size = new Size(183, 0);
            templateRefTab.TabIndex = 2;
            templateRefTab.Text = "原图";
            templateRefTab.UseVisualStyleBackColor = true;
            // 
            // mapRefLayout
            // 
            mapRefLayout.Dock = DockStyle.Fill;
            mapRefLayout.Location = new Point(3, 43);
            mapRefLayout.Name = "mapRefLayout";
            mapRefLayout.Padding = new Padding(0, 2, 2, 0);
            mapRefLayout.Size = new Size(283, 411);
            mapRefLayout.TabIndex = 1;
            // 
            // PromptBox
            // 
            PromptBox.Dock = DockStyle.Fill;
            PromptBox.ForeColor = SystemColors.GrayText;
            PromptBox.Hint = null;
            PromptBox.Location = new Point(3, 466);
            PromptBox.Name = "PromptBox";
            PromptBox.Size = new Size(289, 94);
            PromptBox.TabIndex = 3;
            PromptBox.Text = "";
            // 
            // MainEditor
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(889, 776);
            Controls.Add(RightSideLayOut);
            Controls.Add(Buttons);
            Controls.Add(ToolPanel);
            Controls.Add(MainViewTab);
            Controls.Add(menuStrip);
            MainMenuStrip = menuStrip;
            Name = "MainEditor";
            Text = "MainEditor";
            Load += MainEditor_Load;
            MainViewTab.ResumeLayout(false);
            curMapContextMenu.ResumeLayout(false);
            CurMap.ResumeLayout(false);
            PaintingPanel.ResumeLayout(false);
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            ToolPanel.ResumeLayout(false);
            toolTab.ResumeLayout(false);
            brushPanel.ResumeLayout(false);
            decoratorPanel.ResumeLayout(false);
            Buttons.ResumeLayout(false);
            flowLayoutPanel2.ResumeLayout(false);
            RightSideLayOut.ResumeLayout(false);
            StyleList.ResumeLayout(false);
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            mapStyle.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TabControl MainViewTab;
        private TabPage CurMap;
        private TabPage brushPanel;
        private TableLayoutPanel MainViewLayout;
        private Panel PaintingPanel;
        private Panel ToolPanel;
        private Panel Buttons;
        private TableLayoutPanel RightSideLayOut;
        private TabControl toolTab;
        private TabPage decoratorPanel;
        private TableLayoutPanel StyleList;
        private FlowLayoutPanel flowLayoutPanel1;
        private Label styleLable;
        private TabControl mapStyle;
        private TabPage refTab;
        private TabPage templateRefTab;
        private Button btnGen;
        private ToolStripMenuItem FolderStripMenuItem;
        private ToolStripMenuItem HelpStripMenuItem;
        private MenuStrip menuStrip;
        private FlowLayoutPanel brushList;
        private FlowLayoutPanel decoratorList;
        private ContextMenuStrip curMapContextMenu;
        private ToolStripMenuItem reloadDrawingMenuItem;
        private ToolStripMenuItem saveMapMenuItem;
        private ToolStripMenuItem clearMapMenuItem;
        private ToolStripMenuItem partialRedrawMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem closeMapMenuItem;
        private ToolStripMenuItem renameMapMenuItem;
        private FlowLayoutPanel mapRefLayout;
        private HintRichTextBox PromptBox;
        private Components.SizeSetting paintSize;
        private Components.SizeSetting genSize;
        private Components.RulerPainting rulerPainting;
        private FlowLayoutPanel flowLayoutPanel2;
        private Button btnStop;
        private ToolStripMenuItem loadImageStripMenuItem;
    }
}