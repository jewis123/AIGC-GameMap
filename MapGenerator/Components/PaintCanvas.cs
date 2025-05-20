using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace MapGenerator.Components
{


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
        private List<Bitmap> maskUndoStack = new List<Bitmap>(); // 遮罩撤销栈
        private float _zoomScale = 1.0f;
        // 装饰物相关属性
        private bool _decoratorMode = false;
        private Image? _currentDecorator = null;
        private int _decoratorSize = 50; // 默认大小
        private readonly int _minDecoratorSize = 10; // 最小大小
        private readonly int _maxDecoratorSize = 256; // 最大大小
        private Point _currentMousePos = Point.Empty;
        private List<DecoratorItem> _decoratorItems = new List<DecoratorItem>();
        private string _decoratorName = "";
        private bool _isLocked = false; // 画布锁定状态
        private int[] originSize = [512, 512];

        public bool DecorateMode => _decoratorMode;
        public bool IsLocked => _isLocked;

        public bool CanChangeCursorIcon { get; private set; } = true;

        public PaintCanvas()
        {
            DoubleBuffered = true;
            AutoScroll = false;
            BackColor = Color.White;
            MouseDown += CanvasControl_MouseDown;
            MouseMove += CanvasControl_MouseMove;
            MouseUp += CanvasControl_MouseUp;
            Paint += CanvasControl_Paint;
            KeyDown += CanvasControl_KeyDown;
            MouseWheel += CanvasControl_MouseWheel;
            MouseEnter += CanvasControl_MouseEnter;
            MouseLeave += CanvasControl_MouseLeave;
            Click += CanvasControl_Click;
            SetBrushColorFromAppSettings();
        }

        // 添加鼠标进入事件处理
        private void CanvasControl_MouseEnter(object? sender, EventArgs e)
        {
            isMouseInControl = true;
            Invalidate();
        }

        // 添加鼠标离开事件处理
        private void CanvasControl_MouseLeave(object? sender, EventArgs e)
        {
            isMouseInControl = false;
            Invalidate();
        }

        private void CanvasControl_MouseDown(object? sender, MouseEventArgs e)
        {
            // 如果是装饰模式，在Click事件中处理
            if (_decoratorMode || _isLocked)
                return;

            isDrawing = true;
            currentPath = new List<Point>();

            // 转换鼠标位置到原始坐标系
            Point originalPoint = new Point(
                (int)(e.Location.X / _zoomScale),
                (int)(e.Location.Y / _zoomScale)
            );

            currentPath.Add(originalPoint);

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
            // 存储实际的鼠标位置用于显示
            lastMousePosition = e.Location;

            // 计算原始坐标系中的位置（反缩放）
            Point originalPoint = new Point(
                (int)(e.Location.X / _zoomScale),
                (int)(e.Location.Y / _zoomScale)
            );

            // 装饰模式下使用缩放后的位置
            if (_decoratorMode)
            {
                _currentMousePos = e.Location;
            }

            if (isDrawing)
            {
                // 使用原始坐标系的位置
                currentPath.Add(originalPoint);
            }

            Invalidate();
            DrawCursor();
        }

        private void CanvasControl_MouseUp(object? sender, MouseEventArgs e)
        {
            if (isDrawing && currentPath.Count > 1)
            {
                Color pathColor = isMaskMode ? maskColor : currentBrushColor;
                // 只在普通绘画模式下记录drawingPaths
                if (!isMaskMode && !_decoratorMode)
                {
                    drawingPaths.Add(new DrawingPathInfo
                    {
                        Path = currentPath,
                        Color = pathColor,
                        BrushSize = brushSize
                    });
                }
                // 如果是在涂抹模式下，在maskLayer上绘制
                if (isMaskMode && maskLayer != null)
                {
                    // 保存撤销快照
                    maskUndoStack.Add(new Bitmap(maskLayer));
                    using (Graphics g = Graphics.FromImage(maskLayer))
                    {
                        // 设置抗锯齿和高质量绘制
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

            // 获取要绘制的区域尺寸
            int drawWidth = (int)(originSize[0] * _zoomScale);
            int drawHeight = (int)(originSize[1] * _zoomScale);

            // 首先绘制背景图像（如果有）
            if (backgroundImage != null)
            {
                e.Graphics.DrawImage(backgroundImage, 0, 0, drawWidth, drawHeight);
            }

            // 绘制所有绘图路径
            foreach (var pathInfo in drawingPaths)
            {
                if (!isMaskMode || pathInfo.Color != maskColor)
                {
                    using (Pen pen = new Pen(pathInfo.Color, pathInfo.BrushSize * _zoomScale))
                    {
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;
                        pen.LineJoin = LineJoin.Round;

                        if (pathInfo.Path.Count > 1)
                        {
                            // 转换路径点为缩放后的坐标
                            Point[] scaledPoints = pathInfo.Path.Select(p => new Point(
                                (int)(p.X * _zoomScale),
                                (int)(p.Y * _zoomScale)
                            )).ToArray();

                            e.Graphics.DrawLines(pen, scaledPoints);
                        }
                    }
                }
            }

            // 如果当前正在绘制，绘制当前路径
            if (isDrawing && currentPath.Count > 1)
            {
                using (Pen pen = new Pen(isMaskMode ? maskColor : currentBrushColor, brushSize * _zoomScale))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    pen.LineJoin = LineJoin.Round;

                    // 转换当前路径点为缩放后的坐标
                    Point[] scaledPoints = currentPath.Select(p => new Point(
                        (int)(p.X * _zoomScale),
                        (int)(p.Y * _zoomScale)
                    )).ToArray();

                    e.Graphics.DrawLines(pen, scaledPoints);
                }
            }

            // 如果有遮罩图层并且处于遮罩模式，半透明显示遮罩
            if (isMaskMode && maskLayer != null)
            {
                ColorMatrix matrix = new ColorMatrix();
                matrix.Matrix33 = maskOpacity;
                using (ImageAttributes attributes = new ImageAttributes())
                {
                    attributes.SetColorMatrix(matrix);
                    Rectangle destRect = new Rectangle(0, 0, drawWidth, drawHeight);
                    Rectangle sourceRect = new Rectangle(0, 0, maskLayer.Width, maskLayer.Height);
                    e.Graphics.DrawImage(maskLayer, destRect,
                        sourceRect.X, sourceRect.Y, sourceRect.Width, sourceRect.Height,
                        GraphicsUnit.Pixel, attributes);
                }
            }

            // 绘制所有装饰项
            foreach (var decorator in _decoratorItems)
            {
                // 计算缩放后的位置和大小
                int scaledX = (int)(decorator.Location.X * _zoomScale) - (int)(decorator.Size * _zoomScale) / 2;
                int scaledY = (int)(decorator.Location.Y * _zoomScale) - (int)(decorator.Size * _zoomScale) / 2;
                int scaledSize = (int)(decorator.Size * _zoomScale);

                e.Graphics.DrawImage(decorator.Image,
                    new Rectangle(scaledX, scaledY, scaledSize, scaledSize),
                    0, 0, decorator.Image.Width, decorator.Image.Height,
                    GraphicsUnit.Pixel);
            }

            // 如果处于装饰模式，绘制当前装饰预览
            if (_decoratorMode && _currentDecorator != null && !_currentMousePos.IsEmpty)
            {
                // 计算预览位置（相对于鼠标位置，无需反缩放，因为鼠标位置已经是屏幕坐标）
                int scaledX = _currentMousePos.X - _decoratorSize / 2;
                int scaledY = _currentMousePos.Y - _decoratorSize / 2;

                // 先生成缩放后的高质量Bitmap
                using (Bitmap scaledBmp = new Bitmap(_decoratorSize, _decoratorSize))
                {
                    using (Graphics gBmp = Graphics.FromImage(scaledBmp))
                    {
                        gBmp.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        gBmp.SmoothingMode = SmoothingMode.AntiAlias;
                        gBmp.Clear(Color.Transparent);
                        gBmp.DrawImage(_currentDecorator, 0, 0, _decoratorSize, _decoratorSize);
                    }

                    ColorMatrix cm = new ColorMatrix();
                    cm.Matrix33 = 0.5f; // 半透明
                    using (ImageAttributes imgAttr = new ImageAttributes())
                    {
                        imgAttr.SetColorMatrix(cm);
                        e.Graphics.DrawImage(
                            scaledBmp,
                            new Rectangle(scaledX, scaledY, _decoratorSize, _decoratorSize),
                            0, 0, _decoratorSize, _decoratorSize,
                            GraphicsUnit.Pixel,
                            imgAttr
                        );
                    }
                }
            }

            // 显示鼠标位置的像素坐标
            if (isMouseInControl)
            {
                float actualX = lastMousePosition.X / _zoomScale;
                float actualY = lastMousePosition.Y / _zoomScale;
                string positionText = $"X: {(int)actualX}, Y: {(int)actualY}";
                e.Graphics.DrawString(positionText, Font, Brushes.Black, lastMousePosition.X + 5, lastMousePosition.Y + 5);
            }

            // UI提示
            if (isMaskMode)
            {
                string modeText = "遮罩模式：绘制遮罩区域";
                e.Graphics.DrawString(modeText, new Font("Arial", 12, FontStyle.Bold), Brushes.Red, new PointF(10, 10));
            }

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

            Invalidate();
        }

        private void CanvasControl_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z) // 按下 Ctrl + Z 进行撤销操作
            {
                if (DecorateMode)
                {
                    if (_decoratorItems.Count > 0)
                        _decoratorItems.RemoveAt(_decoratorItems.Count - 1);
                    Invalidate();
                    return;
                }

                if (isMaskMode && maskUndoStack.Count > 0)
                {
                    // 撤销遮罩
                    maskLayer?.Dispose();
                    maskLayer = new Bitmap(maskUndoStack[maskUndoStack.Count - 1]);
                    maskUndoStack[maskUndoStack.Count - 1].Dispose();
                    maskUndoStack.RemoveAt(maskUndoStack.Count - 1);
                    Invalidate();
                    return;
                }

                if (drawingPaths.Count > 0)
                {
                    // 移除最后添加的绘制路径（最上层）
                    drawingPaths.RemoveAt(drawingPaths.Count - 1);
                    Invalidate();
                }
            }
        }

        private void DrawCursor()
        {
            if (!CanChangeCursorIcon)
            {
                return;
            }

            // 如果是装饰模式，使用系统十字光标
            if (_decoratorMode)
            {
                Cursor.Dispose();
                Cursor = Cursors.Cross;
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
                    using (Pen pen = new Pen(Color.Red, 2))
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

        // 设置画布的尺寸
        public void SetCanvasSize(int width, int height)
        {
            originSize = [width, height];
            Size = new Size(width, height);

            // 如果已经有遮罩层，也要调整大小
            if (isMaskMode && (maskLayer == null || maskLayer.Width != width || maskLayer.Height != height))
            {
                CreateMaskLayer();
            }

            Invalidate();
        }

        private void SetBrushColorFromAppSettings()
        {
            currentBrushColor = AppSettings.GetBrush(currentBrushType).Color;
        }

        public void SetBrushType(int brushType)
        {
            currentBrushType = brushType;
            SetBrushColorFromAppSettings();
            Invalidate();
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
            Invalidate();
        }

        // 设置遮罩模式
        public void SetMaskMode(bool enable)
        {
            isMaskMode = enable;

            // 如果启用遮罩模式，创建遮罩图层
            if (enable && maskLayer == null)
            {
                CreateMaskLayer();
            }
            else
            {
                ClearMaskLayer();
            }

            if (!enable)
            {
                DoResetCursor();
            }

            Invalidate();
        }


        // 获取遮罩模式状态
        public bool IsInMaskMode()
        {
            return isMaskMode;
        }


        // 创建遮罩图层
        private void CreateMaskLayer()
        {
            // 释放旧的遮罩图层
            maskLayer?.Dispose();

            // 使用原始尺寸创建新的遮罩图层
            maskLayer = new Bitmap(originSize[0], originSize[1]);
            using (Graphics g = Graphics.FromImage(maskLayer))
            {
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
                Invalidate();
            }

            foreach (var maskBmp in maskUndoStack)
            {
                maskBmp.Dispose();
            }
            maskUndoStack.Clear();
            maskLayer?.Dispose();
            maskLayer = null;
        }

        // 获取遮罩图层
        public Bitmap GetMaskLayer()
        {
            if (maskLayer == null)
            {
                CreateMaskLayer();
            }

            // 确保maskLayer不为null后再创建新的Bitmap
            if (maskLayer != null)
            {
                var oldMask = maskLayer;
                //消除缩放影响
                using (Graphics g = Graphics.FromImage(maskLayer))
                {
                    // 启用高质量插值模式
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    // 绘制缩放后的遮罩
                    g.DrawImage(oldMask,
                        new Rectangle(0, 0, originSize[0], originSize[1]),
                        new Rectangle(0, 0, oldMask.Width, oldMask.Height),
                        GraphicsUnit.Pixel);
                }
                return new Bitmap(maskLayer); // 返回副本以避免资源问题
            }

            // 如果maskLayer仍然为null，返回一个新的空白Bitmap
            return new Bitmap(Width, Height);
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
                foreach (var maskBmp in maskUndoStack)
                {
                    maskBmp.Dispose();
                }
                maskUndoStack.Clear();
                maskLayer = null;
            }

            // 重置遮罩模式
            isMaskMode = false;

            // 清空装饰
            ClearDecorators();

            // 刷新画布显示
            Invalidate();
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
                Cursor = Cursors.Cross;
                _decoratorName = decoratorName;
            }
            else
            {
                // 重置为普通画笔光标
                brushSizeChanged = true;
                DrawCursor();
            }

            Invalidate();
        }

        // 清空所有装饰
        public void ClearDecorators()
        {
            foreach (var item in _decoratorItems)
            {
                item.Image?.Dispose();
            }
            _decoratorItems.Clear();
            Invalidate();
        }

        // 设置装饰大小
        public void SetDecoratorSize(int size)
        {
            _decoratorSize = Math.Clamp(size, _minDecoratorSize, _maxDecoratorSize);
            Invalidate();
        }

        // 添加装饰到列表
        private void AddDecorator(Point location)
        {
            if (_currentDecorator != null)
            {
                // 存储原始位置（反缩放）
                Point originalLocation = new Point(
                    (int)(location.X / _zoomScale),
                    (int)(location.Y / _zoomScale)
                );

                // 存储原始大小（反缩放）
                int originalSize = (int)(_decoratorSize / _zoomScale);

                // 为显示列表创建装饰项
                var imgDisplayClone = _currentDecorator.Clone() as Image;
                if (imgDisplayClone == null) return;

                DecoratorItem displayItem = new DecoratorItem(
                    imgDisplayClone,
                    originalLocation,  // 使用原始位置
                    originalSize,  // 使用原始大小
                    _decoratorName
                );
                _decoratorItems.Add(displayItem);

                Invalidate();
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
            Invalidate();
        }


        // 解锁画布，允许绘制操作
        public void UnlockCanvas()
        {
            _isLocked = false;
            Invalidate();
        }


        internal IEnumerable<string> GetBrushNames()
        {
            IEnumerable<string> names = new List<string>();
            foreach (var idx in drawedBrush)
            {
                names = names.Append(AppSettings.GetBrush(idx).Name);
            }

            return names;
        }

        internal void CurserIconCanChange(bool flag)
        {
            CanChangeCursorIcon = flag;
            if (!flag)
            {
                DoResetCursor();
            }
        }

        private void DoResetCursor()
        {
            Cursor.Dispose();
            Cursor = Cursors.Default;
        }

        /// <summary>
        /// 设置画布缩放比例
        /// </summary>
        public void SetZoomScale(float scale)
        {
            _zoomScale = scale;

            Size = new Size((int)(originSize[0] * scale), (int)(originSize[1] * scale));

            Invalidate();
        }

        /// <summary>
        /// 获取当前缩放比例
        /// </summary>
        /// <returns>当前缩放比例</returns>
        public float GetZoomScale()
        {
            return _zoomScale;
        }

        // 渲染实际的绘制结果，zoomscale无关
        public Bitmap RenderToBitmap()
        {
            // 创建一个与画布原始尺寸相同大小的位图
            Bitmap bitmap = new Bitmap(originSize[0], originSize[1]);

            // 从位图创建图形上下文
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                // 启用抗锯齿
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                // 使用白色背景清除
                g.Clear(BackColor);

                // 首先绘制背景图像（如果有）
                if (backgroundImage != null)
                {
                    g.DrawImage(backgroundImage, 0, 0, originSize[0], originSize[1]);
                }

                // 使用原始路径绘制所有完成的路径
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

                // 绘制所有装饰（使用原始数据）
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

    }
}