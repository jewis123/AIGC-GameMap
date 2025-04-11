namespace MapGenerator.Components
{
    partial class IconItemControl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            label = new Label();
            bgColor = new Panel();
            flowLayoutPanel1 = new FlowLayoutPanel();
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // label
            // 
            label.AutoSize = true;
            label.Dock = DockStyle.Fill;
            label.Font = new Font("Microsoft YaHei UI", 10F);
            label.Location = new Point(59, 0);
            label.Name = "label";
            label.Size = new Size(37, 20);
            label.TabIndex = 0;
            label.Text = "默认";
            label.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // bgColor
            // 
            bgColor.BackColor = Color.MistyRose;
            bgColor.BorderStyle = BorderStyle.FixedSingle;
            bgColor.Location = new Point(3, 3);
            bgColor.Name = "bgColor";
            bgColor.Size = new Size(50, 13);
            bgColor.TabIndex = 1;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoSize = true;
            flowLayoutPanel1.Controls.Add(bgColor);
            flowLayoutPanel1.Controls.Add(label);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Location = new Point(0, 0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(103, 23);
            flowLayoutPanel1.TabIndex = 2;
            // 
            // IconItemControl
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            Controls.Add(flowLayoutPanel1);
            Name = "IconItemControl";
            Size = new Size(103, 23);
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label;
        private Panel bgColor;
        private FlowLayoutPanel flowLayoutPanel1;
    }
}
