// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SKCanvasExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Skia
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
        public static void DrawDiagonalLine(this SKCanvas canvas, Point begin, Point end, SKPaint paint)
        {
            DrawLine(canvas, begin, end, paint);
        }

        public static void DrawHorizontalLine(this SKCanvas canvas, Point begin, Point end, SKPaint paint)
        {
            DrawLine(canvas, begin, end, paint);
        }

        public static void DrawLineThroughPoints(this SKCanvas canvas, IEnumerable<Point> points, SKPaint paint)
        {
            var allPoints = points.ToList();
            if (allPoints.Count > 0)
            {
                var renderingPoints = allPoints.Select(p => new SKPoint((float)p.X, (float)p.Y)).ToArray();

                canvas.DrawPoints(SKPointMode.Polygon, renderingPoints, paint);
            }
        }

        public static void DrawVerticalLine(this SKCanvas canvas, Point begin, Point end, SKPaint paint)
        {
            DrawLine(canvas, begin, end, paint);
        }

        public static void DrawLine(this SKCanvas canvas, Point begin, Point end, SKPaint paint)
        {
            canvas.DrawLine((float)begin.X, (float)begin.Y, (float)end.X, (float)end.Y, paint);
        }
    }
}