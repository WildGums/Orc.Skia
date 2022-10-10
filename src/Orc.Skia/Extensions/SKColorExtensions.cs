namespace Orc.Skia
{
    using SkiaSharp;
    using System.Windows.Media;

    public static class SKColorExtensions
    {
        public static SKColor ToSKColor(this Color color)
        {
            return new SKColor(color.R, color.G, color.B, color.A);
        }
    }
}
