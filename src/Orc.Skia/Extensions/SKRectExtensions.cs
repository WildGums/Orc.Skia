namespace Orc.Skia
{
    using SkiaSharp;
    using System.Windows;

    public static class SKRectExtensions
    {
        public static SKRectI ToSKRectI(this Rect rect)
        {
            return new SKRectI((int)rect.Left, (int)rect.Top, (int)rect.Right, (int)rect.Bottom);
        }

        public static SKRect ToSKRect(this Rect rect)
        {
            return new SKRect((float)rect.Left, (float)rect.Top, (float)rect.Right, (float)rect.Bottom);
        }

        public static Rect ToRect(this SKRectI rect)
        {
            return new Rect(rect.Location.ToPoint(), rect.Size.ToSize());
        }

        public static Rect ToRect(this SKRect rect)
        {
            return new Rect(rect.Location.ToPoint(), rect.Size.ToSize());
        }

        public static Int32Rect ToInt32Rect(this SKRectI rect)
        {
            return new Int32Rect(rect.Left, rect.Top, rect.Width, rect.Height);
        }
    }
}
