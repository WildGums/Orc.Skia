// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WriteableBitmapExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Skia
{
    using System;

#if NETFX_CORE
    using Interop;
    using Windows.Foundation;
    using Windows.UI.Xaml.Media.Imaging;
#else
    using System.Windows.Media.Imaging;
#endif

    public static class WriteableBitmapExtensions
    {
#if NETFX_CORE
        internal static IntPtr GetPixels(this WriteableBitmap bitmap)
        {
            var buffer = bitmap.PixelBuffer as IBufferByteAccess;
            if (buffer is null)
            {
                throw new InvalidCastException("Unable to convert WriteableBitmap.PixelBuffer to IBufferByteAccess.");
            }

            IntPtr ptr;
            var hr = buffer.Buffer(out ptr);
            if (hr < 0)
            {
                throw new InvalidCastException("Unable to retrieve pixel address from WriteableBitmap.PixelBuffer.");
            }

            return ptr;
        }
#endif
    }
}
