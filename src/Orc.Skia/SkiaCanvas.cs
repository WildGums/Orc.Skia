// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SkiaCanvas.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------



#pragma warning disable 414

namespace Orc.Skia
{
    using System;
    using SkiaSharp;
#if NETFX_CORE
    using Windows.Foundation;
    using Windows.Graphics.Display;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;
#else
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Catel;
#endif

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

        private IntPtr _pixels;
        private WriteableBitmap _bitmap;

        protected double DpiX;
        protected double DpiY;
        private bool _ignorePixelScaling;

        private DateTime _lastTime = DateTime.Now;

        private bool _isRendering = false;
        private int _renderScopeCounter;
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
        public void Update()
        {
            if (ActualWidth == 0 || ActualHeight == 0 || Visibility != Visibility.Visible)
            {
                return;
            }

            lock (_syncObject)
            {
                if (DpiX == 0 || DpiY == 0)
                {
                    RecalculateDpi();
                }

                var info = GetImageInfo();

                var isClearCanvas = _bitmap == null;
                if (isClearCanvas)
                {
                    CreateBitmap(info);
                }

#if NET || NETCORE
                using (new BitmapLockScope(_bitmap))
                {
#endif
                    using (var surface = SKSurface.Create(info, _pixels, info.Width * 4))
                    {
                        var canvas = surface.Canvas;

                        if (!_ignorePixelScaling)
                        {
                            var matrix = canvas.TotalMatrix;

                            matrix.ScaleX = 1.0f * (float)DpiX;
                            matrix.ScaleY = 1.0f * (float)DpiY;

                            canvas.SetMatrix(matrix);
                        }

                        if (!IsRenderingAllowed() || _isRendering)
                        {
                            return;
                        }

                        using (new RenderingScope(this, canvas))
                        {
                            var eventArgs = new CanvasRenderingEventArgs(canvas);

                            OnRendering(canvas);
                            Rendering?.Invoke(this, eventArgs);

                            Render(canvas, isClearCanvas);

                            Rendered?.Invoke(this, eventArgs);
                            OnRendered(canvas);
                        }

                        InvalidateBitmap(canvas);
                    }
#if NET || NETCORE
                }
#endif
            }
        }

        public Rect GetSurfaceBoundary()
        {
            var info = GetImageInfo();

        //  var info = _skImageInfo;

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
            Update2();
            //    Invalidate();
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

            Update2();

            _lastTime = DateTime.Now;
        }

        private SKImageInfo _skImageInfo;

        protected void Update2()
        {
            if (Visibility != Visibility.Visible)
            {
                return;
            }

            var size = CreateSize(out var scaleX, out var scaleY);
            if (size.Width <= 0 || size.Height <= 0)
            {
                return;
            }

            _skImageInfo = new SKImageInfo(size.Width, size.Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);

            var isClearCanvas = false;
            if (_bitmap == null || _skImageInfo.Width != _bitmap.PixelWidth || _skImageInfo.Height != _bitmap.PixelHeight)
            {
                isClearCanvas = true;
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
            _bitmap.AddDirtyRect(new Int32Rect(0, 0, size.Width, size.Height));
            _bitmap.Unlock();

            if (isClearCanvas)
            {
                var brush = new ImageBrush
                {
                    ImageSource = _bitmap,
                    AlignmentX = AlignmentX.Left,
                    AlignmentY = AlignmentY.Top,
                    Stretch = Stretch.Fill
                };

                SetValue(BackgroundProperty, brush);
            }
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
            if (transformDevice == null)
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
            if (source != null)
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

            Update2();
        }

        protected virtual bool IsRenderingAllowed()
        {
            return true;
        }

        protected SKImageInfo GetImageInfo()
        {

            return _skImageInfo;
            //int width, height;
            //if (_ignorePixelScaling)
            //{
            //    width = (int)ActualWidth;
            //    height = (int)ActualHeight;
            //}
            //else
            //{
            //    width = (int)(ActualWidth * _dpiX);
            //    height = (int)(ActualHeight * _dpiY);
            //}

            //if (width == 0 || height == 0)
            //{
            //    return new SKImageInfo();
            //}

            //return new SKImageInfo(width, height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
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

        private void CreateBitmap(SKImageInfo info)
        {
            if (_bitmap == null || _bitmap.PixelWidth != info.Width || _bitmap.PixelHeight != info.Height)
            {
                FreeBitmap();

#if NETFX_CORE
                _bitmap = new WriteableBitmap(info.Width, info.Height);
                _pixels = _bitmap.GetPixels();
#elif NET || NETCORE
                _bitmap = new WriteableBitmap(info.Width, info.Height, 96d, 96d, PixelFormats.Bgra32, BitmapPalettes.Halftone256Transparent);
                _pixels = _bitmap.BackBuffer;
#else
                TARGET PLATFORM NOT YET SUPPORTED
#endif

                var brush = new ImageBrush
                {
                    ImageSource = _bitmap,
                    AlignmentX = AlignmentX.Left,
                    AlignmentY = AlignmentY.Top,
                    Stretch = Stretch.Fill
                };

                if (IgnorePixelScaling)
                {
                    var matrix = new ScaleTransform
                    {
                        ScaleX = 1.0 / DpiX,
                        ScaleY = 1.0 / DpiY
                    };

                    brush.Transform = matrix;
                }

                SetValue(BackgroundProperty, brush);
            }
        }

        private void FreeBitmap()
        {
            SetValue(BackgroundProperty, null);
            _bitmap = null;
            _pixels = IntPtr.Zero;
        }
        #endregion

        #region Nested classes
#if NET || NETCORE
        private class BitmapLockScope : Disposable
        {
            private readonly WriteableBitmap _bitmap;

            public BitmapLockScope(WriteableBitmap bitmap)
            {
                _bitmap = bitmap;
                _bitmap.Lock();
            }

            protected override void DisposeManaged()
            {
                _bitmap.Unlock();
            }
        }
#endif

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
        #endregion
    }
}
