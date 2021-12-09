namespace Orc.Skia
{
    using System;
    using System.Windows;
    using SkiaSharp;
    using SkiaSharp.Views.Desktop;

    /// <summary>
    /// Interaction logic for GLSkiaCanvas.xaml
    /// </summary>
    public partial class GLSkiaCanvas
    {
        private SKSurface _surface;
        private GRContext _grContext;
        private SKSize _screenCanvasSize;
        private SKImageInfo _info;

        private readonly WglContext _glContext = new();

        #region Constructors
        public GLSkiaCanvas()
        {
            InitializeComponent();

            Loaded += (s, e) => { // only at this point the control is ready
                Window.GetWindow(this) // get the parent window
                    .Closing += (s1, e1) => Dispose(); //disposing logic here
            };

            _glContext.MakeCurrent();
        }
        #endregion

        #region Methods
        private void OnPaintCanvas(object sender, SKPaintSurfaceEventArgs e)
        {
            _info = e.Info;

            OnPaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }

        public void Update()
        {
            DrawOffscreen(_surface.Canvas, _info.Width, _info.Height);
        }

        private void OnPaintSurface(SKCanvas canvas, int width, int height)
        {
            var canvasSize = new SKSize(width, height);

            // check if we need to recreate the off-screen surface
            if (_screenCanvasSize != canvasSize)
            {
                _surface?.Dispose();
                _grContext?.Dispose();

                _grContext = GRContext.CreateGl();
                _surface = SKSurface.Create(_grContext, true, new SKImageInfo(width, height));

                _screenCanvasSize = canvasSize;
            }

            // draw onto off-screen gl context
            DrawOffscreen(_surface.Canvas, width, height);

            // draw offscreen surface onto screen
            canvas.DrawSurface(_surface, new SKPoint(0f, 0f));
        }

        private void Dispose()
        {
            _surface?.Dispose();
            _grContext?.Dispose();
            _glContext.Destroy();
        }

        private void DrawOffscreen(SKCanvas canvas, int width, int height)
        {
            Rendering?.Invoke(this, new CanvasRenderingEventArgs(canvas));

            //canvas.Clear(SKColors.Gray.WithAlpha(0x80));

            //// will be more expensive in the real world
            //using (var paint = new SKPaint())
            //{
            //    paint.TextSize = 64.0f;
            //    paint.IsAntialias = true;
            //    paint.Color = 0xFF4281A4;
            //    paint.IsStroke = false;
            //    canvas.DrawText("SkiaSharp", width / 2f, 64.0f, paint);
            //}
        }

        public event EventHandler<CanvasRenderingEventArgs> Rendering;
        #endregion
    }
}
