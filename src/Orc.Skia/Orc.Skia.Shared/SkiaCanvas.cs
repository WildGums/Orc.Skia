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

        private double _dpiX;
        private double _dpiY;
        private bool _ignorePixelScaling;

        private DateTime _lastTime = DateTime.Now;
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

        ///// <summary>
        ///// One pixel width in 0-1 range
        ///// </summary>
        //public double OnePixelHeigthIn01Range
        //{
        //    get { return _onePixelHeigthIn01Range; }
        //}

        ///// <summary>
        ///// One pixel width in 0-1 range
        ///// </summary>
        //public double OnePixelWidthIn01Range
        //{
        //    get { return _onePixelWidthIn01Range; }
        //}
        #endregion

        #region Events
        public event EventHandler<CanvasRenderingEventArgs> Rendering;

        public event EventHandler<CanvasRenderingEventArgs> Rendered;
        #endregion

        #region Methods
        public void Clear()
        {
            Update(true);
        }

        public virtual void OnRendering(SKCanvas canvas)
        {
        }

        public virtual void OnRendered(SKCanvas canvas)
        {
        }

        public bool RecalculateDpi()
        {
            var previousDpiX = _dpiX;
            var previousDpiY = _dpiY;

#if NETFX_CORE
            var display = DisplayInformation.GetForCurrentView();
            _dpiX = _dpiY = display.LogicalDpi / 96.0f;
#else
            var source = PresentationSource.FromVisual(this);
            if (source != null)
            {
                var transformToDevice = source.CompositionTarget.TransformToDevice;

                _dpiX = transformToDevice.M11;
                _dpiY = transformToDevice.M22;
            }
#endif

            if (Math.Abs(_dpiX - previousDpiX) > 0.1d ||
                Math.Abs(_dpiY - previousDpiY) > 0.1d)
            {
                return true;
            }

            return false;
        }

        public void Invalidate()
        {
            FreeBitmap();

            Update();

//#if NETFX_CORE
//            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Update);
//#else
//            Dispatcher.BeginInvoke((Action)Update, null);
//#endif
        }

        private void Update()
        {
            Update(true);
        }

        private void Update(bool clear)
        {
            if (ActualWidth == 0 || ActualHeight == 0 || Visibility != Visibility.Visible)
            {
                return;
            }

            lock (_syncObject)
            {
                if (_dpiX == 0 || _dpiY == 0)
                {
                    RecalculateDpi();
                }

                int width, height;
                if (_ignorePixelScaling)
                {
                    width = (int) ActualWidth;
                    height = (int) ActualHeight;
                }
                else
                {
                    width = (int) (ActualWidth * _dpiX);
                    height = (int) (ActualHeight * _dpiY);
                }

                if (width == 0 || height == 0)
                {
                    return;
                }

                var info = new SKImageInfo(width, height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);

                if (_bitmap == null)
                {
                    CreateBitmap(info);

                    clear = false;
                }

#if NET
                _bitmap.Lock();
#endif

                using (var surface = SKSurface.Create(info, _pixels, width * 4))
                {
                    var canvas = surface.Canvas;

                    if (!_ignorePixelScaling)
                    {
                        var matrix = canvas.TotalMatrix;

                        matrix.ScaleX = 1.0f * (float)_dpiX;
                        matrix.ScaleY = 1.0f * (float)_dpiY;

                        canvas.SetMatrix(matrix);
                    }

                    if (clear)
                    {
                        canvas.Clear();
                    }

                    var eventArgs = new CanvasRenderingEventArgs(canvas);

                    OnRendering(canvas);
                    Rendering?.Invoke(this, eventArgs);

                    Render(canvas);

                    Rendered?.Invoke(this, eventArgs);
                    OnRendered(canvas);

#if NET
                    var clipDeviceBounds = canvas.ClipDeviceBounds.ToRect();
                    _bitmap.AddDirtyRect(clipDeviceBounds.ToInt32Rect());
#elif NETFX_CORE
                    _bitmap.Invalidate();
#endif

#if NET
                    _bitmap.Unlock();
#endif
                }
            }
        }

        protected virtual void Render(SKCanvas canvas)
        {
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (RecalculateDpi())
            {
                Invalidate();
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            FreeBitmap();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Invalidate();
        }

#if NETFX_CORE
        private void OnCompositionTargetRendering(object sender, object e)
#else
        private void OnCompositionTargetRendering(object sender, EventArgs e)
#endif
        {
            if (DateTime.Now - _lastTime >= _frameDelay)
            {
                Update();

                _lastTime = DateTime.Now;
            }
        }

        private void CreateBitmap(SKImageInfo info)
        {
            if (_bitmap == null || _bitmap.PixelWidth != info.Width || _bitmap.PixelHeight != info.Height)
            {
                FreeBitmap();

#if NETFX_CORE
                _bitmap = new WriteableBitmap(info.Width, info.Height);
                _pixels = _bitmap.GetPixels();
#else
                _bitmap = new WriteableBitmap(info.Width, info.Height, 96d, 96d, PixelFormats.Bgra32, BitmapPalettes.Halftone256Transparent);
                _pixels = _bitmap.BackBuffer;
#endif

                var brush = new ImageBrush
                {
                    ImageSource = _bitmap,
                    AlignmentX = AlignmentX.Left,
                    AlignmentY = AlignmentY.Top,
                    Stretch = Stretch.None
                };

                if (!_ignorePixelScaling)
                {
                    var matrix = new ScaleTransform
                    {
                        ScaleX = 1.0 / _dpiX,
                        ScaleY = 1.0 / _dpiY
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
    }
}