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
            curMapContextMenu = new ContextMenuStrip(components);
            reloadDrawingMenuItem = new ToolStripMenuItem();
            saveMapMenuItem = new ToolStripMenuItem();
            clearMapMenuItem = new ToolStripMenuItem();
            partialRedrawMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            renameMapMenuItem = new ToolStripMenuItem();
            closeMapMenuItem = new ToolStripMenuItem();
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
            PromptBox = new HintRichTextBox();
            mapStyle = new TabControl();
            refTab = new TabPage();
            refTabLayout = new FlowLayoutPanel();
            loraTab = new TabPage();
            modelLayout = new FlowLayoutPanel();
            historyTab = new TabPage();
            historyLayout = new FlowLayoutPanel();
            MainViewTab = new TabControl();
            zoomBar = new Components.ZoomBar();
            curMapContextMenu.SuspendLayout();
            menuStrip.SuspendLayout();
            ToolPanel.SuspendLayout();
            toolTab.SuspendLayout();
            brushPanel.SuspendLayout();
            decoratorPanel.SuspendLayout();
            Buttons.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            RightSideLayOut.SuspendLayout();
            mapStyle.SuspendLayout();
            refTab.SuspendLayout();
            loraTab.SuspendLayout();
            historyTab.SuspendLayout();
            SuspendLayout();
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
            loadImageStripMenuItem.Click += loadImageStripMenuItem_Click;
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
            // paintSize
            // 
            paintSize.Location = new Point(4, 30);
            paintSize.Name = "paintSize";
            paintSize.Size = new Size(286, 29);
            paintSize.TabIndex = 2;
            paintSize.Load += paintSize_Load;
            // 
            // genSize
            // 
            genSize.Location = new Point(3, 3);
            genSize.Name = "genSize";
            genSize.Size = new Size(286, 29);
            genSize.TabIndex = 1;
            genSize.Load += genSize_Load;
            // 
            // RightSideLayOut
            // 
            RightSideLayOut.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            RightSideLayOut.ColumnCount = 1;
            RightSideLayOut.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            RightSideLayOut.Controls.Add(PromptBox, 0, 1);
            RightSideLayOut.Controls.Add(mapStyle, 0, 0);
            RightSideLayOut.Location = new Point(590, 28);
            RightSideLayOut.Name = "RightSideLayOut";
            RightSideLayOut.RowCount = 3;
            RightSideLayOut.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            RightSideLayOut.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
            RightSideLayOut.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            RightSideLayOut.Size = new Size(295, 589);
            RightSideLayOut.TabIndex = 5;
            // 
            // PromptBox
            // 
            PromptBox.Dock = DockStyle.Fill;
            PromptBox.ForeColor = SystemColors.GrayText;
            PromptBox.Hint = null;
            PromptBox.Location = new Point(3, 472);
            PromptBox.Name = "PromptBox";
            PromptBox.Size = new Size(289, 94);
            PromptBox.TabIndex = 3;
            PromptBox.Text = "";
            // 
            // mapStyle
            // 
            mapStyle.Controls.Add(refTab);
            mapStyle.Controls.Add(loraTab);
            mapStyle.Controls.Add(historyTab);
            mapStyle.Location = new Point(3, 3);
            mapStyle.Name = "mapStyle";
            mapStyle.SelectedIndex = 0;
            mapStyle.Size = new Size(289, 463);
            mapStyle.TabIndex = 1;
            // 
            // refTab
            // 
            refTab.Controls.Add(refTabLayout);
            refTab.Location = new Point(4, 26);
            refTab.Name = "refTab";
            refTab.Size = new Size(281, 433);
            refTab.TabIndex = 1;
            refTab.Text = "特征参考";
            refTab.UseVisualStyleBackColor = true;
            // 
            // refTabLayout
            // 
            refTabLayout.Dock = DockStyle.Fill;
            refTabLayout.Location = new Point(0, 0);
            refTabLayout.Name = "refTabLayout";
            refTabLayout.Size = new Size(281, 433);
            refTabLayout.TabIndex = 0;
            // 
            // loraTab
            // 
            loraTab.Controls.Add(modelLayout);
            loraTab.Location = new Point(4, 26);
            loraTab.Name = "loraTab";
            loraTab.Padding = new Padding(3);
            loraTab.Size = new Size(281, 433);
            loraTab.TabIndex = 3;
            loraTab.Text = "绘画模型";
            loraTab.UseVisualStyleBackColor = true;
            // 
            // modelLayout
            // 
            modelLayout.Dock = DockStyle.Fill;
            modelLayout.Location = new Point(3, 3);
            modelLayout.Name = "modelLayout";
            modelLayout.Size = new Size(275, 427);
            modelLayout.TabIndex = 0;
            // 
            // historyTab
            // 
            historyTab.Controls.Add(historyLayout);
            historyTab.Location = new Point(4, 26);
            historyTab.Name = "historyTab";
            historyTab.Size = new Size(281, 433);
            historyTab.TabIndex = 2;
            historyTab.Text = "记录";
            historyTab.UseVisualStyleBackColor = true;
            // 
            // historyLayout
            // 
            historyLayout.Dock = DockStyle.Fill;
            historyLayout.Location = new Point(0, 0);
            historyLayout.Name = "historyLayout";
            historyLayout.Size = new Size(281, 433);
            historyLayout.TabIndex = 0;
            // 
            // MainViewTab
            // 
            MainViewTab.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            MainViewTab.ContextMenuStrip = curMapContextMenu;
            MainViewTab.Location = new Point(0, 28);
            MainViewTab.Name = "MainViewTab";
            MainViewTab.SelectedIndex = 0;
            MainViewTab.Size = new Size(590, 563);
            MainViewTab.TabIndex = 1;
            // 
            // zoomBar
            // 
            zoomBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            zoomBar.BackColor = SystemColors.Control;
            zoomBar.Location = new Point(328, 597);
            zoomBar.MinimumSize = new Size(0, 29);
            zoomBar.Name = "zoomBar";
            zoomBar.Size = new Size(262, 29);
            zoomBar.TabIndex = 6;
            // 
            // MainEditor
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(889, 776);
            Controls.Add(zoomBar);
            Controls.Add(RightSideLayOut);
            Controls.Add(Buttons);
            Controls.Add(ToolPanel);
            Controls.Add(MainViewTab);
            Controls.Add(menuStrip);
            MainMenuStrip = menuStrip;
            Name = "MainEditor";
            Text = "创作模式";
            Load += MainEditor_Load;
            curMapContextMenu.ResumeLayout(false);
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            ToolPanel.ResumeLayout(false);
            toolTab.ResumeLayout(false);
            brushPanel.ResumeLayout(false);
            decoratorPanel.ResumeLayout(false);
            Buttons.ResumeLayout(false);
            flowLayoutPanel2.ResumeLayout(false);
            RightSideLayOut.ResumeLayout(false);
            mapStyle.ResumeLayout(false);
            refTab.ResumeLayout(false);
            loraTab.ResumeLayout(false);
            historyTab.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TabPage brushPanel;
        private TableLayoutPanel MainViewLayout;
        private Panel ToolPanel;
        private Panel Buttons;
        private TableLayoutPanel RightSideLayOut;
        private TabControl toolTab;
        private TabPage decoratorPanel;
        private TabControl mapStyle;
        private TabPage refTab;
        private TabPage historyTab;
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
        private HintRichTextBox PromptBox;
        private Components.SizeSetting paintSize;
        private Components.SizeSetting genSize;
        private FlowLayoutPanel flowLayoutPanel2;
        private Button btnStop;
        private ToolStripMenuItem loadImageStripMenuItem;
        private TabControl MainViewTab;
        private TabPage loraTab;
        private FlowLayoutPanel refTabLayout;
        private FlowLayoutPanel modelLayout;
        private FlowLayoutPanel historyLayout;
        private Components.ZoomBar zoomBar;
    }
}