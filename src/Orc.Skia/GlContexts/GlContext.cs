namespace Orc.Skia
{
    using System;
    using SkiaSharp;

#pragma warning disable IDISP025 // Class with no virtual dispose method should be sealed.
    public abstract class GlContext : IDisposable
#pragma warning restore IDISP025 // Class with no virtual dispose method should be sealed.
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
