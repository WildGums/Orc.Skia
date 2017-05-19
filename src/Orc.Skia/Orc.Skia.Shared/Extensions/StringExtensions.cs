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
        #region Methods
        internal static List<string> SplitLines(this string text)
        {
            if (text == null)
            {
                return null;
            }

            var lines = text.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);
            return lines.ToList();
        }
        #endregion
    }
}