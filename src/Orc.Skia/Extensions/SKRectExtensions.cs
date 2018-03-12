// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SKRectExtensions.cs" company="WildGums">
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

#if !NETFX_CORE

        public static Int32Rect ToInt32Rect(this SKRectI rect)
        {
            return new Int32Rect(rect.Left, rect.Top, rect.Width, rect.Height);
        }
#endif
    }
}