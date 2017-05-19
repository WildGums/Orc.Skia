// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SKPointExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Skia
{
    using SkiaSharp;

#if NETFX_CORE
    using Windows.Foundation;
#else
    using System.Windows;
#endif

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