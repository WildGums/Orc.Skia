namespace Orc.Skia
{
    using System;
    using System.Net.Mime;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Forms;
    using System.Windows.Forms.Integration;
    using System.Windows.Media;
    using System.Windows.Threading;
    using OpenTK;
    using SkiaSharp.Views.Desktop;
    using Color = System.Drawing.Color;
    using HorizontalAlignment = System.Windows.HorizontalAlignment;
    using Point = System.Windows.Point;
    using SKPaintGLSurfaceEventArgs = SkiaSharp.Views.Desktop.SKPaintGLSurfaceEventArgs;

    public sealed class HostFormSkiaCanvas : Canvas, ISkiaElement, IDisposable
    {
#pragma warning disable IDISP002 // Dispose member.
        private readonly SKGLControl _skGlControl;
        private WindowsFormsHost _host;
#pragma warning restore IDISP002 // Dispose member.
        private bool _disposed;


        public HostFormSkiaCanvas()
        {
            _skGlControl = new SKGLControl();
            _skGlControl.BackColor = Color.White;

            _host = new WindowsFormsHost();

            _host.VerticalAlignment = VerticalAlignment.Stretch;
            _host.HorizontalAlignment = HorizontalAlignment.Stretch;

            _skGlControl.MakeCurrent();
            _skGlControl.Dock = DockStyle.Fill;
            _host.Child = _skGlControl;
            _host.Width = 400;
            _host.Height = 588;

            Children.Add(_host);

            _skGlControl.PaintSurface += SkGlControlOnPaintSurface;
        }

        public event EventHandler<CanvasRenderingEventArgs> Rendering;
        public event EventHandler<CanvasRenderingEventArgs> Rendered;
        public void Update()
        {
            //_host.InvalidateVisual();
            _skGlControl.Invalidate();
        //    System.Windows.Forms.Application.DoEvents();
            System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                new Action(delegate { }));

            //_skGlControl.Invalidate();
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
        }

        private void SkGlControlOnPaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
        {
            Rendering?.Invoke(this, new CanvasRenderingEventArgs(e.Surface.Canvas));
            Rendered?.Invoke(this, new CanvasRenderingEventArgs(e.Surface.Canvas));

//            var canvas = e.Surface.Canvas;
//            var leftOffset = 0;

            //            canvas.Clear(Colors.White.ToSKColor());

            //            var colorData = System.Windows.Media.Color.FromArgb(255, 255, 28, 255);
            //#pragma warning disable IDISP001 // Dispose created.
            //#pragma warning disable IDISP003 // Dispose previous before re-assigning.
            //            var solidLinePaint = SKPaintHelper.CreateLinePaint(1, colorData, LineType.Solid);
            //            var dashedLinePaint = SKPaintHelper.CreateLinePaint(1, colorData, LineType.Dashed);
            //            var dottedLinePaint = SKPaintHelper.CreateLinePaint(1, colorData, LineType.Dotted);

            //            canvas.DrawHorizontalLine(new Point(10 + leftOffset, 10), new Point(210, 15), solidLinePaint);
            //            canvas.DrawHorizontalLine(new Point(10 + leftOffset, 30), new Point(210, 35), dashedLinePaint);
            //            canvas.DrawHorizontalLine(new Point(10 + leftOffset, 50), new Point(210, 55), dottedLinePaint);

            //            colorData = System.Windows.Media.Color.FromArgb(255, 0, 0, 255);
            //            canvas.DrawHorizontalLine(new Point(150 + leftOffset, 290), new Point(440, 296), dashedLinePaint);
            //            canvas.DrawHorizontalLine(new Point(150 + leftOffset, 300), new Point(440, 306), dottedLinePaint);

            //            colorData = System.Windows.Media.Color.FromArgb(255, 255, 0, 0);
            //            canvas.DrawVerticalLine(new Point(550 + leftOffset, 90), new Point(556, 380), dashedLinePaint);
            //            canvas.DrawVerticalLine(new Point(560 + leftOffset, 90), new Point(566, 380), dottedLinePaint);

            //            colorData = System.Windows.Media.Color.FromArgb(255, 155, 155, 255);
            //            canvas.DrawVerticalLine(new Point(310 + leftOffset, 10), new Point(315, 210), solidLinePaint);
            //            canvas.DrawVerticalLine(new Point(330 + leftOffset, 10), new Point(335, 210), dashedLinePaint);
            //            canvas.DrawVerticalLine(new Point(350 + leftOffset, 10), new Point(355, 210), dottedLinePaint);

            //            colorData = System.Windows.Media.Color.FromArgb(255, 255, 155, 155);


            //            solidLinePaint = SKPaintHelper.CreateLinePaint(5, colorData, LineType.Solid);

            //            dashedLinePaint = SKPaintHelper.CreateLinePaint(5, colorData, LineType.Dashed);
            //            dottedLinePaint = SKPaintHelper.CreateLinePaint(5, colorData, LineType.Dotted);

            //            var points = new[] { new Point(10, 100), new Point(210, 100), new Point(210, 130), new Point(10, 130), new Point(10, 160), new Point(210, 160) };
            //            canvas.DrawLineThroughPoints(points, solidLinePaint);

            //            points = new[] { new Point(10, 250), new Point(210, 250), new Point(210, 280), new Point(10, 280), new Point(10, 310), new Point(210, 310) };
            //            canvas.DrawLineThroughPoints(points, dashedLinePaint);

            //            points = new[] { new Point(260, 250), new Point(460, 250), new Point(460, 280), new Point(260, 280), new Point(260, 310), new Point(460, 310) };
            //            canvas.DrawLineThroughPoints(points, dottedLinePaint);

            //            colorData = System.Windows.Media.Color.FromArgb(255, 0, 0, 127);

            //            solidLinePaint = SKPaintHelper.CreateLinePaint(2, colorData, LineType.Solid);
            //            dashedLinePaint = SKPaintHelper.CreateLinePaint(2, colorData, LineType.Dashed);
            //            dottedLinePaint = SKPaintHelper.CreateLinePaint(2, colorData, LineType.Dotted);

            //            points = new[] { new Point(40, -10), new Point(40, 70), new Point(590, 70), new Point(590, 490), new Point(610, 490) };
            //            canvas.DrawLineThroughPoints(points, solidLinePaint);

            //            points = new[] { new Point(50, -20), new Point(50, 80), new Point(580, 80), new Point(580, 480), new Point(610, 480) };
            //            canvas.DrawLineThroughPoints(points, dashedLinePaint);

            //            points = new[] { new Point(60, -30), new Point(60, 90), new Point(570, 90), new Point(570, 470), new Point(610, 470) };
            //            canvas.DrawLineThroughPoints(points, dottedLinePaint);

            //            var lineHeight = 20;
            //            var lineWidth = 250;

            //            for (var i = lineHeight; i < 400; i += lineHeight)
            //            {
            //                canvas.DrawText("Text is also part of performance", new Rect(40, i, lineWidth, lineHeight),
            //                    Colors.DarkGreen, 14f);
            //            }

            //#pragma warning restore IDISP001 // Dispose created.
            //#pragma warning restore IDISP003 // Dispose previous before re-assigning.

            //            //         Rendering?.Invoke(this, new CanvasRenderingEventArgs(e.Surface.Canvas));
            //            //    Rendered?.Invoke(this, new CanvasRenderingEventArgs(e.Surface.Canvas));
        }


        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }
    }
}
