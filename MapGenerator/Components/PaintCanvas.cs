using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace MapGenerator.Components
{
    internal static class Win32
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool DestroyIcon(IntPtr hIcon);
    }

    public partial class PaintCanvas : UserControl
    {
        private class DrawingPathInfo
        {
            public List<Point> Path { get; set; } = new List<Point>(); // 初始化为空列表而不是null
            public Color Color { get; set; }
            public int BrushSize { get; set; }
        }

        // 装饰物类，用于记录添加到画布上的装饰
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

        // 将Stack改为List，确保绘制顺序为先进先出，后绘制的覆盖先绘制的
        private List<DrawingPathInfo> drawingPaths = new List<DrawingPathInfo>();
        private List<Point> currentPath = new List<Point>();
        private bool isDrawing = false;
        private Point lastMousePosition;
        private Color currentBrushColor;
        private int brushSize = 30;
        private bool brushSizeChanged = true;
        private int currentBrushType = 0;
        public HashSet<int> drawedBrush = new HashSet<int>();

        public int OffSet;

        private Bitmap? backgroundImage = null;

        // 添加Layer支持
        private Bitmap? maskLayer = null;
        private bool isMaskMode = false;
        private Color maskColor = Color.Pink;
        private float maskOpacity = 0.5f; // 涂抹区域的透明度
        private bool isMouseInControl = false;

        // 装饰物相关属性
        private bool _decoratorMode = false;
        private Image? _currentDecorator = null;
        private int _decoratorSize = 50; // 默认大小
        private readonly int _minDecoratorSize = 10; // 最小大小
        private readonly int _maxDecoratorSize = 100; // 最大大小
        private Point _currentMousePos = Point.Empty;
        private List<DecoratorItem> _decoratorItems = new List<DecoratorItem>();
        private string _decoratorName = "";

        private bool _isLocked = false; // 画布锁定状态

        public bool DecorateMode=>_decoratorMode;
        public bool IsLocked =>_isLocked;
        
        public PaintCanvas()
        {
            this.DoubleBuffered = true;
            this.BackColor = Color.White;
            this.MouseDown += CanvasControl_MouseDown;
            this.MouseMove += CanvasControl_MouseMove;
            this.MouseUp += CanvasControl_MouseUp;
            this.Paint += CanvasControl_Paint;
            this.KeyDown += CanvasControl_KeyDown;
            this.MouseWheel += CanvasControl_MouseWheel;
            this.MouseEnter += CanvasControl_MouseEnter;
            this.MouseLeave += CanvasControl_MouseLeave;
            this.Click += CanvasControl_Click;
            SetBrushColorFromAppSettings();
        }

        // 添加鼠标进入事件处理
        private void CanvasControl_MouseEnter(object? sender, EventArgs e)
        {
            isMouseInControl = true;
            this.Invalidate();
        }

        // 添加鼠标离开事件处理
        private void CanvasControl_MouseLeave(object? sender, EventArgs e)
        {
            isMouseInControl = false;
            this.Invalidate();
        }

        private void CanvasControl_MouseDown(object? sender, MouseEventArgs e)
        {
            // 如果是装饰模式，在Click事件中处理
            if (_decoratorMode || _isLocked)
                return;

            isDrawing = true;
            currentPath = new List<Point>();
            currentPath.Add(e.Location);

            if (!isMaskMode)
            {
                drawedBrush.Add(currentBrushType);
            }
        }

        private void CanvasControl_Click(object? sender, EventArgs e)
        {
            if (_decoratorMode && e is MouseEventArgs mouseEvent && mouseEvent.Button == MouseButtons.Left)
            {
                AddDecorator(mouseEvent.Location);
            }
        }

        private void CanvasControl_MouseMove(object? sender, MouseEventArgs e)
        {
            lastMousePosition = e.Location;

            // 更新装饰模式的鼠标位置
            if (_decoratorMode)
            {
                _currentMousePos = e.Location;
            }

            this.Invalidate();

            if (isDrawing)
            {
                currentPath.Add(e.Location);
                this.Invalidate();
            }

            DrawCursor();
        }

        void DrawCursor()
        {
            // 如果是装饰模式，使用系统十字光标
            if (_decoratorMode)
            {
                Cursor.Dispose();
                this.Cursor = Cursors.Cross;
                return;
            }

            if (!brushSizeChanged)
            {
                return;
            }

            brushSizeChanged = false;

            // 计算圆形光标的直径
            int diameter = brushSize;

            Cursor.Dispose();

            // 创建位图，确保圆形能够完整显示
            using (Bitmap cursorBitmap = new Bitmap(diameter, diameter))
            {
                // 使用Graphics对象绘制位图
                using (Graphics g = Graphics.FromImage(cursorBitmap))
                {
                    // 启用抗锯齿，使光标边缘更平滑
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    // 清除背景为透明
                    g.Clear(Color.Transparent);

                    // 创建一个黑色的画笔
                    using (Pen pen = new Pen(Color.Black, 1))
                    {
                        // 绘制圆形，直径为brushSize
                        g.DrawEllipse(pen, 0, 0, diameter - 1, diameter - 1);
                    }
                }

                // 从位图创建光标并设置为当前窗口的光标
                try
                {
                    IntPtr hIcon = cursorBitmap.GetHicon();
                    Cursor = new Cursor(hIcon);
                }
                catch
                {
                    // 处理异常，例如使用默认光标
                    Cursor = Cursors.Default;
                }
            }
        }


        private void CanvasControl_MouseUp(object? sender, MouseEventArgs e)
        {
            if (isDrawing && currentPath.Count > 1)
            {
                Color pathColor = isMaskMode ? maskColor : currentBrushColor;

                drawingPaths.Add(new DrawingPathInfo
                {
                    Path = currentPath,
                    Color = pathColor,
                    BrushSize = brushSize
                });

                // 如果是在涂抹模式下，在maskLayer上绘制
                if (isMaskMode && maskLayer != null)
                {
                    using (Graphics g = Graphics.FromImage(maskLayer))
                    {
                        // 设置抗锯齿
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                        using (Pen pen = new Pen(Color.White, brushSize))
                        {
                            // 设置线条端点和连接样式
                            pen.StartCap = LineCap.Round;
                            pen.EndCap = LineCap.Round;
                            pen.LineJoin = LineJoin.Round;

                            if (currentPath.Count > 1)
                            {
                                g.DrawLines(pen, currentPath.ToArray());
                            }
                        }
                    }
                }
            }
            isDrawing = false;
        }

        private void CanvasControl_Paint(object? sender, PaintEventArgs e)
        {
            // 启用抗锯齿
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // 首先绘制背景图像（如果有）
            if (backgroundImage != null)
            {
                e.Graphics.DrawImage(backgroundImage, 0, 0, this.Width, this.Height);
            }

            // 绘制所有绘图路径，但如果处于遮罩模式则只绘制非遮罩路径
            foreach (var pathInfo in drawingPaths)
            {
                if (!isMaskMode || pathInfo.Color != maskColor)
                {
                    using (Pen pen = new Pen(pathInfo.Color, pathInfo.BrushSize))
                    {
                        // 设置线条端点和连接样式，以提高平滑度
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;
                        pen.LineJoin = LineJoin.Round;

                        if (pathInfo.Path.Count > 1)
                        {
                            e.Graphics.DrawLines(pen, pathInfo.Path.ToArray());
                        }
                    }
                }
            }

            // 如果当前正在绘制，绘制当前路径
            using (Pen pen = new Pen(isMaskMode ? maskColor : currentBrushColor, brushSize))
            {
                // 设置线条端点和连接样式，以提高平滑度
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                pen.LineJoin = LineJoin.Round;

                if (isDrawing && currentPath.Count > 1)
                {
                    e.Graphics.DrawLines(pen, currentPath.ToArray());
                }
            }

            // 如果有遮罩图层并且处于遮罩模式，半透明显示遮罩
            if (isMaskMode && maskLayer != null)
            {
                // 创建半透明色彩用于显示遮罩
                ColorMatrix matrix = new ColorMatrix();
                matrix.Matrix33 = maskOpacity; // 设置透明度
                using (ImageAttributes attributes = new ImageAttributes())
                {
                    attributes.SetColorMatrix(matrix);
                    // 绘制遮罩图层
                    Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
                    e.Graphics.DrawImage(maskLayer, rect, 0, 0, maskLayer.Width, maskLayer.Height, GraphicsUnit.Pixel, attributes);
                }
            }

            // 绘制所有已添加的装饰项
            foreach (var item in _decoratorItems)
            {
                int x = item.Location.X - item.Size / 2;
                int y = item.Location.Y - item.Size / 2;
                
                e.Graphics.DrawImage(item.Image, 
                    new Rectangle(x, y, item.Size, item.Size),
                    0, 0, item.Image.Width, item.Image.Height,
                    GraphicsUnit.Pixel);
            }
            
            // 如果处于装饰模式，绘制当前装饰预览
            if (_decoratorMode && _currentDecorator != null && !_currentMousePos.IsEmpty)
            {
                int x = _currentMousePos.X - _decoratorSize / 2;
                int y = _currentMousePos.Y - _decoratorSize / 2;
                
                // 半透明绘制
                ColorMatrix cm = new ColorMatrix();
                cm.Matrix33 = 0.5f; // 设置透明度为50%
                ImageAttributes imgAttr = new ImageAttributes();
                imgAttr.SetColorMatrix(cm);
                
                e.Graphics.DrawImage(_currentDecorator,
                    new Rectangle(x, y, _decoratorSize, _decoratorSize),
                    0, 0, _currentDecorator.Width, _currentDecorator.Height,
                    GraphicsUnit.Pixel,
                    imgAttr);
            }

            // 显示鼠标位置的像素坐标
            if (isMouseInControl)
            {
                string positionText = $"X: {lastMousePosition.X}, Y: {lastMousePosition.Y}";
                e.Graphics.DrawString(positionText, Font, Brushes.Black, new PointF(lastMousePosition.X + 5, lastMousePosition.Y + 5));
            }

            // 如果是遮罩模式，显示提示
            if (isMaskMode)
            {
                string modeText = "遮罩模式：选择要重绘的区域";
                e.Graphics.DrawString(modeText, new Font("Arial", 12, FontStyle.Bold), Brushes.Red, new PointF(10, 10));
            }
            
            // 如果是装饰模式，显示提示
            if (_decoratorMode)
            {
                string modeText = "装饰模式：点击添加装饰，滚轮调整大小";
                e.Graphics.DrawString(modeText, new Font("Arial", 12, FontStyle.Bold), Brushes.Blue, new PointF(10, isMaskMode ? 40 : 10));
            }
        }

        private void CanvasControl_MouseWheel(object? sender, MouseEventArgs e)
        {
            if (_decoratorMode)
            {
                // 缩放装饰大小
                int change = e.Delta > 0 ? 5 : -5;
                SetDecoratorSize(_decoratorSize + change);
            }
            else
            {
                if (e.Delta > 0)
                {
                    brushSize = Math.Max(20, Math.Min(brushSize + 5, 100));
                }
                else
                {
                    brushSize = Math.Max(20, Math.Max(brushSize - 5, 1));
                }

                brushSizeChanged = true;

                DrawCursor();
            }
            
            this.Invalidate();
        }

        private void CanvasControl_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z) // 按下 Ctrl + Z 进行撤销操作
            {
                if (drawingPaths.Count > 0)
                {
                    // 移除最后添加的绘制路径（最上层）
                    drawingPaths.RemoveAt(drawingPaths.Count - 1);
                    this.Invalidate();
                }
            }
        }

        // 设置画布的尺寸
        public void SetCanvasSize(int width, int height)
        {
            this.Size = new Size(width, height);

            // 如果已经有遮罩层，也要调整大小
            if (isMaskMode && (maskLayer == null || maskLayer.Width != width || maskLayer.Height != height))
            {
                CreateMaskLayer(width, height);
            }

            this.Invalidate();
        }

        private void SetBrushColorFromAppSettings()
        {
            currentBrushColor = AppSettings.GetBrush(currentBrushType).Color;
        }

        public void SetBrushType(int brushType)
        {
            currentBrushType = brushType;
            SetBrushColorFromAppSettings();
            this.Invalidate();
        }

        // 设置背景图像
        public void SetBackgroundImage(Bitmap image)
        {
            // 保存之前的背景图像的引用
            Bitmap? oldImage = backgroundImage;

            // 设置新的背景图像
            backgroundImage = new Bitmap(image);

            // 释放旧的背景图像资源
            oldImage?.Dispose();

            // 更新绘制状态
            isDrawing = false;
            currentPath.Clear();

            // 刷新画布
            this.Invalidate();
        }

        // 设置遮罩模式
        public void SetMaskMode(bool enable)
        {
            isMaskMode = enable;

            // 如果启用遮罩模式，创建遮罩图层
            if (enable && maskLayer == null)
            {
                CreateMaskLayer(this.Width, this.Height);
            }

            this.Invalidate();
        }

        // 获取遮罩模式状态
        public bool IsInMaskMode()
        {
            return isMaskMode;
        }

        // 创建遮罩图层
        private void CreateMaskLayer(int width, int height)
        {
            // 释放旧的遮罩图层
            maskLayer?.Dispose();

            // 创建新的遮罩图层，使用黑色背景（黑色表示不重绘区域）
            maskLayer = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(maskLayer))
            {
                // 启用抗锯齿
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Black);
            }
        }

        // 清除遮罩图层
        public void ClearMaskLayer()
        {
            if (maskLayer != null)
            {
                using (Graphics g = Graphics.FromImage(maskLayer))
                {
                    // 启用抗锯齿
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.Clear(Color.Black);
                }
                this.Invalidate();
            }
        }

        // 获取遮罩图层
        public Bitmap GetMaskLayer()
        {
            if (maskLayer == null)
            {
                CreateMaskLayer(this.Width, this.Height);
            }

            // 确保maskLayer不为null后再创建新的Bitmap
            if (maskLayer != null)
            {
                if (!isMaskMode)
                    return new Bitmap(maskLayer); // 返回副本以避免资源问题
                else
                    return ConverToMask();
            }

            // 如果maskLayer仍然为null，返回一个新的空白Bitmap
            return new Bitmap(this.Width, this.Height);
        }

        private Bitmap ConverToMask()
        {
            try
            {
                if (maskLayer == null)
                {
                    return new Bitmap(this.Width, this.Height);
                }

                // 创建一个新的RGBA位图
                Bitmap maskBitmap = new Bitmap(maskLayer.Width, maskLayer.Height, PixelFormat.Format32bppArgb);

                // 锁定两个位图的位图数据进行高效处理
                BitmapData sourceData = maskLayer.LockBits(
                    new Rectangle(0, 0, maskLayer.Width, maskLayer.Height),
                    ImageLockMode.ReadOnly,
                    maskLayer.PixelFormat);

                BitmapData targetData = maskBitmap.LockBits(
                    new Rectangle(0, 0, maskBitmap.Width, maskBitmap.Height),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format32bppArgb);

                try
                {
                    unsafe
                    {
                        byte* sourcePtr = (byte*)sourceData.Scan0;
                        byte* targetPtr = (byte*)targetData.Scan0;

                        int sourceBytesPerPixel = Image.GetPixelFormatSize(sourceData.PixelFormat) / 8;
                        int targetBytesPerPixel = 4; // 32bpp = 4 bytes per pixel

                        for (int y = 0; y < maskLayer.Height; y++)
                        {
                            byte* currentSourceLine = sourcePtr + (y * sourceData.Stride);
                            byte* currentTargetLine = targetPtr + (y * targetData.Stride);

                            for (int x = 0; x < maskLayer.Width; x++)
                            {
                                byte* currentSourcePixel = currentSourceLine + (x * sourceBytesPerPixel);
                                byte* currentTargetPixel = currentTargetLine + (x * targetBytesPerPixel);

                                // 获取源位图像素亮度 (R+G+B)/3
                                byte brightness = (byte)((currentSourcePixel[0] + currentSourcePixel[1] + currentSourcePixel[2]) / 3);

                                // 设置目标像素
                                currentTargetPixel[0] = 255; // B
                                currentTargetPixel[1] = 255; // G
                                currentTargetPixel[2] = 255; // R

                                // A通道: 白色区域变为透明(0)，黑色区域保持不透明(255)
                                // 255 - brightness实现了这种反转
                                currentTargetPixel[3] = (byte)(255 - brightness);
                            }
                        }
                    }
                }
                finally
                {
                    // 解锁位图数据
                    maskLayer.UnlockBits(sourceData);
                    maskBitmap.UnlockBits(targetData);
                }

                return maskBitmap;
            }
            catch (Exception ex)
            {
                // 异常情况下返回空白位图
                Console.WriteLine($"转换遮罩时出错: {ex.Message}");
                return new Bitmap(this.Width, this.Height);
            }
        }

        // Render the canvas drawing to a bitmap
        public Bitmap RenderToBitmap()
        {
            // Create a bitmap with the same size as the canvas
            Bitmap bitmap = new Bitmap(this.Width, this.Height);

            // Create a graphics context from the bitmap
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                // 启用抗锯齿
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                // Clear with white background
                g.Clear(this.BackColor);

                // 首先绘制背景图像（如果有）
                if (backgroundImage != null)
                {
                    g.DrawImage(backgroundImage, 0, 0, this.Width, this.Height);
                }

                // Draw all completed paths
                foreach (var pathInfo in drawingPaths)
                {
                    using (Pen pen = new Pen(pathInfo.Color, pathInfo.BrushSize))
                    {
                        // 设置线条端点和连接样式
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;
                        pen.LineJoin = LineJoin.Round;

                        if (pathInfo.Path.Count > 1)
                        {
                            g.DrawLines(pen, pathInfo.Path.ToArray());
                        }
                    }
                }

                // Draw current path if we're in the middle of drawing
                using (Pen pen = new Pen(currentBrushColor, brushSize))
                {
                    // 设置线条端点和连接样式
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    pen.LineJoin = LineJoin.Round;

                    if (isDrawing && currentPath.Count > 1)
                    {
                        g.DrawLines(pen, currentPath.ToArray());
                    }
                }
                
                // 绘制所有装饰
                foreach (var item in _decoratorItems)
                {
                    int x = item.Location.X - item.Size / 2;
                    int y = item.Location.Y - item.Size / 2;
                    
                    g.DrawImage(item.Image, 
                        new Rectangle(x, y, item.Size, item.Size),
                        0, 0, item.Image.Width, item.Image.Height,
                        GraphicsUnit.Pixel);
                }
            }

            return bitmap;
        }

        // 清空所有绘图内容
        public void ClearDrawing()
        {
            // 清空绘图路径集合
            drawingPaths.Clear();

            // 清空当前路径
            currentPath.Clear();

            // 重置绘图状态
            isDrawing = false;

            // 清空已使用的笔刷记录
            drawedBrush.Clear();

            // 清空背景图像
            backgroundImage?.Dispose();
            backgroundImage = null;

            // 清空遮罩图层
            if (maskLayer != null)
            {
                maskLayer.Dispose();
                maskLayer = null;
            }

            // 重置遮罩模式
            isMaskMode = false;
            
            // 清空装饰
            ClearDecorators();

            // 刷新画布显示
            this.Invalidate();
        }
        
        // 装饰模式相关方法
        
        // 启用装饰模式
        public void SetDecoratorMode(bool enabled, Image? decoratorImage = null, string decoratorName = "")
        {
            if (_decoratorMode == enabled)
                return;


            _decoratorMode = enabled;
            _currentDecorator = decoratorImage;
            
            // 设置光标
            if (_decoratorMode && _currentDecorator != null)
            {
                // 使用十字光标
                this.Cursor = Cursors.Cross;
                _decoratorName = decoratorName;
            }
            else
            {
                // 重置为普通画笔光标
                brushSizeChanged = true;
                DrawCursor();
            }

            this.Invalidate();
        }

        // 清空所有装饰
        public void ClearDecorators()
        {
            foreach (var item in _decoratorItems)
            {
                item.Image?.Dispose();
            }
            _decoratorItems.Clear();
            this.Invalidate();
        }

        // 设置装饰大小
        public void SetDecoratorSize(int size)
        {
            _decoratorSize = Math.Clamp(size, _minDecoratorSize, _maxDecoratorSize);
            this.Invalidate();
        }

        // 添加装饰到列表
        private void AddDecorator(Point location)
        {
            if (_currentDecorator != null)
            {
                // 创建新的装饰项并添加到列表
                DecoratorItem item = new DecoratorItem(
                    _currentDecorator.Clone() as Image,
                    location,
                    _decoratorSize,
                    _decoratorName);
                
                _decoratorItems.Add(item);
                this.Invalidate();
            }
        }

        // 获取画布中所有装饰的信息
        public List<DecoratorItem> GetDecorators()
        {
            return _decoratorItems;
        }
        
        // 检查是否处于装饰模式
        public bool IsInDecoratorMode()
        {
            return _decoratorMode;
        }

        // 检查画布是否为空
        public bool IsCanvasEmpty()
        {
            // 检查是否有绘制的路径
            if (drawingPaths.Count > 0)
                return false;
            
            // 检查是否有正在绘制的路径
            if (isDrawing && currentPath.Count > 1)
                return false;
            
            // 检查是否有背景图像
            if (backgroundImage != null)
                return false;
            
            // 检查是否有装饰物
            if (_decoratorItems.Count > 0)
                return false;
            
            // 所有检查都通过，画布为空
            return true;
        }

        // 锁定画布，阻止绘制操作
        public void LockCanvas()
        {
            _isLocked = true;
            this.Invalidate();
        }

        // 解锁画布，允许绘制操作
        public void UnlockCanvas()
        {
            _isLocked = false;
            this.Invalidate();
        }

   
    }
}
