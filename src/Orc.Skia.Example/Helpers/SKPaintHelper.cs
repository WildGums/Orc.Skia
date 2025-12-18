namespace Orc.Skia.Example;

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
            IsAntialias = true,
        };

        return paint;
    }

    public static SKPaint CreateTextPaint(double width, Color color)
    {
        var paint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = color.ToSKColor(),
            StrokeWidth = (float)width,
            IsAntialias = true,
        };

        return paint;
    }

    public static SKFont CreateFont(double fontSize)
    {
        var paint = new SKFont
        {
            Size = (float)fontSize,
            Subpixel = true,
        };

        return paint;
    }
}
