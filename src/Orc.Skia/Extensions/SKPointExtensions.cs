namespace Orc.Skia
{
    using SkiaSharp;
    using System.Windows;

    public static class SKPointExtensions
    {
        public static SKPoint ToSKPoint(this Point point)
        {
            return new SKPoint((float)point.X, (float)point.Y);
        }

        public static Point ToPoint(this SKPointI point)
        {
            return new Point(point.X, point.Y);
        }

        public static Point ToPoint(this SKPoint point)
        {
            return new Point(point.X, point.Y);
        }
    }
}
