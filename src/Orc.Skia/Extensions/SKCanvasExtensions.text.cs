// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SKCanvasExtensions.text.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Skia
{
    using System;
    using System.Collections.Generic;
    using SkiaSharp;

#if NETFX_CORE
    using Windows.Foundation;
    using Windows.UI;
#else
    using System.Windows;
    using System.Windows.Media;
#endif

    public static partial class SKCanvasExtensions
    {
        public const float DefaultFontSize = 14f;
        public const float DefaultFontWidth = 4f;

        public static Rect MeasureTextBounds(this SKCanvas canvas, string text, Color color, float fontSize = DefaultFontSize, double width = DefaultFontWidth, SKTextAlign textAlign = SKTextAlign.Left)
        {
            var paint = SKPaintHelper.CreateTextPaint(fontSize, width, color, textAlign);

            var bounds = SKRect.Empty;
            paint.MeasureText(text, ref bounds);

            return bounds.ToRect();
        }

        public static void DrawText(this SKCanvas canvas, string line, Rect rect, Color color, float fontSize = DefaultFontSize, 
            double width = DefaultFontWidth, double? lineSpacing = null, SKTextAlign textAlign = SKTextAlign.Left, bool clip = true)
        {
            DrawText(canvas, line.SplitLines(), rect, color, fontSize, width, lineSpacing, textAlign);
        }

        public static void DrawText(this SKCanvas canvas, string[] lines, Rect rect, Color color, float fontSize = DefaultFontSize, 
            double width = DefaultFontWidth, double? lineSpacing = null, SKTextAlign textAlign = SKTextAlign.Left, bool clip = true)
        {
            if (lines == null)
            {
                return;
            }

            var paint = SKPaintHelper.CreateTextPaint(fontSize, width, color, textAlign);
            var finalLineSpacing = lineSpacing ?? paint.FontSpacing;
            var finalCharSpacing = fontSize / 8f;

            var begin = rect.GetTopLeft();
            var currentPoint = begin;

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var lineHeight = 0f;

                var lineBounds = SKRect.Empty;
                paint.MeasureText(line, ref lineBounds);

                if (clip && begin.Y + rect.Height < currentPoint.Y + lineBounds.Height)
                {
                    break;
                }

                // To get the right spacing, duplicate the spaces
                line = line.Replace(" ", "  ");

                for (var j = 0; j < line.Length; j++)
                {
                    var charString = line[j].ToString();

                    var bounds = SKRect.Empty;
                    paint.MeasureText(charString, ref bounds);

                    if (clip && begin.X + rect.Width < currentPoint.X + bounds.Width)
                    {
                        break;
                    }

                    if (bounds.Height > lineHeight)
                    {
                        lineHeight = bounds.Height;
                    }

                    canvas.DrawText(charString, currentPoint.X, currentPoint.Y, paint);

                    currentPoint.X += bounds.Width;

                    if (charString == " ")
                    {
                        currentPoint.X += finalCharSpacing;
                    }
                }

                currentPoint.X = begin.X;
                currentPoint.Y = currentPoint.Y + lineHeight + finalLineSpacing;
            }
        }
    }
}
