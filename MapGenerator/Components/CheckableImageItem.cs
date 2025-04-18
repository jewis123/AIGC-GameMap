namespace MapGenerator.Components
{
    public partial class CheckableImageItem : UserControl
    {
        private bool _selected = false;
        public string FilePath { get;private set; }
        public Dictionary<string, object> userData = new Dictionary<string, object>();
        public bool Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
                this.checkBox1.Checked = _selected;
            }
        }

        public CheckableImageItem()
        {
            InitializeComponent();

            this.BackgroundImageLayout = ImageLayout.Zoom;
            
            this.checkBox1.Click += (s,e) => OnClick(e);
            this.img.Click += (s,e) => OnClick(e);
            this.label.Click += (s,e) => OnClick(e);

            this.checkBox1.DoubleClick += (s,e) => OnDoubleClick(e);
            this.img.DoubleClick += (s,e) => OnDoubleClick(e);
            this.label.DoubleClick += (s,e) => OnDoubleClick(e);
        }

        public void SetContent(string label, string imgPath, int[]? size = null){
            FilePath = imgPath;
            this.label.Text = label;
            this.img.BackgroundImage = Image.FromFile(imgPath);
            if(size != null){
                this.img.Width = size[0];
                this.img.Height = size[1];
            }
        }
    }
}
