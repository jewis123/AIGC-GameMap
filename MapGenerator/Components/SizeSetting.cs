namespace MapGenerator.Components
{
    public partial class SizeSetting : UserControl
    {
        public class SizeSettingEventArgs : EventArgs
        {
            public int iWidthVal { get; set; }
            public int iHeightVal { get; set; }

            public SizeSettingEventArgs(int iWidthVal, int iHeightVal)
            {
                this.iWidthVal = iWidthVal;
                this.iHeightVal = iHeightVal;
            }
        }

        private int validWidth = 512;
        private int validHeight = 512;

        public int[] PixelSize
        {   
            get
            {
                return [validWidth, validHeight];
            }
        }

        public new event EventHandler<SizeSettingEventArgs> OnSizeChanged;

        public SizeSetting()
        {
            InitializeComponent();
        }

        public void SetLabel(string label)
        {
            this.label.Text = label;
        }

        private void width_Leave(object sender, EventArgs e)
        {
            if (int.TryParse(this.width.Text, out int width))
            {
                validWidth = width;

                OnSizeChanged?.Invoke(this, new SizeSettingEventArgs(validWidth, validHeight));
            }
        }

        private void height_Leave(object sender, EventArgs e)
        {
            if (int.TryParse(this.height.Text, out int height))
            {
                validHeight = height;

                OnSizeChanged?.Invoke(this, new SizeSettingEventArgs(validWidth, validHeight));
            }
        }

        private void width_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Enter) // 按下 Ctrl + Z 进行撤销操作
            {
                if (int.TryParse(this.width.Text, out int width))
                {
                    validWidth = width;

                    OnSizeChanged?.Invoke(this, new SizeSettingEventArgs(validWidth, validHeight));
                }
            }
        }

        private void height_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Enter) // 按下 Ctrl + Z 进行撤销操作
            {
                if (int.TryParse(this.height.Text, out int height))
                {
                    validHeight = height;

                    OnSizeChanged?.Invoke(this, new SizeSettingEventArgs(validWidth, validHeight));
                }
            }
        }
    
        public void GetSize(out int width, out int height)
        {
            width = validWidth;
            height = validHeight;
        }
    }
}
