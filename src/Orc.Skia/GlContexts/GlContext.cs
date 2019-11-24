// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlContext.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Skia
{
    using System;
    using SkiaSharp;

    public abstract class GlContext : IDisposable
    {
        #region IDisposable Members
        void IDisposable.Dispose() => Destroy();
        #endregion

        #region Methods
        public abstract void MakeCurrent();
        public abstract void SwapBuffers();
        public abstract void Destroy();
        public abstract GRGlTextureInfo CreateTexture(SKSizeI textureSize);
        public abstract void DestroyTexture(uint texture);
        #endregion
    }
}
