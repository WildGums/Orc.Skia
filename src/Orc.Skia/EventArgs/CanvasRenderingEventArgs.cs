// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CanvasRenderingEventArgs.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Skia
{
    using System;
    using System.ComponentModel;
    using SkiaSharp;

    public class CanvasRenderingEventArgs : EventArgs
    {
        #region Constructors
        public CanvasRenderingEventArgs(SKCanvas canvas)
        {
            Canvas = canvas;
        }
        #endregion

        #region Properties
        public SKCanvas Canvas { get; }
        #endregion
    }
}