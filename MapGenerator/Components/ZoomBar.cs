namespace MapGenerator.Components
{
    public partial class ZoomBar : UserControl
    {
        public event EventHandler<int>? OnZoomChanged;
        public event EventHandler? OnZoomReset;
        public string LabelText
        {
            get => label.Text;
            set => label.Text = value;
        }

        public ZoomBar()
        {
            InitializeComponent();

            zoomTrack.ValueChanged += (sender, e) =>
            {
                OnZoomChanged?.Invoke(this, zoomTrack.Value);
                this.label.Text = $"{LabelText}{zoomTrack.Value}%";
            };

            // 禁止鼠标滚轮改变值
            zoomTrack.MouseWheel += (s, e) =>
            {
                if (e is System.Windows.Forms.HandledMouseEventArgs hme)
                    hme.Handled = true;
            };

            resetBtn.Click += (s, e) =>
            {
                OnZoomReset?.Invoke(this, EventArgs.Empty);
                this.label.Text = $"{LabelText}100%";
            };

            this.label.Text = $"{LabelText}100%";
        }

        internal void SetZoomValue(float v)
        {
            zoomTrack.Value = (int)(v * 100);
            this.label.Text = $"{LabelText}{zoomTrack.Value}%";
        }
    }
}
