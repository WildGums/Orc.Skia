namespace Orc.Skia
{
    using System;
    using SkiaSharp;

    public class CanvasRenderingEventArgs : EventArgs
    {
        public CanvasRenderingEventArgs(SKCanvas canvas)
        {
            Canvas = canvas;
        }

        public SKCanvas Canvas { get; }
    }
}
