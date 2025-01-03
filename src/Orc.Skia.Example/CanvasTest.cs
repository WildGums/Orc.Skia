﻿#pragma warning disable IDISP001 // Dispose created
#pragma warning disable IDISP003 // Dispose previous before re-assigning

namespace Orc.Skia.Example;

using System.Collections.Generic;
using SkiaSharp;
using Orc.Theming;

using System.Windows;
using System.Windows.Media;

/// <summary>
/// The canvas test.
/// </summary>
public class CanvasTest
{
    /// <summary>
    /// The run tests.
    /// </summary>
    /// <param name="canvas">The canvas.</param>
    internal static void RunTests(SKCanvas canvas)
    {
        var colorData = BuildColor(255, 28, 255);

        var solidLinePaint = Orc.Skia.SKPaintHelper.CreateLinePaint(1, colorData, LineType.Solid);
        var dashedLinePaint = Orc.Skia.SKPaintHelper.CreateLinePaint(1, colorData, LineType.Dashed);
        var dottedLinePaint = Orc.Skia.SKPaintHelper.CreateLinePaint(1, colorData, LineType.Dotted);

        canvas.DrawHorizontalLine(new Point(10, 10), new Point(210, 15), solidLinePaint);
        canvas.DrawHorizontalLine(new Point(10, 30), new Point(210, 35), dashedLinePaint);
        canvas.DrawHorizontalLine(new Point(10, 50), new Point(210, 55), dottedLinePaint);

        colorData = BuildColor(0, 0, 255);
        canvas.DrawHorizontalLine(new Point(150, 290), new Point(440, 296), dashedLinePaint);
        canvas.DrawHorizontalLine(new Point(150, 300), new Point(440, 306), dottedLinePaint);

        colorData = BuildColor(255, 0, 0);
        canvas.DrawVerticalLine(new Point(550, 90), new Point(556, 380), dashedLinePaint);
        canvas.DrawVerticalLine(new Point(560, 90), new Point(566, 380), dottedLinePaint);

        colorData = BuildColor(155, 155, 255);
        canvas.DrawVerticalLine(new Point(310, 10), new Point(315, 210), solidLinePaint);
        canvas.DrawVerticalLine(new Point(330, 10), new Point(335, 210), dashedLinePaint);
        canvas.DrawVerticalLine(new Point(350, 10), new Point(355, 210), dottedLinePaint);

        colorData = BuildColor(255, 155, 155);

        solidLinePaint = Orc.Skia.SKPaintHelper.CreateLinePaint(5, colorData, LineType.Solid);
        dashedLinePaint = Orc.Skia.SKPaintHelper.CreateLinePaint(5, colorData, LineType.Dashed);
        dottedLinePaint = Orc.Skia.SKPaintHelper.CreateLinePaint(5, colorData, LineType.Dotted);

        var points = new[] { new Point(10, 100), new Point(210, 100), new Point(210, 130), new Point(10, 130), new Point(10, 160), new Point(210, 160) };
        canvas.DrawLineThroughPoints(points, solidLinePaint);

        points = new[] { new Point(10, 250), new Point(210, 250), new Point(210, 280), new Point(10, 280), new Point(10, 310), new Point(210, 310) };
        canvas.DrawLineThroughPoints(points, dashedLinePaint);

        points = new[] { new Point(260, 250), new Point(460, 250), new Point(460, 280), new Point(260, 280), new Point(260, 310), new Point(460, 310) };
        canvas.DrawLineThroughPoints(points, dottedLinePaint);

        colorData = BuildColor(0, 127, 127);

        solidLinePaint = Orc.Skia.SKPaintHelper.CreateLinePaint(2, colorData, LineType.Solid);
        dashedLinePaint = Orc.Skia.SKPaintHelper.CreateLinePaint(2, colorData, LineType.Dashed);
        dottedLinePaint = Orc.Skia.SKPaintHelper.CreateLinePaint(2, colorData, LineType.Dotted);

        points = new[] { new Point(40, -10), new Point(40, 70), new Point(590, 70), new Point(590, 490), new Point(610, 490) };
        canvas.DrawLineThroughPoints(points, solidLinePaint);

        points = new[] { new Point(50, -20), new Point(50, 80), new Point(580, 80), new Point(580, 480), new Point(610, 480) };
        canvas.DrawLineThroughPoints(points, dashedLinePaint);

        points = new[] { new Point(60, -30), new Point(60, 90), new Point(570, 90), new Point(570, 470), new Point(610, 470) };
        canvas.DrawLineThroughPoints(points, dottedLinePaint);

        var lineHeight = 20;
        var lineWidth = 250;

        for (var i = lineHeight; i < 400; i += lineHeight)
        {
            canvas.DrawText("Text is also part of performance", new Rect(40, i, lineWidth, lineHeight), 
                Colors.DarkGreen, 14f);
        }
    }

    /// <summary>
    /// Draws zig zags to check clipping.
    /// </summary>
    /// <param name="canvas">The line canvas.</param>
    internal static void RunZigZagTests(SKCanvas canvas)
    {
        var colorData = BuildColor(148, 131, 72);

        var solidLinePaint = Orc.Skia.SKPaintHelper.CreateLinePaint(3, colorData, LineType.Solid);
        var dashedLinePaint = Orc.Skia.SKPaintHelper.CreateLinePaint(3, colorData, LineType.Dashed);
        var dottedLinePaint = Orc.Skia.SKPaintHelper.CreateLinePaint(3, colorData, LineType.Dotted);

        var points = BuildVerticalZigZag(-20, 20, -20, 520, 15);
        canvas.DrawLineThroughPoints(points, solidLinePaint);

        points = BuildVerticalZigZag(270, 330, -20, 520, 25);
        canvas.DrawLineThroughPoints(points, dashedLinePaint);

        points = BuildVerticalZigZag(550, 650, -20, 520, 40);
        canvas.DrawLineThroughPoints(points, dottedLinePaint);
    }

    /// <summary>
    /// The build color.
    /// </summary>
    /// <param name="r">The r.</param>
    /// <param name="g">The g.</param>
    /// <param name="b">The b.</param>
    /// <param name="a">The a.</param>
    /// <returns>The <see cref="int"/>.</returns>
    private static Color BuildColor(int r, int g, int b, int a = 255)
    {
        var colorData = a << 24; // A
        colorData |= r << 16; // R
        colorData |= g << 8; // G
        colorData |= b << 0; // B
        return colorData.ToColor();
    }

    /// <summary>
    /// The build vertical zig zag.
    /// </summary>
    /// <param name="xMin">The x min.</param>
    /// <param name="xMax">The x max.</param>
    /// <param name="yMin">The y min.</param>
    /// <param name="yMax">The y max.</param>
    /// <param name="amplitude">The amplitude.</param>
    /// <returns>The <see><cref>Point[]</cref></see> .</returns>
    private static Point[] BuildVerticalZigZag(int xMin, int xMax, int yMin, int yMax, int amplitude)
    {
        var points = new List<Point> { new Point(xMin, yMin) };

        for (int y = yMin, side = 0; y < yMax; y += amplitude, side++)
        {
            if (side % 2 == 0)
            {
                points.Add(new Point(xMax, y));
                points.Add(new Point(xMax, y + amplitude));
            }
            else
            {
                points.Add(new Point(xMin, y));
                points.Add(new Point(xMin, y + amplitude));
            }
        }

        return points.ToArray();
    }
}
