namespace Orc.Skia
{
    using SkiaSharp;
    using System.Windows;

    public static class SKSizeExtensions
    {
        public static Size ToSize(this SKSizeI size)
        {
            return new Size(size.Width, size.Height);
        }

        public static Size ToSize(this SKSize size)
        {
            return new Size(size.Width, size.Height);
        }
    }
}
