// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SkiaCanvas.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


#pragma warning disable 414

namespace Orc.Skia
{
#if NETFX_CORE
    using Windows.Foundation;
    using Windows.Graphics.Display;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;
#else
    using System.Windows.Controls;
#endif
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Catel;
    using SkiaSharp;

    /// <summary>
    /// SkiaCanvas class.
    /// </summary>
    /// <remarks>
    /// Some parts are based on https://raw.githubusercontent.com/mono/SkiaSharp/master/source/SkiaSharp.Views/SkiaSharp.Views.UWP/SKXamlCanvas.cs (MIT).
    /// </remarks>
    public class SkiaCanvas : Canvas
    {
        #region Fields
        private readonly TimeSpan _frameDelay = TimeSpan.FromMilliseconds(5);
        private readonly object _syncObject = new object();

        protected double DpiX;
        protected double DpiY;

        private bool _canUseVulkan;
        private bool _ignorePixelScaling;
        private SKImageInfo _skImageInfo;
        private DateTime _lastTime = DateTime.Now;
        private bool _isRendering = false;
        private int _renderScopeCounter;
        private IntPtr _pixels;
        private WriteableBitmap _bitmap;
        #endregion

        #region Constructors
        public SkiaCanvas()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            SizeChanged += OnSizeChanged;
            CompositionTarget.Rendering += OnCompositionTargetRendering;
        }
        #endregion

        #region Properties
        public bool IgnorePixelScaling
        {
            get { return _ignorePixelScaling; }
            set
            {
                _ignorePixelScaling = value;
                Invalidate();
            }
        }
        #endregion

        #region Events
        public event EventHandler<CanvasRenderingEventArgs> Rendering;

        public event EventHandler<CanvasRenderingEventArgs> Rendered;
        #endregion

        #region Methods
        public Rect GetSurfaceBoundary()
        {
            var info = GetImageInfo();

            return new Rect(new Point(0, 0), new Point(info.Width, info.Height));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Initialize();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Terminate();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            RecalculateDpi();
            Invalidate();
        }

#if NETFX_CORE
        private void OnCompositionTargetRendering(object sender, object e)
#elif NET || NETCORE
        private void OnCompositionTargetRendering(object sender, EventArgs e)
#else
        TARGET PLATFORM NOT YET SUPPORTED
#endif
        {
            if (DateTime.Now - _lastTime < _frameDelay)
            {
                return;
            }

            Update();

            _lastTime = DateTime.Now;
        }

        public void Update()
        {
            if (ActualWidth == 0 || ActualHeight == 0 || Visibility != Visibility.Visible)
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

                var isClearCanvas = false;
                if (_bitmap is null || size.Width != _bitmap.PixelWidth || size.Height != _bitmap.PixelHeight)
                {
                    isClearCanvas = true;
                    _skImageInfo = new SKImageInfo(size.Width, size.Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
                    _bitmap = new WriteableBitmap(_skImageInfo.Width, _skImageInfo.Height, 96 * scaleX, 96 * scaleY, PixelFormats.Pbgra32, null);
                }

                if (!isClearCanvas && (!IsRenderingAllowed() || _isRendering))
                {
                    return;
                }

                // draw on the bitmap
                _bitmap.Lock();

                using (var grvBackendContext = new GRVkBackendContext())
                {
#pragma warning disable IDISP001 // Dispose created.
                    var renderContext = CreateRenderContext();
#pragma warning restore IDISP001 // Dispose created.
                    if (renderContext is not null)
                    {
                        try
                        {
                            using (var surface = SKSurface.Create(renderContext, false, _skImageInfo))
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
                        }
                        finally
                        {
                            renderContext?.Dispose();
                        }
                    }
                }

                // draw the bitmap to the screen
                _bitmap.AddDirtyRect(new Int32Rect(0, 0, size.Width, size.Height));
                _bitmap.Unlock();

                if (isClearCanvas)
                {
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
        }

        protected GRContext CreateRenderContext()
        {
            GRContext renderContext = null;

            if (_canUseVulkan)
            {
#pragma warning disable IDISP004 // Don't ignore created IDisposable.
                renderContext = GRContext.CreateVulkan(new GRVkBackendContext(), new GRContextOptions());
#pragma warning restore IDISP004 // Don't ignore created IDisposable.
            }

            if (renderContext is null)
            {
                _canUseVulkan = false;

#pragma warning disable IDISP004 // Don't ignore created IDisposable.
                renderContext = GRContext.CreateGl(new GRContextOptions());
#pragma warning restore IDISP004 // Don't ignore created IDisposable.
            }

            return renderContext;
        }

        private SKSizeI CreateSize(out double scaleX, out double scaleY)
        {
            scaleX = 1.0;
            scaleY = 1.0;

            var w = ActualWidth;
            var h = ActualHeight;

            if (!IsPositive(w) || !IsPositive(h))
            {
                return SKSizeI.Empty;
            }

            if (IgnorePixelScaling)
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

#if NETFX_CORE
            var display = DisplayInformation.GetForCurrentView();
            _dpiX = _dpiY = display.LogicalDpi / 96.0f;
#elif NET || NETCORE
            var source = PresentationSource.FromVisual(this);
            if (source is not null)
            {
                var transformToDevice = source.CompositionTarget.TransformToDevice;

                DpiX = transformToDevice.M11;
                DpiY = transformToDevice.M22;
            }
#else
            TARGET PLATFORM NOT YET SUPPORTED
#endif

            if (Math.Abs(DpiX - previousDpiX) > 0.1d ||
                Math.Abs(DpiY - previousDpiY) > 0.1d)
            {
                return true;
            }

            return false;
        }

        public virtual void Invalidate()
        {
            FreeBitmap();

            Update();
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
#if NET || NETCORE
            var clipDeviceBounds = canvas.DeviceClipBounds.ToRect();
            _bitmap.AddDirtyRect(clipDeviceBounds.ToInt32Rect());
#elif NETFX_CORE
            _bitmap.Invalidate();
#else
        TARGET PLATFORM NOT YET SUPPORTED
#endif
        }

        private void FreeBitmap()
        {
            SetValue(BackgroundProperty, null);
            _bitmap = null;
            _pixels = IntPtr.Zero;
        }
        #endregion

        #region Nested classes
        private class RenderingScope : Disposable
        {
            #region Fields
            private readonly SkiaCanvas _canvas;
            private readonly SKCanvas _skCanvas;
            #endregion

            #region Constructors
            public RenderingScope(SkiaCanvas canvas, SKCanvas skCanvas)
            {
                _canvas = canvas;
                _skCanvas = skCanvas;
                canvas._isRendering = true;
                canvas._renderScopeCounter++;
            }
            #endregion

            #region Methods
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
            #endregion
        }
        #endregion
    }
}
