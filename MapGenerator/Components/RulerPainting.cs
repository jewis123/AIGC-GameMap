namespace MapGenerator.Components
{
    public partial class RulerPainting : UserControl
    {
        private const int RulerSize = 30; // 标尺的宽度/高度
        private PaintCanvas canvasControl { set; get; }
        private bool _isSyncingScroll = false;
        private int expandSize = 100;
        private Button exitMaskButton;
        private Button exitDecoratorButton; // 添加退出装饰模式的按钮
        // 新增成员变量用于动态调整标尺刻度间隔
        private int rulerInterval = 5;
        // 定义装饰模式退出事件
        public event EventHandler? DecoratorModeExited;
        public event EventHandler? MaskModeExited;
        public int[] originSize = [512, 512];

        // 保留DecoratorItem类以确保向后兼容性
        public class DecoratorItem
        {
            public Image Image { get; set; }
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
            this.AutoScroll = true;
            canvasControl = new PaintCanvas();
            canvasControl.Dock = DockStyle.None;
            canvasControl.Size = new Size(originSize[0], originSize[1]);
            canvasControl.Location = new Point(RulerSize, RulerSize);
            this.Controls.Add(canvasControl);

            // 创建退出装饰模式按钮
            exitDecoratorButton = new Button();
            exitDecoratorButton.Location = new Point(this.Width - 100, 5);
            exitDecoratorButton.Text = "退出装饰";
            exitDecoratorButton.Size = new Size(80, 25);
            exitDecoratorButton.BackColor = Color.LightCoral;
            exitDecoratorButton.ForeColor = Color.White;
            exitDecoratorButton.FlatStyle = FlatStyle.Flat;
            exitDecoratorButton.Visible = false; // 默认隐藏
            exitDecoratorButton.Click += ExitDecoratorButton_Click;
            this.Controls.Add(exitDecoratorButton);

            // 创建退出装饰模式按钮
            exitMaskButton = new Button();
            exitMaskButton.Location = new Point(this.Width - 100, 5);
            exitMaskButton.Text = "退出遮罩";
            exitMaskButton.Size = new Size(80, 25);
            exitMaskButton.BackColor = Color.LightCoral;
            exitMaskButton.ForeColor = Color.White;
            exitMaskButton.FlatStyle = FlatStyle.Flat;
            exitMaskButton.Visible = false; // 默认隐藏
            exitMaskButton.Click += ExitMaskButton_Click;
            this.Controls.Add(exitMaskButton);

            this.Paint += RulerDrawingControl_Paint;
            this.MouseMove += RulerDrawingControl_MouseMove;
            this.Resize += RulerPainting_Resize; // 添加大小改变事件处理
            this.Scroll += RulerPainting_Scroll;
            this.Click += RulerPainting_Click;
            this.Enter += RulerPainting_Enter;
            this.MouseDown += RulerPainting_MouseDown;
        }

        private void ExitMaskButton_Click(object? sender, EventArgs e)
        {
            SetMaskMode(false);

            MaskModeExited?.Invoke(this, EventArgs.Empty);
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

        // 处理控件大小变化，更新退出按钮位置和刷新标尺
        private void RulerPainting_Resize(object? sender, EventArgs e)
        {
            UpdateExitButtonPosition();
            this.Invalidate(); // 刷新标尺显示
        }

        // 更新退出按钮位置到右上角
        private void UpdateExitButtonPosition()
        {
            exitDecoratorButton.Location = new Point(this.Width - 100, 5);
            exitMaskButton.Location = new Point(this.Width - 100, 5);
            exitDecoratorButton.BringToFront();
            exitMaskButton.BringToFront();
        }

        private void RulerDrawingControl_MouseMove(object? sender, MouseEventArgs e)
        {
            this.Invalidate();
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
            int scrollX = Math.Abs(this.AutoScrollPosition.X);
            g.FillRectangle(Brushes.LightGray, 0, 0, this.Width, RulerSize);
            g.DrawLine(Pens.DarkGray, 0, RulerSize, this.Width, RulerSize);
            g.DrawLine(Pens.DarkGray, RulerSize, 0, RulerSize, RulerSize);

            // 获取当前缩放比例
            float currentZoomScale = GetCanvasZoomScale();

            // 刻度间隔始终用实际像素为单位
            int baseInterval = rulerInterval; // 如10、20、50、100
            // 动态调整刻度间隔以适应缩放后显示效果
            while (baseInterval * currentZoomScale < 30) baseInterval *= 2;
            while (baseInterval * currentZoomScale > 150 && baseInterval > 10) baseInterval /= 2;

            // 计算实际像素范围
            int startValue = (int)Math.Floor((float)scrollX / currentZoomScale / baseInterval) * baseInterval;
            int endValue = (int)Math.Ceiling(((scrollX + this.Width - RulerSize) / currentZoomScale) / baseInterval) * baseInterval;

            for (int x = startValue; x <= endValue; x += baseInterval)
            {
                int posX = (int)(x * currentZoomScale) - scrollX + RulerSize;
                if (posX < RulerSize || posX > this.Width) continue;
                int tickHeight = (x % (baseInterval * 5) == 0) ? 10 : 5;
                g.DrawLine(Pens.Black, posX, 0, posX, tickHeight);
                if (x % (baseInterval * 5) == 0)
                {
                    g.DrawString(x.ToString(), Font, Brushes.Black, new PointF(posX - 10, 12));
                }
            }
        }

        private void DrawVerticalRuler(Graphics g)
        {
            int scrollY = Math.Abs(this.AutoScrollPosition.Y);
            g.FillRectangle(Brushes.LightGray, 0, 0, RulerSize, this.Height);
            g.DrawLine(Pens.DarkGray, RulerSize, 0, RulerSize, this.Height);
            g.DrawLine(Pens.DarkGray, 0, RulerSize, RulerSize, RulerSize);

            // 获取当前缩放比例
            float currentZoomScale = GetCanvasZoomScale();

            // 刻度间隔始终用实际像素为单位
            int baseInterval = rulerInterval;
            while (baseInterval * currentZoomScale < 30) baseInterval *= 2;
            while (baseInterval * currentZoomScale > 150 && baseInterval > 10) baseInterval /= 2;

            int startValue = (int)Math.Floor((float)scrollY / currentZoomScale / baseInterval) * baseInterval;
            int endValue = (int)Math.Ceiling(((scrollY + this.Height - RulerSize) / currentZoomScale) / baseInterval) * baseInterval;

            for (int y = startValue; y <= endValue; y += baseInterval)
            {
                int posY = (int)(y * currentZoomScale) - scrollY + RulerSize;
                if (posY < RulerSize || posY > this.Height) continue;
                int tickWidth = (y % (baseInterval * 5) == 0) ? 10 : 5;
                g.DrawLine(Pens.Black, 0, posY, tickWidth, posY);
                if (y % (baseInterval * 5) == 0)
                {
                    g.DrawString(y.ToString(), Font, Brushes.Black, new PointF(3, posY - 5));
                }
            }
        }

        // 设置绘图区域的尺寸
        public void SetDrawingSize(int width, int height)
        {
            canvasControl.SetCanvasSize(width, height);
            this.AutoScrollMinSize = new Size(width + RulerSize + expandSize, height + RulerSize + expandSize); // 关键，保证横纵滚动条都能出现
            this.PerformLayout();
            this.Refresh(); // 或者 Invalidate + Update
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
            exitMaskButton.Visible = enable;
            if (enable)
            {
                canvasControl.CurserIconCanChange(true);
            }
        }

        // 获取当前是否处于遮罩模式
        public bool IsInMaskMode()
        {
            return canvasControl.IsInMaskMode();
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

        internal void CurserIconCanChange(bool flag)
        {
            canvasControl.CurserIconCanChange(flag);
            if (!flag)
            {
                canvasControl.ResetCursor();
            }
        }

        internal IEnumerable<string> GetBrushNames()
        {
            return canvasControl.GetBrushNames();
        }

        // 检查画布是否被锁定
        public bool IsCanvasLocked => canvasControl.IsLocked;

        public bool IsDecorateMode => canvasControl.DecorateMode;

        // 滚动RulerPainting时同步PaintCanvas
        private void RulerPainting_Scroll(object? sender, ScrollEventArgs e)
        {
            if (_isSyncingScroll) return;
            _isSyncingScroll = true;
            try
            {
                // 只同步内容区（减去标尺偏移）
                var pos = this.AutoScrollPosition;
                // PaintCanvas内容区应与RulerPainting一致
                canvasControl.AutoScrollPosition = new Point(Math.Abs(pos.X), Math.Abs(pos.Y));
                this.Invalidate(); // 滚动时强制重绘标尺
            }
            finally { _isSyncingScroll = false; }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (e is System.Windows.Forms.HandledMouseEventArgs hme)
                    hme.Handled = true;
        }

        // 修复获得焦点时滚动条跳到顶部
        private void RulerPainting_Enter(object? sender, EventArgs e)
        {
            RestoreScrollPosition();
        }

        // 修复鼠标按下时滚动条跳到顶部
        private void RulerPainting_MouseDown(object? sender, MouseEventArgs e)
        {
            RestoreScrollPosition();
        }

        // 修复点击后滚动条跳到顶部的问题
        private void RulerPainting_Click(object? sender, EventArgs e)
        {
            RestoreScrollPosition();
        }

        // 统一滚动条恢复逻辑
        private void RestoreScrollPosition()
        {
            var pos = this.AutoScrollPosition;
            BeginInvoke(new Action(() =>
            {
                this.AutoScrollPosition = new Point(Math.Abs(pos.X), Math.Abs(pos.Y));
                canvasControl.AutoScrollPosition = new Point(Math.Abs(pos.X), Math.Abs(pos.Y));
            }));
        }

        internal void SetZoomScale(int zoomPercentage)
        {
            // 计算缩放因子
            float scale = zoomPercentage / 100f;

            // 计算缩放后的尺寸
            int newWidth = (int)(originSize[0] * scale);
            int newHeight = (int)(originSize[1] * scale);

            // 更新画布尺寸以适应新的缩放比例
            canvasControl.SetZoomScale(scale);

            this.AutoScrollMinSize = new Size(newWidth + RulerSize + expandSize, newHeight + RulerSize + expandSize); // 关键，保证横纵滚动条都能出现

            // 标尺刻度动态调整：根据缩放比例调整刻度间隔
            // 例如：缩放大于100%时，刻度间隔变小；缩放小于100%时，刻度间隔变大
            // 可通过成员变量保存当前刻度间隔
            int baseInterval = 10; // 基础刻度间隔（像素）
            int scaledInterval = (int)(baseInterval * scale);
            scaledInterval = Math.Max(5, scaledInterval); // 最小5像素

            // 将刻度间隔传递给绘制标尺的方法（可通过成员变量或参数）
            this.rulerInterval = scaledInterval;

            // 刷新控件，重绘标尺
            this.Invalidate();
        }

        // 获取画布的当前缩放比例
        public float GetCanvasZoomScale()
        {
            return canvasControl.GetZoomScale();
        }
    }
}
