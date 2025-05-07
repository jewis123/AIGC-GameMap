
namespace MapGenerator.Components
{
    public partial class IconItemControl_V : UserControl
    {
        public int Idx { private set; get; }
        public string FilePath { get; private set; }
        public IconItemControl_V()
        {
            InitializeComponent();

            // 为子控件绑定 Click 事件
            label.Click += (s, e) => OnClick(e);
            bgColor.Click += (s, e) => OnClick(e);
        }


        public void SetBrushType(Color color, string label, int idx)
        {
            this.bgColor.BackColor = color;
            this.label.Text = label;
            this.Idx = idx;
        }


        public void SetImg(string imgPath, string label, int idx, int[]? width_height = null)
        {
            FilePath = imgPath;
            try
            {
                if (width_height != null)
                {
                    this.label.Width = this.bgColor.Width = width_height[0];
                    this.bgColor.Height = width_height[1];
                }
                else
                {
                    this.bgColor.Width = this.label.Width = this.bgColor.Height = 64;
                }
                this.label.Text = label;
                using (var srcImg = Image.FromFile(imgPath))
                {
                    // 按照_width or height 计算等比缩放后的尺寸
                    var thumb = new Bitmap(this.bgColor.Width, this.bgColor.Height);
                    using (var g = Graphics.FromImage(thumb))
                    {
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.Clear(Color.Transparent);
                        g.DrawImage(srcImg, 0, 0, thumb.Width, thumb.Height);
                    }
                    this.bgColor.BackgroundImage = (Image)thumb.Clone();
                    thumb.Dispose();
                }
                this.Idx = idx;

            }
            catch
            {
                Console.Write("找不到资源: " + imgPath);
            }
        }

        public void SetBackground(bool selected)
        {
            this.BackColor = selected ? Color.Yellow : Color.White;
        }

        public Image GetImage()
        {
            return (Image)this.bgColor.BackgroundImage.Clone();
        }

        public string GetImgName()
        {
            return this.label.Text;
        }
    }
}
