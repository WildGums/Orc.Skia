﻿namespace Orc.Skia;

using System;
using System.Windows;

/// <summary>
/// Rectangle extension class
/// </summary>
public static class RectangleExtensions
{
    public static Int32Rect ToInt32Rect(this Rect rect)
    {
        var int32Rect = new Int32Rect((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
        return int32Rect;
    }

    public static Point GetTopLeft(this Rect rect)
    {
        return new Point(rect.Left, rect.Top);
    }

    /// <summary>
    /// Checks if second rectangle have common area bigger than 0.
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="toCheck">Second rectangle to check</param>
    /// <returns></returns>
    /// <remarks>
    /// .NET intersection method <c>Rect.Intersect(Rect)</c> returns not
    /// empty rectangle if 2 rectangles have common side or part of side. This method returns
    /// true only if intersection has area bigger than 0 (width * height &gt;0).
    /// </remarks>
    public static bool IntersectsAndIsNotNeighbour(this Rect rect, Rect toCheck)
    {
        var intersection = rect;
        intersection.Intersect(toCheck);
        return intersection is { Height: > 0, Width: > 0 };
    }

    public static Rect OffsetTopLeft(this Rect rect, Point offset)
    {
        rect.Y += offset.Y;
        rect.X += offset.X;

        return rect;
    }

    public static Rect OffsetTopLeft(this Rect rect, double offsetX, double offsetY)
    {
        rect.Y += offsetY;
        rect.X += offsetX;

        return rect;
    }

    public static Rect ProduceIntersection(this Rect firstRect, Rect rect)
    {
        firstRect.Intersect(rect);
        return firstRect;
    }

    public static Rect SetTopLeft(this Rect rect, Point newLocation)
    {
        var location = new Rect(rect.X, rect.Y, rect.Width, rect.Height)
        {
            X = newLocation.X,
            Y = newLocation.Y
        };

        return location;
    }

    public static Rect SubtractHeight(this Rect rect, double value)
    {
        rect.Height = Math.Max(0, rect.Height - value);
        return rect;
    }

    public static Rect SubtractWidth(this Rect rect, double value)
    {
        rect.Width = Math.Max(0, rect.Width - value);
        return rect;
    }
}
