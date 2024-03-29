﻿#pragma warning disable 414

namespace Orc.Skia;

using System.Windows.Controls;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Catel;
using SkiaSharp;
using System.Diagnostics;

/// <summary>
/// SkiaCanvas class.
/// </summary>
/// <remarks>
/// Some parts are based on https://raw.githubusercontent.com/mono/SkiaSharp/master/source/SkiaSharp.Views/SkiaSharp.Views.UWP/SKXamlCanvas.cs (MIT).
/// </remarks>
public class SkiaCanvas : Canvas, ISkiaElement
{
    private readonly object _syncObject = new();

    protected double DpiX;
    protected double DpiY;

    private bool _canUseVulkan;
    private bool _canUseGl;

    private bool _ignorePixelScaling;
    private SKImageInfo _skImageInfo;
    private bool _isRendering;
    private int _renderScopeCounter;
    private WriteableBitmap? _bitmap;

    private Stopwatch _stopwatch;

    public SkiaCanvas()
    {
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        SizeChanged += OnSizeChanged;
        CompositionTarget.Rendering += OnCompositionTargetRendering;

        _stopwatch = Stopwatch.StartNew();
        FrameDelayInMilliseconds = 5;
        ForceNewBitmaps = ForceNewBitmapsDefaultValue;

        // Allow all by default
        _canUseVulkan = true;
        _canUseGl = true;
    }

    public int FrameDelayInMilliseconds { get; set; }

    public bool RenderWhenInvisible { get; set; }

    public static bool ForceNewBitmapsDefaultValue { get; set; }

    public bool ForceNewBitmaps { get; set; }

    public bool IgnorePixelScaling
    {
        get { return _ignorePixelScaling; }
        set
        {
            _ignorePixelScaling = value;
            Invalidate();
        }
    }

    public event EventHandler<CanvasRenderingEventArgs>? Rendering;

    public event EventHandler<CanvasRenderingEventArgs>? Rendered;

    public Rect GetSurfaceBoundary()
    {
        var info = GetImageInfo();

        return new Rect(new Point(0, 0), new Point(info.Width, info.Height));
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        Initialize();
    }

    private void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        Terminate();
    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        RecalculateDpi();
        Invalidate();
    }

    private void OnCompositionTargetRendering(object? sender, EventArgs e)
    {
        if (_stopwatch.ElapsedMilliseconds < FrameDelayInMilliseconds)
        {
            return;
        }

        if (!RenderWhenInvisible && !IsVisible)
        {
            return;
        }

        Update(true);

        _stopwatch = Stopwatch.StartNew();
    }

    public void Update()
    {
        Update(false);
    }

    protected virtual void Update(bool isInternal)
    {
        var isClearCanvas = _bitmap is null;

        if (ActualWidth == 0 || ActualHeight == 0)
        {
            return;
        }

        lock (_syncObject)
        {
            var size = CreateSize(out var scaleX, out var scaleY);
            if (size.Width <= 0 || size.Height <= 0)
            {
                return;
            }

            if (_bitmap is null || size.Width != _bitmap.PixelWidth || size.Height != _bitmap.PixelHeight)
            {
                isClearCanvas = true;
            }

            if (!isClearCanvas && (!IsRenderingAllowed() || _isRendering))
            {
                return;
            }

            if (_bitmap is null || isClearCanvas || (!isInternal && ForceNewBitmaps))
            {
                isClearCanvas = true;

                FreeBitmap();

                _skImageInfo = new SKImageInfo(size.Width, size.Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
                _bitmap = new WriteableBitmap(_skImageInfo.Width, _skImageInfo.Height, 96 * scaleX, 96 * scaleY, PixelFormats.Pbgra32, null);
            }

            // draw on the bitmap
            _bitmap.Lock();

            using (var surface = SKSurface.Create(_skImageInfo, _bitmap.BackBuffer, _bitmap.BackBufferStride))
            {
                var canvas = surface.Canvas;
                using (new RenderingScope(this, canvas))
                {
                    var eventArgs = new CanvasRenderingEventArgs(canvas);

                    OnRendering(canvas);
                    Rendering?.Invoke(this, eventArgs);

                    Render(canvas, isClearCanvas);

                    Rendered?.Invoke(this, eventArgs);
                    OnRendered(canvas);
                }
            }

            // draw the bitmap to the screen
            if (!isClearCanvas)
            {
                var dirtyRect = new Int32Rect(0, 0, size.Width, size.Height);
                _bitmap.AddDirtyRect(dirtyRect);
            }

            _bitmap.Unlock();

            if (!isClearCanvas)
            {
                return;
            }

            var brush = new ImageBrush
            {
                ImageSource = _bitmap,
                AlignmentX = AlignmentX.Left,
                AlignmentY = AlignmentY.Top,
                Stretch = Stretch.None
            };

            SetValue(BackgroundProperty, brush);
        }
    }

    protected GRContext CreateRenderContext()
    {
        GRContext? renderContext = null;

        // TODO: What is best order of performance?

        if (_canUseVulkan)
        {
#pragma warning disable IDISP001 // Dispose created.
            var backendContext = new GRVkBackendContext();
            var contextOptions = new GRContextOptions();
#pragma warning restore IDISP001 // Dispose created.

#pragma warning disable IDISP004 // Don't ignore created IDisposable.
            renderContext = GRContext.CreateVulkan(backendContext, contextOptions);
#pragma warning restore IDISP004 // Don't ignore created IDisposable.

            if (renderContext is null)
            {
                _canUseVulkan = false;

                backendContext.Dispose();
            }
        }

        if (renderContext is null &&
            _canUseGl)
        {
#pragma warning disable IDISP004 // Don't ignore created IDisposable.
#pragma warning disable IDISP003 // Dispose previous before re-assigning.
            renderContext = GRContext.CreateGl(new GRContextOptions());
#pragma warning restore IDISP003 // Dispose previous before re-assigning.
#pragma warning restore IDISP004 // Don't ignore created IDisposable.

            if (renderContext is null)
            {
                _canUseGl = false;
            }
        }

#if DEBUG
        if (renderContext is null)
        {
            Debug.WriteLine("Render context is null");
        }
#endif

        // Fallback to software rendering?
        if (renderContext is null)
        {
            throw new InvalidOperationException("Render context could not be created");
        }

        return renderContext;
    }

    protected SKSizeI CreateSize(out double scaleX, out double scaleY)
    {
        scaleX = 1.0;
        scaleY = 1.0;

        var w = ActualWidth;
        var h = ActualHeight;

        if (!IsPositive(w) || !IsPositive(h))
        {
            return SKSizeI.Empty;
        }

        if (_ignorePixelScaling)
        {
            return new SKSizeI((int)w, (int)h);
        }

        var transformDevice = PresentationSource.FromVisual(this)?.CompositionTarget.TransformToDevice;
        if (transformDevice is null)
        {
            return new SKSizeI((int)w, (int)h);
        }

        var m = (Matrix)transformDevice;
        scaleX = m.M11;
        scaleY = m.M22;
        return new SKSizeI((int)(w * scaleX), (int)(h * scaleY));

        bool IsPositive(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value) && value > 0;
        }
    }

    protected virtual bool RecalculateDpi()
    {
        var previousDpiX = DpiX;
        var previousDpiY = DpiY;

        var source = PresentationSource.FromVisual(this);
        if (source is not null)
        {
            var transformToDevice = source.CompositionTarget?.TransformToDevice;
            if (transformToDevice is null)
            {
                return false;
            }

            DpiX = transformToDevice.Value.M11;
            DpiY = transformToDevice.Value.M22;
        }

        return Math.Abs(DpiX - previousDpiX) > 0.1d ||
               Math.Abs(DpiY - previousDpiY) > 0.1d;
    }

    public virtual void Invalidate()
    {
        FreeBitmap();

        Update(true);
    }

    protected virtual bool IsRenderingAllowed()
    {
        return true;
    }

    protected SKImageInfo GetImageInfo()
    {
        return _skImageInfo;
    }

    protected virtual void Render(SKCanvas canvas, bool isClearCanvas)
    {
    }

    protected virtual void OnRendering(SKCanvas canvas)
    {
    }

    protected virtual void OnRendered(SKCanvas canvas)
    {
    }

    protected virtual void Initialize()
    {
        if (RecalculateDpi())
        {
            Invalidate();
        }
    }

    protected virtual void Terminate()
    {
        FreeBitmap();
    }

    protected virtual void Resize()
    {
        Invalidate();
    }

    protected void InvalidateBitmap(SKCanvas canvas)
    {
        var bitmap = _bitmap;
        if (bitmap is not null)
        {
            var clipDeviceBounds = canvas.DeviceClipBounds.ToRect();
            bitmap.AddDirtyRect(clipDeviceBounds.ToInt32Rect());
        }
    }

    private void FreeBitmap()
    {
        if (_bitmap is null)
        {
            return;
        }

        SetValue(BackgroundProperty, null);
        _bitmap = null;
    }

    private class RenderingScope : Disposable
    {
        private readonly SkiaCanvas _canvas;
        private readonly SKCanvas _skCanvas;

        public RenderingScope(SkiaCanvas canvas, SKCanvas skCanvas)
        {
            _canvas = canvas;
            _skCanvas = skCanvas;
            canvas._isRendering = true;
            canvas._renderScopeCounter++;
        }

        protected override void DisposeManaged()
        {
            _canvas._isRendering = false;
            if (_canvas._renderScopeCounter == 0)
            {
                return;
            }

            if (--_canvas._renderScopeCounter != 0)
            {
                return;
            }

            _canvas.InvalidateBitmap(_skCanvas);
        }
    }
}
