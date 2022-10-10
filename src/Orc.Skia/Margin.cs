namespace Orc.Skia
{
    /// <summary>
    /// Margin setting for rendering request.
    /// </summary>
    public class Margin
    {
        /// <summary>
        /// Margin object constructor with double value for all sides.
        /// </summary>
        /// <param name="margin">Margin value for all sides.</param>
        public Margin(double margin)
        {
            Top = Bottom = Left = Right = (int) margin;
        }

        /// <summary>
        /// Margin object constructor with int value for all sides.
        /// </summary>
        /// <param name="margin">Margin value for all sides.</param>
        public Margin(int margin)
        {
            Top = Bottom = Left = Right = margin;
        }

        /// <summary>
        /// Margin object constructor with values for all sides.
        /// </summary>
        /// <param name="top">Margin value on top.</param>
        /// <param name="bottom">Margin value on bottom.</param>
        /// <param name="left">Margin value on left.</param>
        /// <param name="right">Margin value on right.</param>
        public Margin(int top, int bottom, int left, int right)
        {
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
        }

        /// <summary>
        /// Margin object constructor with values for all sides.
        /// </summary>
        /// <param name="top">Margin value on top.</param>
        /// <param name="bottom">Margin value on bottom.</param>
        /// <param name="left">Margin value on left.</param>
        /// <param name="right">Margin value on right.</param>
        public Margin(double top, double bottom, double left, double right)
        {
            Top = (int) top;
            Bottom = (int) bottom;
            Left = (int) left;
            Right = (int) right;
        }

        /// <summary>
        /// If this margin should be added to standard font margin.
        /// </summary>
        public bool AddToStandard { get; set; }

        /// <summary>
        /// Margin value on bottom.
        /// </summary>
        public int Bottom { get; set; }

        /// <summary>
        /// Returns true if all side of margin are <value>0</value>.
        /// </summary>
        /// <value></value>
        public bool IsZero
        {
            get { return !(Top != 0 || Bottom != 0 || Left != 0 || Right != 0); }
        }

        /// <summary>
        /// Margin value on left.
        /// </summary>
        public int Left { get; set; }

        /// <summary>
        /// Margin value on right.
        /// </summary>
        public int Right { get; set; }

        /// <summary>
        /// Margin value on top.
        /// </summary>
        public int Top { get; set; }

        /// <summary>
        /// Returns given <see cref="Margin"/> instance or new if given <paramref name="margin"/> value is null.
        /// </summary>
        /// <param name="margin">Given margin to check.</param>
        /// <param name="value">Value of new margin on all edges.</param>
        /// <returns></returns>
        public static Margin CurrentOrNew(Margin margin, double value = 0)
        {
            if (margin is not null)
            {
                if (margin.AddToStandard)
                {
                    var sum = new Margin(margin.Top + value, margin.Bottom + value, margin.Left + value, margin.Right + value);
                    return sum;
                }

                return margin;
            }

            return new Margin(value);
        }
    }
}
