// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBufferByteAccess.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Skia.Interop
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport]
    [Guid("905a0fef-bc53-11df-8c49-001e4fc686da")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IBufferByteAccess
    {
        long Buffer([Out] out IntPtr value);
    }
}