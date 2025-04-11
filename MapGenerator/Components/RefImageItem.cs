namespace MapGenerator.Components
{
    public partial class RefImageItem : UserControl
    {
        public string FilePath{get;set;}
        public int Index { get; set; }

        public RefImageItem()
        {
            InitializeComponent();

            this.BackgroundImageLayout = ImageLayout.Zoom;
        }

        internal void SetCheckBox(bool v)
        {
            this.checkBox1.Checked = v;
        }
    }
}
