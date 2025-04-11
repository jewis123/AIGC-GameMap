namespace MapGenerator.Components
{
    partial class SizeSetting
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
            if(OnSizeChanged !=  null)
                OnSizeChanged = null;

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
            width = new TextBox();
            flowLayoutPanel1 = new FlowLayoutPanel();
            height = new TextBox();
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // label
            // 
            label.AutoSize = true;
            label.Dock = DockStyle.Fill;
            label.Location = new Point(3, 0);
            label.Name = "label";
            label.Size = new Size(43, 29);
            label.TabIndex = 0;
            label.Text = "label1";
            label.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // width
            // 
            width.Dock = DockStyle.Bottom;
            width.Location = new Point(52, 3);
            width.Name = "width";
            width.Size = new Size(100, 23);
            width.TabIndex = 1;
            width.Text = "512";
            width.KeyDown += width_KeyDown;
            width.Leave += width_Leave;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(label);
            flowLayoutPanel1.Controls.Add(width);
            flowLayoutPanel1.Controls.Add(height);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Location = new Point(0, 0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(300, 44);
            flowLayoutPanel1.TabIndex = 2;
            // 
            // height
            // 
            height.Location = new Point(158, 3);
            height.Name = "height";
            height.Size = new Size(100, 23);
            height.TabIndex = 2;
            height.Text = "512";
            height.KeyDown += height_KeyDown;
            height.Leave += height_Leave;
            // 
            // SizeSetting
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(flowLayoutPanel1);
            Name = "SizeSetting";
            Size = new Size(300, 44);
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label label;
        private TextBox width;
        private FlowLayoutPanel flowLayoutPanel1;
        private TextBox height;
    }
}
