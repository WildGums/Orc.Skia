namespace Orc.Skia;

using System;
using SkiaSharp;
using System.Windows.Media;

public static class SKPaintHelper
{
    public static SKFont CreateTextPaint(double fontSize, double width, Color color, SKTextAlign textAlign = SKTextAlign.Left)
    {
        var paint = new SKFont
        {
            Style = SKPaintStyle.Fill,
            Color = color.ToSKColor(),
            StrokeWidth = (float)width,
            TextSize = (float)fontSize,
            TextAlign = textAlign,
            Subpixel = true,
            Typeface = new SKTypeface
            {
                FontStyle = new SKFontStyle
                {
                    Slant = new 
                }
            }
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
