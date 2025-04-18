namespace MapGenerator.Components
{
    partial class CheckableImageItem
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
            checkBox1 = new CheckBox();
            label = new Label();
            img = new Panel();
            SuspendLayout();
            // 
            // checkBox1
            // 
            checkBox1.AutoCheck = false;
            checkBox1.CausesValidation = false;
            checkBox1.CheckAlign = ContentAlignment.MiddleCenter;
            checkBox1.Enabled = false;
            checkBox1.Location = new Point(0, 0);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(20, 20);
            checkBox1.TabIndex = 0;
            checkBox1.TabStop = false;
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // label
            // 
            label.Dock = DockStyle.Bottom;
            label.Location = new Point(0, 100);
            label.Name = "label";
            label.Size = new Size(100, 17);
            label.TabIndex = 1;
            // 
            // img
            // 
            img.BackgroundImageLayout = ImageLayout.Zoom;
            img.Location = new Point(3, 3);
            img.Name = "img";
            img.Size = new Size(94, 94);
            img.TabIndex = 2;
            // 
            // CheckableImageItem
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            Controls.Add(checkBox1);
            Controls.Add(label);
            Controls.Add(img);
            Name = "CheckableImageItem";
            Size = new Size(100, 117);
            ResumeLayout(false);
        }

        #endregion

        private CheckBox checkBox1;
        private Label label;
        private Panel img;
    }
}
