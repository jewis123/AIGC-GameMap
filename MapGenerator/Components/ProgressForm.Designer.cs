namespace MapGenerator.Components
{
    partial class ProgressForm
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
            progressBar = new ProgressBar();
            label = new Label();
            SuspendLayout();
            // 
            // progressBar
            // 
            progressBar.Location = new Point(43, 53);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(701, 23);
            progressBar.TabIndex = 0;
            // 
            // label
            // 
            label.AutoSize = true;
            label.Location = new Point(43, 98);
            label.Name = "label";
            label.Size = new Size(0, 17);
            label.TabIndex = 1;
            // 
            // ProgressForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(788, 158);
            ControlBox = false;
            Controls.Add(label);
            Controls.Add(progressBar);
            Name = "ProgressForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "AIGC进度";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ProgressBar progressBar;
        private Label label;
    }
}