// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SkiaException.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Skia
{
    using System;

    public class SkiaException : Exception
    {
        #region Constructors
        public SkiaException(string message)
            : base(message)
        {
        }
        #endregion
    }
}