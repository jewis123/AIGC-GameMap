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
            loadDrawingMenuItem = new ToolStripMenuItem();
            saveMapMenuItem = new ToolStripMenuItem();
            clearMapMenuItem = new ToolStripMenuItem();
            partialRedrawMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            renameMapMenuItem = new ToolStripMenuItem();
            closeMapMenuItem = new ToolStripMenuItem();
            CurMap = new TabPage();
            MainViewLayout = new TableLayoutPanel();
            PaintingPanel = new Panel();
            rulerPainting = new Components.RulerPainting();
            ToolPanel = new Panel();
            toolTab = new TabControl();
            brushPanel = new TabPage();
            brushList = new FlowLayoutPanel();
            decoratorPanel = new TabPage();
            decoratorList = new FlowLayoutPanel();
            Buttons = new Panel();
            paintSize = new Components.SizeSetting();
            genSize = new Components.SizeSetting();
            btnGen = new Button();
            RightSideLayOut = new TableLayoutPanel();
            StyleList = new TableLayoutPanel();
            flowLayoutPanel1 = new FlowLayoutPanel();
            styleLable = new Label();
            mapStyle = new TabControl();
            allRefTab = new TabPage();
            refTab = new TabPage();
            templateRefTab = new TabPage();
            mapRefLayout = new FlowLayoutPanel();
            PromptBox = new HintRichTextBox();
            文件ToolStripMenuItem = new ToolStripMenuItem();
            编辑ToolStripMenuItem = new ToolStripMenuItem();
            menuStrip = new MenuStrip();
            btnStop = new Button();
            flowLayoutPanel2 = new FlowLayoutPanel();
            MainViewTab.SuspendLayout();
            curMapContextMenu.SuspendLayout();
            CurMap.SuspendLayout();
            MainViewLayout.SuspendLayout();
            PaintingPanel.SuspendLayout();
            ToolPanel.SuspendLayout();
            toolTab.SuspendLayout();
            brushPanel.SuspendLayout();
            decoratorPanel.SuspendLayout();
            Buttons.SuspendLayout();
            RightSideLayOut.SuspendLayout();
            StyleList.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            mapStyle.SuspendLayout();
            menuStrip.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // MainViewTab
            // 
            MainViewTab.ContextMenuStrip = curMapContextMenu;
            MainViewTab.Controls.Add(CurMap);
            MainViewTab.Dock = DockStyle.Fill;
            MainViewTab.Location = new Point(0, 25);
            MainViewTab.Name = "MainViewTab";
            MainViewTab.SelectedIndex = 0;
            MainViewTab.Size = new Size(1106, 662);
            MainViewTab.TabIndex = 1;
            // 
            // curMapContextMenu
            // 
            curMapContextMenu.Items.AddRange(new ToolStripItem[] { loadDrawingMenuItem, saveMapMenuItem, clearMapMenuItem, partialRedrawMenuItem, toolStripSeparator1, renameMapMenuItem, closeMapMenuItem });
            curMapContextMenu.Name = "curMapContextMenu";
            curMapContextMenu.Size = new Size(125, 142);
            // 
            // loadDrawingMenuItem
            // 
            loadDrawingMenuItem.Name = "loadDrawingMenuItem";
            loadDrawingMenuItem.Size = new Size(124, 22);
            loadDrawingMenuItem.Text = "加载涂鸦";
            loadDrawingMenuItem.Click += loadDrawingMenuItem_Click;
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
            CurMap.Controls.Add(MainViewLayout);
            CurMap.Location = new Point(4, 26);
            CurMap.Name = "CurMap";
            CurMap.Padding = new Padding(3);
            CurMap.Size = new Size(1098, 632);
            CurMap.TabIndex = 0;
            CurMap.Text = "tab1";
            CurMap.UseVisualStyleBackColor = true;
            // 
            // MainViewLayout
            // 
            MainViewLayout.ColumnCount = 2;
            MainViewLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 72.47475F));
            MainViewLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 27.5252533F));
            MainViewLayout.Controls.Add(PaintingPanel, 0, 0);
            MainViewLayout.Controls.Add(ToolPanel, 0, 1);
            MainViewLayout.Controls.Add(Buttons, 1, 1);
            MainViewLayout.Controls.Add(RightSideLayOut, 1, 0);
            MainViewLayout.Dock = DockStyle.Fill;
            MainViewLayout.Location = new Point(3, 3);
            MainViewLayout.Name = "MainViewLayout";
            MainViewLayout.RowCount = 2;
            MainViewLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 79.07348F));
            MainViewLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20.9265175F));
            MainViewLayout.Size = new Size(1092, 626);
            MainViewLayout.TabIndex = 0;
            // 
            // PaintingPanel
            // 
            PaintingPanel.BackColor = Color.WhiteSmoke;
            PaintingPanel.Controls.Add(rulerPainting);
            PaintingPanel.Dock = DockStyle.Fill;
            PaintingPanel.Location = new Point(3, 3);
            PaintingPanel.Name = "PaintingPanel";
            PaintingPanel.Size = new Size(785, 488);
            PaintingPanel.TabIndex = 0;
            // 
            // rulerPainting
            // 
            rulerPainting.BackColor = Color.LightGray;
            rulerPainting.Dock = DockStyle.Fill;
            rulerPainting.Location = new Point(0, 0);
            rulerPainting.Name = "rulerPainting";
            rulerPainting.Size = new Size(785, 488);
            rulerPainting.TabIndex = 0;
            // 
            // ToolPanel
            // 
            ToolPanel.Controls.Add(toolTab);
            ToolPanel.Dock = DockStyle.Fill;
            ToolPanel.Location = new Point(3, 497);
            ToolPanel.Name = "ToolPanel";
            ToolPanel.Size = new Size(785, 126);
            ToolPanel.TabIndex = 2;
            // 
            // toolTab
            // 
            toolTab.Controls.Add(brushPanel);
            toolTab.Controls.Add(decoratorPanel);
            toolTab.Dock = DockStyle.Fill;
            toolTab.Location = new Point(0, 0);
            toolTab.Name = "toolTab";
            toolTab.SelectedIndex = 0;
            toolTab.Size = new Size(785, 126);
            toolTab.TabIndex = 0;
            // 
            // brushPanel
            // 
            brushPanel.BackColor = Color.WhiteSmoke;
            brushPanel.Controls.Add(brushList);
            brushPanel.Location = new Point(4, 26);
            brushPanel.Name = "brushPanel";
            brushPanel.Padding = new Padding(3);
            brushPanel.Size = new Size(777, 96);
            brushPanel.TabIndex = 0;
            brushPanel.Text = "笔刷模式";
            // 
            // brushList
            // 
            brushList.Dock = DockStyle.Fill;
            brushList.Location = new Point(3, 3);
            brushList.Name = "brushList";
            brushList.Size = new Size(771, 90);
            brushList.TabIndex = 0;
            // 
            // decoratorPanel
            // 
            decoratorPanel.BackColor = Color.WhiteSmoke;
            decoratorPanel.Controls.Add(decoratorList);
            decoratorPanel.Location = new Point(4, 26);
            decoratorPanel.Name = "decoratorPanel";
            decoratorPanel.Padding = new Padding(3);
            decoratorPanel.Size = new Size(777, 59);
            decoratorPanel.TabIndex = 1;
            decoratorPanel.Text = "装饰模式";
            // 
            // decoratorList
            // 
            decoratorList.Dock = DockStyle.Fill;
            decoratorList.Location = new Point(3, 3);
            decoratorList.Name = "decoratorList";
            decoratorList.Size = new Size(771, 53);
            decoratorList.TabIndex = 0;
            // 
            // Buttons
            // 
            Buttons.AutoScroll = true;
            Buttons.Controls.Add(flowLayoutPanel2);
            Buttons.Controls.Add(paintSize);
            Buttons.Controls.Add(genSize);
            Buttons.Dock = DockStyle.Fill;
            Buttons.Location = new Point(794, 497);
            Buttons.Name = "Buttons";
            Buttons.Size = new Size(295, 126);
            Buttons.TabIndex = 3;
            // 
            // paintSize
            // 
            paintSize.Location = new Point(4, 30);
            paintSize.Name = "paintSize";
            paintSize.Size = new Size(286, 29);
            paintSize.TabIndex = 2;
            paintSize.Load += genSize_Load;
            // 
            // genSize
            // 
            genSize.Location = new Point(3, 3);
            genSize.Name = "genSize";
            genSize.Size = new Size(286, 29);
            genSize.TabIndex = 1;
            genSize.Load += paintSize_Load;
            // 
            // btnGen
            // 
            btnGen.Location = new Point(3, 3);
            btnGen.Name = "btnGen";
            btnGen.Size = new Size(75, 23);
            btnGen.TabIndex = 0;
            btnGen.Text = "生成";
            btnGen.UseVisualStyleBackColor = true;
            btnGen.Click += btnGen_Click;
            // 
            // RightSideLayOut
            // 
            RightSideLayOut.ColumnCount = 1;
            RightSideLayOut.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            RightSideLayOut.Controls.Add(StyleList, 0, 0);
            RightSideLayOut.Controls.Add(PromptBox, 0, 1);
            RightSideLayOut.Dock = DockStyle.Fill;
            RightSideLayOut.Location = new Point(794, 3);
            RightSideLayOut.Name = "RightSideLayOut";
            RightSideLayOut.RowCount = 2;
            RightSideLayOut.RowStyles.Add(new RowStyle(SizeType.Percent, 74.79508F));
            RightSideLayOut.RowStyles.Add(new RowStyle(SizeType.Percent, 25.2049179F));
            RightSideLayOut.Size = new Size(295, 488);
            RightSideLayOut.TabIndex = 4;
            // 
            // StyleList
            // 
            StyleList.ColumnCount = 1;
            StyleList.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            StyleList.Controls.Add(flowLayoutPanel1, 0, 0);
            StyleList.Controls.Add(mapRefLayout, 0, 1);
            StyleList.Dock = DockStyle.Fill;
            StyleList.Location = new Point(3, 3);
            StyleList.Name = "StyleList";
            StyleList.RowCount = 2;
            StyleList.RowStyles.Add(new RowStyle(SizeType.Percent, 10.8910894F));
            StyleList.RowStyles.Add(new RowStyle(SizeType.Percent, 89.10891F));
            StyleList.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            StyleList.Size = new Size(289, 359);
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
            mapStyle.Controls.Add(allRefTab);
            mapStyle.Controls.Add(refTab);
            mapStyle.Controls.Add(templateRefTab);
            mapStyle.Location = new Point(65, 3);
            mapStyle.Name = "mapStyle";
            mapStyle.SelectedIndex = 0;
            mapStyle.Size = new Size(191, 24);
            mapStyle.TabIndex = 1;
            // 
            // allRefTab
            // 
            allRefTab.Location = new Point(4, 26);
            allRefTab.Name = "allRefTab";
            allRefTab.Size = new Size(183, 0);
            allRefTab.TabIndex = 0;
            allRefTab.Text = "全部";
            allRefTab.UseVisualStyleBackColor = true;
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
            templateRefTab.Text = "预设";
            templateRefTab.UseVisualStyleBackColor = true;
            // 
            // mapRefLayout
            // 
            mapRefLayout.Dock = DockStyle.Fill;
            mapRefLayout.Location = new Point(3, 42);
            mapRefLayout.Name = "mapRefLayout";
            mapRefLayout.Size = new Size(283, 314);
            mapRefLayout.TabIndex = 1;
            // 
            // PromptBox
            // 
            PromptBox.Dock = DockStyle.Fill;
            PromptBox.ForeColor = SystemColors.GrayText;
            PromptBox.Hint = null;
            PromptBox.Location = new Point(3, 368);
            PromptBox.Name = "PromptBox";
            PromptBox.Size = new Size(289, 117);
            PromptBox.TabIndex = 3;
            PromptBox.Text = "";
            // 
            // 文件ToolStripMenuItem
            // 
            文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            文件ToolStripMenuItem.Size = new Size(44, 21);
            文件ToolStripMenuItem.Text = "文件";
            // 
            // 编辑ToolStripMenuItem
            // 
            编辑ToolStripMenuItem.Name = "编辑ToolStripMenuItem";
            编辑ToolStripMenuItem.Size = new Size(44, 21);
            编辑ToolStripMenuItem.Text = "帮助";
            // 
            // menuStrip
            // 
            menuStrip.Items.AddRange(new ToolStripItem[] { 文件ToolStripMenuItem, 编辑ToolStripMenuItem });
            menuStrip.LayoutStyle = ToolStripLayoutStyle.Flow;
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(1106, 25);
            menuStrip.TabIndex = 0;
            menuStrip.Text = "menuStrip1";
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
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.Controls.Add(btnGen);
            flowLayoutPanel2.Controls.Add(btnStop);
            flowLayoutPanel2.Dock = DockStyle.Bottom;
            flowLayoutPanel2.Location = new Point(0, 76);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new Size(295, 50);
            flowLayoutPanel2.TabIndex = 4;
            flowLayoutPanel2.WrapContents = false;
            // 
            // MainEditor
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1106, 687);
            Controls.Add(MainViewTab);
            Controls.Add(menuStrip);
            MainMenuStrip = menuStrip;
            Name = "MainEditor";
            Text = "MainEditor";
            Load += MainEditor_Load;
            MainViewTab.ResumeLayout(false);
            curMapContextMenu.ResumeLayout(false);
            CurMap.ResumeLayout(false);
            MainViewLayout.ResumeLayout(false);
            PaintingPanel.ResumeLayout(false);
            ToolPanel.ResumeLayout(false);
            toolTab.ResumeLayout(false);
            brushPanel.ResumeLayout(false);
            decoratorPanel.ResumeLayout(false);
            Buttons.ResumeLayout(false);
            RightSideLayOut.ResumeLayout(false);
            StyleList.ResumeLayout(false);
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            mapStyle.ResumeLayout(false);
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            flowLayoutPanel2.ResumeLayout(false);
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
        private TabPage allRefTab;
        private TabPage refTab;
        private TabPage templateRefTab;
        private Button btnGen;
        private ToolStripMenuItem 文件ToolStripMenuItem;
        private ToolStripMenuItem 编辑ToolStripMenuItem;
        private MenuStrip menuStrip;
        private FlowLayoutPanel brushList;
        private FlowLayoutPanel decoratorList;
        private ContextMenuStrip curMapContextMenu;
        private ToolStripMenuItem loadDrawingMenuItem;
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
    }
}