namespace Orc.Skia.Example.Views
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Media;
    using SkiaSharp.Views.Desktop;
    using Color = System.Drawing.Color;

    public partial class MainWindow 
    {
        #region Constructors
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnLoaded(EventArgs e)
        {
#pragma warning disable IDISP001 // Dispose created.
            var skGlControl = new SKGLControl();
#pragma warning restore IDISP001 // Dispose created.
            skGlControl.BackColor = Color.White;

            Host.Child = skGlControl;
            skGlControl.PaintSurface += SkGlControlOnPaintSurface;
        }

        private void SkGlControlOnPaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
        {
            e.Surface.Canvas.Clear(Color.White.ToSKColor());

            CanvasTest.RunTests(e.Surface.Canvas, 0, 0);
        }
        #endregion
    }
}
