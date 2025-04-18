namespace MapGenerator.Components
{
    public partial class RulerPainting : UserControl
    {
        private const int RulerSize = 30; // 标尺的宽度/高度
        public PaintCanvas canvasControl{private set;get;}
        private Button exitDecoratorButton; // 添加退出装饰模式的按钮

        // 定义装饰模式退出事件
        public event EventHandler? DecoratorModeExited;

        // 保留DecoratorItem类以确保向后兼容性
        public class DecoratorItem
        {
            public Image Image{get;set;}
            public Point Location { get; set; }
            public int Size { get; set; }
            public string Name { get; set; }

            public DecoratorItem(Image img, Point location, int size, string name)
            {
                Image = img;
                Location = location;
                Size = size;
                Name = name;
            }
        }

        public RulerPainting()
        {
            this.DoubleBuffered = true;
            this.BackColor = Color.LightGray;
            canvasControl = new PaintCanvas();
            canvasControl.OffSet = RulerSize;
            canvasControl.Location = new Point(canvasControl.OffSet, canvasControl.OffSet);
            canvasControl.Size = new Size(512, 512);
            this.Controls.Add(canvasControl);
            
            // 创建退出装饰模式按钮
            exitDecoratorButton = new Button();
            exitDecoratorButton.Text = "退出装饰";
            exitDecoratorButton.Size = new Size(80, 25);
            exitDecoratorButton.BackColor = Color.LightCoral;
            exitDecoratorButton.ForeColor = Color.White;
            exitDecoratorButton.FlatStyle = FlatStyle.Flat;
            exitDecoratorButton.Visible = false; // 默认隐藏
            exitDecoratorButton.Click += ExitDecoratorButton_Click;
            this.Controls.Add(exitDecoratorButton);
            
            this.Paint += RulerDrawingControl_Paint;
            this.MouseMove += RulerDrawingControl_MouseMove;
            this.Resize += RulerPainting_Resize; // 添加大小改变事件处理
        }

        // 添加退出按钮事件处理
        private void ExitDecoratorButton_Click(object? sender, EventArgs e)
        {
            // 退出装饰模式
            SetDecoratorMode(false);
            // 清空所有装饰物
            ClearDecorators();
            
            // 触发装饰模式退出事件
            DecoratorModeExited?.Invoke(this, EventArgs.Empty);
        }

        // 处理控件大小变化，更新退出按钮位置
        private void RulerPainting_Resize(object? sender, EventArgs e)
        {
            UpdateExitButtonPosition();
        }

        // 更新退出按钮位置到右上角
        private void UpdateExitButtonPosition()
        {
            if (exitDecoratorButton != null)
            {
                exitDecoratorButton.Location = new Point(
                    this.Width - exitDecoratorButton.Width - 5,
                    RulerSize + 5);
            }
        }

        private void RulerDrawingControl_MouseMove(object? sender, MouseEventArgs e)
        {
            this.Invalidate();

            if (e.X > RulerSize && e.Y > RulerSize)
            {
                this.Invalidate();
            }
        }

        private void RulerDrawingControl_Paint(object? sender, PaintEventArgs e)
        {
            // 绘制上侧标尺
            DrawHorizontalRuler(e.Graphics);
            // 绘制左侧标尺
            DrawVerticalRuler(e.Graphics);
        }

        private void DrawHorizontalRuler(Graphics g)
        {
            g.FillRectangle(Brushes.LightGray, 0, 0, this.Width, RulerSize);
            for (int x = 0; x < this.Width - RulerSize; x += 10)
            {
                int tickHeight = (x % 50 == 0) ? 10 : 5;
                g.DrawLine(Pens.Black, x + RulerSize, 0, x + RulerSize, tickHeight);
                if (x % 50 == 0)
                {
                    g.DrawString(x.ToString(), Font, Brushes.Black, new PointF(x + RulerSize - 3, 12));
                }
            }
        }

        private void DrawVerticalRuler(Graphics g)
        {
            g.FillRectangle(Brushes.LightGray, 0, 0, RulerSize, this.Height);
            for (int y = 0; y < this.Height - RulerSize; y += 10)
            {
                int tickWidth = (y % 50 == 0) ? 10 : 5;
                g.DrawLine(Pens.Black, 0, y + RulerSize, tickWidth, y + RulerSize);
                if (y % 50 == 0)
                {
                    g.DrawString(y.ToString(), Font, Brushes.Black, new PointF(3, y + RulerSize - 5));
                }
            }
        }

        // 设置绘图区域的尺寸
        public void SetDrawingSize(int width, int height)
        {
            canvasControl.SetCanvasSize(width, height);
            this.Size = new Size(width + RulerSize, height + RulerSize);
            this.Invalidate();
        }

        internal void SetBrush(int idx)
        {
            canvasControl.SetBrushType(idx);
        }

        // 获取画布的图像，用于保存
        public Bitmap GetCanvasImage()
        {
            // 使用PaintCanvas的RenderToBitmap方法获取画布内容
            return canvasControl.RenderToBitmap();
        }

        // 获取遮罩图像，用于重绘
        public Bitmap GetMaskImage()
        {
            return canvasControl.GetMaskLayer();
        }

        // 设置遮罩模式
        public void SetMaskMode(bool enable)
        {
            canvasControl.SetMaskMode(enable);
        }

        // 获取当前是否处于遮罩模式
        public bool IsInMaskMode()
        {
            return canvasControl.IsInMaskMode();
        }

        // 清空遮罩
        public void ClearMask()
        {
            canvasControl.ClearMaskLayer();
        }

        // 清空画布
        public void ClearCanvas()
        {
            // 访问PaintCanvas并清空其绘图内容
            canvasControl.ClearDrawing();
            // 刷新显示
            this.Invalidate();
        }

        public void DisplayImageOnCanvas(Bitmap image)
        {
            try
            {
                // 清空当前画布
                this.ClearCanvas();
                
                // 调整画布大小以适应图像
                int width = image.Width;
                int height = image.Height;
                
                this.SetDrawingSize(width, height);
                
                // 创建一个新的位图，与画布大小相同
                Bitmap backgroundBitmap = new Bitmap(width, height);
                
                // 将生成的图像绘制到新位图上
                using (Graphics g = Graphics.FromImage(backgroundBitmap))
                {
                    g.DrawImage(image, 0, 0, width, height);
                }
                
                // 直接将图像设置为背景，避免使用 Graphics.FromHwnd
                canvasControl.SetBackgroundImage(backgroundBitmap);
                
                // 刷新显示
                canvasControl.Invalidate();
                this.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"显示图像时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 以下方法代理到PaintCanvas的对应方法
        
        // 启用装饰模式
        public void SetDecoratorMode(bool enabled, Image? decoratorImage = null, string decoratorName = "")
        {
            canvasControl.SetDecoratorMode(enabled, decoratorImage, decoratorName);
            
            // 更新退出按钮的可见性
            exitDecoratorButton.Visible = enabled;
            if (enabled)
            {
                UpdateExitButtonPosition();
                exitDecoratorButton.BringToFront();
            }
        }

        // 清空所有装饰
        public void ClearDecorators()
        {
            canvasControl.ClearDecorators();
        }

        // 设置装饰大小
        public void SetDecoratorSize(int size)
        {
            canvasControl.SetDecoratorSize(size);
        }

        // 获取画布中所有装饰的信息
        public List<PaintCanvas.DecoratorItem> GetDecorators()
        {
            return canvasControl.GetDecorators();
        }
        
        // 检查画布是否为空
        public bool IsCanvasEmpty()
        {
            return canvasControl.IsCanvasEmpty();
        }
        
        // 锁定画布
        public void LockCanvas()
        {
            canvasControl.LockCanvas();
        }
        
        // 解锁画布
        public void UnlockCanvas()
        {
            canvasControl.UnlockCanvas();
        }
        
        // 检查画布是否被锁定
        public bool IsCanvasLocked => canvasControl.IsLocked;
    
        public bool IsDecorateMode => canvasControl.DecorateMode;
    }
}
