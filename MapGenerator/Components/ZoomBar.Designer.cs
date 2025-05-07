namespace MapGenerator.Components
{
    partial class ZoomBar
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
            zoomTrack = new TrackBar();
            resetBtn = new Button();
            ((System.ComponentModel.ISupportInitialize)zoomTrack).BeginInit();
            SuspendLayout();
            // 
            // label
            // 
            label.AutoSize = true;
            label.Location = new Point(45, 4);
            label.Name = "label";
            label.Size = new Size(57, 17);
            label.TabIndex = 0;
            label.Text = "缩放50%";
            // 
            // zoomTrack
            // 
            zoomTrack.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            zoomTrack.LargeChange = 50;
            zoomTrack.Location = new Point(100, 0);
            zoomTrack.Maximum = 200;
            zoomTrack.Minimum = 30;
            zoomTrack.Name = "zoomTrack";
            zoomTrack.Size = new Size(155, 45);
            zoomTrack.SmallChange = 10;
            zoomTrack.TabIndex = 1;
            zoomTrack.TickFrequency = 10;
            zoomTrack.Value = 100;
            // 
            // resetBtn
            // 
            resetBtn.BackColor = Color.IndianRed;
            resetBtn.Location = new Point(14, 0);
            resetBtn.Name = "resetBtn";
            resetBtn.Size = new Size(25, 25);
            resetBtn.TabIndex = 2;
            resetBtn.Text = "®";
            resetBtn.UseVisualStyleBackColor = false;
            // 
            // ZoomBar
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            Controls.Add(resetBtn);
            Controls.Add(zoomTrack);
            Controls.Add(label);
            MinimumSize = new Size(0, 29);
            Name = "ZoomBar";
            Size = new Size(255, 29);
            ((System.ComponentModel.ISupportInitialize)zoomTrack).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label;
        private TrackBar zoomTrack;
        private Button resetBtn;
    }
}
