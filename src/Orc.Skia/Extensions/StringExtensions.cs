namespace Orc.Skia;

using System;

internal static class StringExtensions
{
    private static readonly string[] LineSplitters = new[] { "\r\n", "\r", "\n" };

    internal static string[] SplitLines(this string text)
    {
        var lines = text.Split(LineSplitters, StringSplitOptions.None);
        return lines;
    }
}
