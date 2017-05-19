// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntColors.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Skia.Coloring
{
#if NETFX_CORE
    using Windows.UI;

#else
    using System.Windows.Media;
#endif

    /// <summary>
    /// The integer colors.
    /// </summary>
    public static class IntColors
    {
        #region Constants
        /// <summary>
        /// The color white
        /// </summary>
        public static readonly int White = -1;

        /// <summary>
        /// The black color.
        /// </summary>
        public static readonly int Black = -16777216;

        /// <summary>
        /// The dark gray.
        /// </summary>
        public static readonly int DarkGray = -5658199;

        /// <summary>
        /// The light gray.
        /// </summary>
        public static readonly int LightGray = -2631721;

        /// <summary>
        /// The medium gray.
        /// </summary>
        public static readonly int MediumGray = -4079167;

        /// <summary>
        /// The soft gray.
        /// </summary>
        public static readonly int SoftGray = -1052689;

        /// <summary>
        /// The red.
        /// </summary>
        public static readonly int Red = -65536;

        /// <summary>
        /// The blue.
        /// </summary>
        public static readonly int Blue = -16776961;

        /// <summary>
        /// The Gold colour.
        /// </summary>
        public static readonly int Gold = ColorFromRgb(0xff, 0xd7, 0).ToInt();

        /// <summary>
        /// The Yellow colour.
        /// </summary>
        public static readonly int Yellow = ColorFromRgb(0xff, 0xff, 0).ToInt();

        /// <summary>
        /// The transparent soft gray color
        /// </summary>
        public static readonly int TransparentSoftGray = 520093696;
        #endregion

        #region Methods
        /// <summary>
        /// Creates a color from RGB values with alpha = 1.
        /// </summary>
        /// <param name="r">R channel.</param>
        /// <param name="g">G channel.</param>
        /// <param name="b">B channel.</param>
        /// <returns>New color.</returns>
        private static Color ColorFromRgb(byte r, byte g, byte b)
        {
#if NETFX_CORE
            return Color.FromArgb(255, r, g, b);
#else
            return Color.FromRgb(r, g, b);
#endif
        }
        #endregion
    }
}