// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorsExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Skia.Coloring
{
    using System;

#if NETFX_CORE
    using Windows.UI;
#endif

#if NET || NETCORE
    using System.Windows.Media;
#endif

    /// <summary>
    /// Class with extension methods for color structures and colors as integers.
    /// </summary>
    public static class ColorsExtensions
    {
        #region Methods
        /// <summary>
        /// Calculates aplha blend of two colors.
        /// </summary>
        /// <param name="source">Source color as color.</param>
        /// <param name="dest">Destination color as int</param>
        public static int AlphaBlend(this Color source, int dest)
        {
            var srcPixelPremultiplyAlpha = (uint)source.PremultiplyAlpha();
            var invAlpha = 255 - source.A;

            // extract 16-bit pairs
            var dstRB = dest & 0x00ff00ff;
            var dstAG = (dest >> 8) & 0x00ff00ff;

            var srcRB = srcPixelPremultiplyAlpha & 0x00ff00ff;
            var srcAG = (srcPixelPremultiplyAlpha >> 8) & 0x00ff00ff;

            // calculate alpha blend
            dstRB = (int)(srcRB + ((invAlpha * dstRB) >> 8)) & 0x00ff00ff;
            dstAG = (int)(srcAG + ((invAlpha * dstAG) >> 8)) & 0x00ff00ff;

            // create new pixel
            return dstRB + (dstAG << 8);
        }

        /// <summary>
        /// Converts HSB to RGB.
        /// </summary>
        public static Color HSBToRGB(double h, double s, double b)
        {
            double r = 0;
            double g = 0;
            double blue = 0;

            if (s == 0)
            {
                r = g = blue = b;
            }
            else
            {
                // the color wheel consists of 6 sectors. Figure out which sector you're in.
                var sectorPos = h / 60.0;
                var sectorNumber = (int)(Math.Floor(sectorPos));
                // get the fractional part of the sector
                var fractionalSector = sectorPos - sectorNumber;

                // calculate values for the three axes of the color.
                var p = b * (1.0 - s);
                var q = b * (1.0 - (s * fractionalSector));
                var t = b * (1.0 - (s * (1 - fractionalSector)));

                // assign the fractional colors to r, g, and b based on the sector the angle is in.
                switch (sectorNumber)
                {
                    case 0:
                        r = b;
                        g = t;
                        blue = p;
                        break;

                    case 1:
                        r = q;
                        g = b;
                        blue = p;
                        break;

                    case 2:
                        r = p;
                        g = b;
                        blue = t;
                        break;

                    case 3:
                        r = p;
                        g = q;
                        blue = b;
                        break;

                    case 4:
                        r = t;
                        g = p;
                        blue = b;
                        break;

                    case 5:
                        r = b;
                        g = p;
                        blue = q;
                        break;
                }
            }

            return Color.FromArgb(
                AlphaBlendingHelper.FullyOpaqueAlpha,
                Convert.ToByte(Double.Parse(String.Format("{0:0.00}", r * 255.0))),
                Convert.ToByte(Double.Parse(String.Format("{0:0.00}", g * 255.0))),
                Convert.ToByte(Double.Parse(String.Format("{0:0.00}", blue * 255.0)))
                );
        }

        /// <summary>
        /// Returns true if color is dark and false if color is light
        /// </summary>
        /// <param name="color">Color to check</param>
        /// <returns></returns>
        public static bool IsDarkColor(this Color color)
        {
            var luminance = color.GetPerceptiveLuminance();
            return luminance >= 0.5;
        }

        /// <summary>
        /// Counting the perceptive luminance - human eye favors green color...
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static float GetPerceptiveLuminance(this Color color)
        {
            return 1 - (0.255f * color.R + 0.655f * color.G + 0.09f * color.B) / 255;
        }

        /// <summary>
        /// Get gray equivalent of color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color GrayColor(this Color color)
        {
            var luminance = 1 - color.GetPerceptiveLuminance();
            var c = Clamp(luminance * 255);

#if NETFX_CORE
            return Color.FromArgb(255, c, c, c);
#else
            return Color.FromRgb(c, c, c);
#endif
        }

        /// <summary>
        /// Makes color more saturated by given coefficient
        /// </summary>
        /// <param name="color"></param>
        /// <param name="coeficient"></param>
        /// <returns></returns>
        public static Color MakeColorMoreSaturated(this Color color, double coeficient)
        {
            var hsb = RGBToHSB(color.R, color.G, color.B);
            var saturation = hsb.Saturation;
            saturation = saturation * coeficient;
            hsb.Saturation = saturation;
            var makeLessBright = HSBToRGB(hsb.Hue, hsb.Saturation, hsb.Brightness);
            makeLessBright.A = 0xff;
            return makeLessBright;
        }

        /// <summary>
        /// Converts RGB to HSB.
        /// </summary>
        public static ColorHsb RGBToHSB(int red, int green, int blue)
        {
            // normalize red, green and blue values
            var r = (red / 255.0);
            var g = (green / 255.0);
            var b = (blue / 255.0);

            // conversion start
            var max = Math.Max(r, Math.Max(g, b));
            var min = Math.Min(r, Math.Min(g, b));

            var h = 0.0;
            if (max == r && g >= b)
            {
                h = 60 * (g - b) / (max - min);
            }
            else if (max == r)
            {
                h = 60 * (g - b) / (max - min) + 360;
            }
            else if (max == g)
            {
                h = 60 * (b - r) / (max - min) + 120;
            }
            else if (max == b)
            {
                h = 60 * (r - g) / (max - min) + 240;
            }

            var s = (max == 0) ? 0.0 : (1.0 - (min / max));

            return new ColorHsb(h, s, max);
        }

        /// <summary>
        /// Changes int value of color from bitmap to its RGBA representation.
        /// </summary>
        /// <param name="colorAsInt">Integer representation of color</param>
        /// <returns>Color as RGBA</returns>
        public static Color ToColor(this int colorAsInt)
        {
            return Orc.Skia.ColorHelper.GetColor(colorAsInt);
        }

        /// <summary>
        /// Returns 0-255 scale representation of color in gray scale
        /// </summary>
        /// <param name="color">Color to gray scale</param>
        /// <returns></returns>
        public static double ToGrayScale(this Color color)
        {
            //returns 1 for white 0 for black color
            var grayShade = (color.R + color.G + color.B)
                            * (color.A / (double)AlphaBlendingHelper.FullyOpaqueAlpha)
                            / ((double)0xff * 3);
            return grayShade;
        }

        /// <summary>
        /// The to integer
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public static int ToInt(this Color color)
        {
            return color.ToInt(ColorShade.Light);
        }

        /// <summary>
        /// The to integer
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="colorShade">The color shade.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public static int ToInt(this Color color, ColorShade colorShade)
        {
            int a = color.A;
            int r = color.R;
            int g = color.G;
            int b = color.B;

            var ratio = 1.00;

            if (colorShade == ColorShade.Medium)
            {
                ratio = 0.80;
            }
            else if (colorShade == ColorShade.Dark)
            {
                ratio = 0.64;
            }

            var c = (a << 24) + ((int)(r * ratio) << 16) + ((int)(g * ratio) << 8) + (int)(b * ratio);

            return c;
        }

        /// <summary>
        /// Premultiplies aplha of the color struct.
        /// </summary>
        /// <param name="color">The color to premultiply</param>
        /// <returns>Premultiplied color</returns>
        private static int PremultiplyAlpha(this Color color)
        {
            int srcAlpha = color.A;

            var srcR = ((color.R * srcAlpha) >> 0x08);
            var srcG = ((color.G * srcAlpha) >> 0x08);
            var srcB = ((color.B * srcAlpha) >> 0x08);

            return (srcAlpha << 24) | (srcR << 16) | (srcG << 8) | srcB;
        }

        /// <summary>
        /// Set brightness
        /// </summary>
        /// <param name="color"></param>
        /// <param name="brightness"></param>
        /// <returns></returns>
        public static Color SetBrightness(this Color color, float brightness)
        {
            var r = Clamp(color.R * brightness);
            var g = Clamp(color.G * brightness);
            var b = Clamp(color.B * brightness);

            return Color.FromArgb(color.A, r, g, b);
        }

        private static byte Clamp(double value)
        {
            var x = (int)Math.Round(value);

            if (x > 255)
            {
                return 255;
            }
            if (x < 0)
            {
                return 0;
            }
            return (byte)x;
        }
        #endregion
    }
}
