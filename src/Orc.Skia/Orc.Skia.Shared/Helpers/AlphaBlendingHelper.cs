// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AlphaBlendingHelper.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Skia
{
    using Coloring;

    /// <summary>
    /// Alpha blending helper for rendering engine.
    /// </summary>
    public static class AlphaBlendingHelper
    {
        #region Constants
        /// <summary>
        /// Const for fully opaque color alpha channel.
        /// </summary>
        public const byte FullyOpaqueAlpha = 0xff;

        /// <summary>
        /// Const for fully transparent color alpha channel.
        /// </summary>
        public const byte FullyTransparentAlpha = 0x00;

        /// <summary>
        /// Filter alpha blending mask Should be the same as <see cref="IntColors.TransparentSoftGray"/>.
        /// </summary>
        private const uint HighlightFilterColor = 0x1F000000;
        #endregion

        #region Methods
        /// <summary>
        /// Grey filter of level 1.
        /// </summary>
        /// <param name="dstPixel">Destination pixel.</param>
        /// <returns></returns>
        public static int ApplyFilter(int dstPixel)
        {
            var destPixel = (uint) dstPixel;

            var dstAlpha = (destPixel >> 24);
            if ((dstAlpha == FullyTransparentAlpha))
            {
                destPixel = HighlightFilterColor;
            }
            else
            {
                const int invAlpha = 0xE0;

                // extract 16-bit pairs
                var dstRB = destPixel & 0x00ff00ff;
                var dstAG = destPixel & 0xff00ff00;

                // calculate alpha blend
                dstRB = (invAlpha*dstRB >> 8) & 0x00ff00ff;
                dstAG = HighlightFilterColor + (invAlpha*(dstAG >> 8)) & 0xff00ff00;

                // create new pixel
                destPixel = dstAG + dstRB;
            }

            dstPixel = (int) destPixel;
            return dstPixel;
        }

        /// <summary>
        /// Grey filter of given level ( <paramref name="depth"/>).
        /// </summary>
        /// <param name="dstPixel">Destination pixel.</param>
        /// <param name="depth">Depth of filter.</param>
        /// <returns></returns>
        public static int ApplyFilter(int dstPixel, int depth)
        {
            var destPixel = (uint) dstPixel;

            var dstAlpha = (destPixel >> 24);
            if ((dstAlpha == FullyTransparentAlpha))
            {
                depth--;
                destPixel = HighlightFilterColor;
            }

            if (depth > 0)
            {
                const int invAlpha = 0xE0;

                // extract 16-bit pairs
                var dstRB = destPixel & 0x00ff00ff;
                var dstAG = destPixel & 0xff00ff00;

                // calculate alpha blend for depth of filter
                while (depth-- > 0)
                {
                    dstRB = ((invAlpha*dstRB) >> 8) & 0x00ff00ff;
                    dstAG = (HighlightFilterColor + (invAlpha*(dstAG >> 8)) & 0xff00ff00);
                }

                // create new pixel
                destPixel = dstAG + dstRB;
            }

            dstPixel = (int) destPixel;
            return dstPixel;
        }

        /// <summary>
        /// Blends 2 pixels using alpha blending.
        /// </summary>
        /// <param name="dstPixel">Destination pixel.</param>
        /// <param name="srcPixel">Source pixel (from another bitmap or mask).</param>
        /// <returns></returns>
        public static int BlendPixels(int dstPixel, int srcPixel)
        {
            var sourcePixel = (uint) srcPixel;
            var destPixel = (uint) dstPixel;

            var srcAlpha = (sourcePixel >> 24);
            if (srcAlpha > 0)
            {
                var dstAlpha = (destPixel >> 24);

                if ((srcAlpha == FullyOpaqueAlpha) || (dstAlpha == FullyTransparentAlpha))
                {
                    destPixel = sourcePixel;
                }
                else
                {
                    var invAlpha = 255 - srcAlpha;

                    // extract 16-bit pairs
                    var srcRB = sourcePixel & 0x00ff00ff;
                    var dstRB = destPixel & 0x00ff00ff;
                    var srcAG = sourcePixel & 0xff00ff00;
                    var dstAG = (destPixel >> 8) & 0x00ff00ff;

                    // calculate alpha blend
                    dstRB = srcRB + (invAlpha*dstRB >> 8) & 0x00ff00ff;
                    dstAG = srcAG + invAlpha*dstAG & 0xff00ff00;

                    // create new pixel
                    destPixel = dstAG + dstRB;
                }

                dstPixel = (int) destPixel;
            }

            return dstPixel;
        }
        #endregion
    }
}