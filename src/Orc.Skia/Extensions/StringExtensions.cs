// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Skia
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static class StringExtensions
    {
        private static readonly string[] LineSplitters = new[] { "\r\n", "\r", "\n" };

        #region Methods
        internal static string[] SplitLines(this string text)
        {
            if (text is null)
            {
                return Array.Empty<string>();
            }

            var lines = text.Split(LineSplitters, StringSplitOptions.None);
            return lines;
        }
        #endregion
    }
}
