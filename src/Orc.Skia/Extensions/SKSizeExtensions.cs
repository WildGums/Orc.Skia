// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SKSizeExtensions.cs" company="WildGums">
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

    public static class SKSizeExtensions
    {
        #region Methods
        public static Size ToSize(this SKSizeI size)
        {
            return new Size(size.Width, size.Height);
        }

        public static Size ToSize(this SKSize size)
        {
            return new Size(size.Width, size.Height);
        }
        #endregion
    }
}