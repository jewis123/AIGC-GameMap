using System;
using System.Collections;

namespace MapGenerator.Components
{
    public partial class ProgressForm : Form, IProgress<int>
    {
        public ProgressForm()
        {
            InitializeComponent();

            label.Dock = DockStyle.Fill;
            label.TextAlign = ContentAlignment.MiddleLeft;
        }

        protected virtual void OnShown(EventArgs e)
        {
            this.progressBar.Value = 0;
        }


        public void Report(int value)
        {
            switch (value)
            {
                case 10:
                    progressBar.Value = 10;
                    label.Text = "正在上传图片...";
                    break;
                case 15:
                    progressBar.Value = 15;
                    label.Text = "正在上传参考图片...";
                    break;
                case 20:
                    progressBar.Value = 20;
                    label.Text = "正在上传遮罩...";
                    break;
                case 30:
                    progressBar.Value = 30;
                    label.Text = "正在准备工作流...";
                    break;
                case 40:
                    progressBar.Value = 40;
                    label.Text = "正在全速生成...";
                    break;
                case 100:
                    progressBar.Value = 100;
                    label.Text = "图片生成完成！";
                    break;
            }
            this.Invalidate();
        }

        public void SetProgress(int percent, string text)
        {
            progressBar.Value = Math.Min(percent, 100);
            this.label.Text = text;
        }
    }
}
