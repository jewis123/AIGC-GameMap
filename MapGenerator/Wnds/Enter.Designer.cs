namespace MapGenerator
{
    partial class Enter
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            listView = new ListView();
            btnCreate = new Button();
            tabControl1 = new TabControl();
            creationModeTab = new TabPage();
            changeModeTab = new TabPage();
            btnFlow = new FlowLayoutPanel();
            newBtn = new Button();
            btnOpen = new Button();
            projectList = new ListView();
            tabControl1.SuspendLayout();
            creationModeTab.SuspendLayout();
            changeModeTab.SuspendLayout();
            btnFlow.SuspendLayout();
            SuspendLayout();
            // 
            // listView
            // 
            listView.Activation = ItemActivation.OneClick;
            listView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listView.Location = new Point(100, 78);
            listView.Name = "listView";
            listView.Size = new Size(616, 292);
            listView.TabIndex = 0;
            listView.UseCompatibleStateImageBehavior = false;
            // 
            // btnCreate
            // 
            btnCreate.Anchor = AnchorStyles.Bottom;
            btnCreate.Location = new Point(323, 444);
            btnCreate.Name = "btnCreate";
            btnCreate.Size = new Size(159, 42);
            btnCreate.TabIndex = 1;
            btnCreate.Text = "创建";
            btnCreate.UseVisualStyleBackColor = true;
            btnCreate.Click += btnCreate_Click;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(creationModeTab);
            tabControl1.Controls.Add(changeModeTab);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(819, 555);
            tabControl1.TabIndex = 2;
            // 
            // creationModeTab
            // 
            creationModeTab.Controls.Add(listView);
            creationModeTab.Controls.Add(btnCreate);
            creationModeTab.Location = new Point(4, 26);
            creationModeTab.Name = "creationModeTab";
            creationModeTab.Padding = new Padding(3);
            creationModeTab.Size = new Size(811, 525);
            creationModeTab.TabIndex = 0;
            creationModeTab.Text = "创作模式";
            creationModeTab.UseVisualStyleBackColor = true;
            // 
            // changeModeTab
            // 
            changeModeTab.Controls.Add(btnFlow);
            changeModeTab.Controls.Add(projectList);
            changeModeTab.Location = new Point(4, 26);
            changeModeTab.Name = "changeModeTab";
            changeModeTab.Padding = new Padding(3);
            changeModeTab.Size = new Size(811, 525);
            changeModeTab.TabIndex = 1;
            changeModeTab.Text = "换皮模式";
            changeModeTab.UseVisualStyleBackColor = true;
            // 
            // btnFlow
            // 
            btnFlow.Anchor = AnchorStyles.Bottom;
            btnFlow.AutoSize = true;
            btnFlow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnFlow.Controls.Add(newBtn);
            btnFlow.Controls.Add(btnOpen);
            btnFlow.Location = new Point(302, 488);
            btnFlow.Name = "btnFlow";
            btnFlow.Size = new Size(162, 29);
            btnFlow.TabIndex = 4;
            // 
            // newBtn
            // 
            newBtn.Location = new Point(3, 3);
            newBtn.Name = "newBtn";
            newBtn.Size = new Size(75, 23);
            newBtn.TabIndex = 2;
            newBtn.Text = "新建";
            newBtn.UseVisualStyleBackColor = true;
            newBtn.Click += newBtn_Click;
            // 
            // btnOpen
            // 
            btnOpen.Location = new Point(84, 3);
            btnOpen.Name = "btnOpen";
            btnOpen.Size = new Size(75, 23);
            btnOpen.TabIndex = 3;
            btnOpen.Text = "打开";
            btnOpen.UseVisualStyleBackColor = true;
            btnOpen.Click += btnOpen_Click;
            // 
            // projectList
            // 
            projectList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            projectList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            projectList.Location = new Point(6, 6);
            projectList.MultiSelect = false;
            projectList.Name = "projectList";
            projectList.Size = new Size(799, 465);
            projectList.TabIndex = 0;
            projectList.UseCompatibleStateImageBehavior = false;
            projectList.View = View.Details;
            // 
            // Enter
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(819, 555);
            Controls.Add(tabControl1);
            Name = "Enter";
            Text = "AIGC地图编辑器";
            Load += Enter_Load;
            tabControl1.ResumeLayout(false);
            creationModeTab.ResumeLayout(false);
            changeModeTab.ResumeLayout(false);
            changeModeTab.PerformLayout();
            btnFlow.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ListView listView;
        private Button btnCreate;
        private TabControl tabControl1;
        private TabPage creationModeTab;
        private TabPage changeModeTab;
        private ListView projectList;
        private FlowLayoutPanel btnFlow;
        private Button newBtn;
        private Button btnOpen;
    }
}
