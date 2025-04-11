namespace MapGenerator.Components
{
    partial class RefImageItem
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
            SuspendLayout();
            // 
            // checkBox1
            // 
            checkBox1.AutoCheck = false;
            checkBox1.CausesValidation = false;
            checkBox1.CheckAlign = ContentAlignment.MiddleCenter;
            checkBox1.Enabled = false;
            checkBox1.Location = new Point(82, 3);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(20, 20);
            checkBox1.TabIndex = 0;
            checkBox1.TabStop = false;
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // RefImageItem
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(checkBox1);
            Name = "RefImageItem";
            Size = new Size(100, 100);
            ResumeLayout(false);
        }

        #endregion

        private CheckBox checkBox1;
    }
}
