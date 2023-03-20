namespace Orc.Skia;

using System;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;

public class SkiaElement : SKElement, ISkiaElement
{
    public event EventHandler<CanvasRenderingEventArgs>? Rendering;

    public event EventHandler<CanvasRenderingEventArgs>? Rendered;

    public void Update()
    {
        InvalidateVisual();
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        Rendering?.Invoke(this, new CanvasRenderingEventArgs(e.Surface.Canvas));

        base.OnPaintSurface(e);

        Rendered?.Invoke(this, new CanvasRenderingEventArgs(e.Surface.Canvas));
    }
}