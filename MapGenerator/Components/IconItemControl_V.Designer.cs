namespace MapGenerator.Components
{
    partial class IconItemControl_V
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
            flowLayoutPanel1 = new FlowLayoutPanel();
            bgColor = new Panel();
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // label
            // 
            label.Font = new Font("Microsoft YaHei UI", 10F);
            label.Location = new Point(3, 146);
            label.Name = "label";
            label.Size = new Size(86, 20);
            label.TabIndex = 0;
            label.Text = "xcxcxczcxzc";
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoSize = true;
            flowLayoutPanel1.Controls.Add(bgColor);
            flowLayoutPanel1.Controls.Add(label);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel1.Location = new Point(0, 0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(209, 166);
            flowLayoutPanel1.TabIndex = 0;
            // 
            // bgColor
            // 
            bgColor.BackColor = SystemColors.Control;
            bgColor.BackgroundImageLayout = ImageLayout.Zoom;
            bgColor.BorderStyle = BorderStyle.FixedSingle;
            bgColor.Location = new Point(3, 3);
            bgColor.Name = "bgColor";
            bgColor.Size = new Size(203, 140);
            bgColor.TabIndex = 1;
            // 
            // IconItemControl_V
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackColor = SystemColors.Control;
            Controls.Add(flowLayoutPanel1);
            Name = "IconItemControl_V";
            Size = new Size(209, 166);
            flowLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label;
        private FlowLayoutPanel flowLayoutPanel1;
        private Panel bgColor;
    }
}
