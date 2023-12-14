namespace Orc.Skia;

using System;

public interface ISkiaElement
{
    event EventHandler<CanvasRenderingEventArgs>? Rendering;
    event EventHandler<CanvasRenderingEventArgs>? Rendered;

    void Update();
}