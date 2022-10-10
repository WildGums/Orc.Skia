namespace Orc.Skia
{
    using System;
    using SkiaSharp;
    using System.Windows.Media;

    public static class SKPaintHelper
    {
        public static SKPaint CreateTextPaint(double fontSize, double width, Color color, SKTextAlign textAlign = SKTextAlign.Left)
        {
            var paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = color.ToSKColor(),
                StrokeWidth = (float)width,
                TextSize = (float)fontSize,
                TextAlign = textAlign,
                SubpixelText = true,
                IsAntialias = true,
            };

            return paint;
        }

        public static SKPaint CreateLinePaint(double width, Color color, LineType lineType = LineType.Solid)
        {
            var paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = color.ToSKColor(),
                StrokeWidth = (float)width,
            };

            var widthAsFloat = (float)width;

            switch (lineType)
            {
                case LineType.Solid:
                    break;

                case LineType.Dashed:
                    paint.PathEffect = SKPathEffect.CreateDash(new[] { widthAsFloat * 4, widthAsFloat * 2 }, widthAsFloat * 2);
                    break;

                case LineType.Dotted:
                    paint.PathEffect = SKPathEffect.CreateDash(new[] { widthAsFloat, widthAsFloat }, widthAsFloat);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(lineType), lineType, null);
            }

            return paint;
        }
    }
}
