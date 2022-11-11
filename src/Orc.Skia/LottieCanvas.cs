#if DEBUG
//#define DEBUG_BACKGROUND
#define DEBUG_TIMING
#define DEBUG_LOGGING
#endif

namespace Orc.Skia
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Web;
    using System.Windows;
    using System.Windows.Media.Animation;
    using System.Windows.Threading;
    using Catel;
    using Catel.Logging;
    using SkiaSharp;
    using SkiaSharp.Skottie;

    public class LottieCanvas : SkiaCanvas
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private const float FramesPerSecond = 60;

        private readonly DispatcherTimer _invalidationTimer = new();
        private readonly DispatcherTimer _resizeTimer = new();
        private readonly Stopwatch _frameWatcher = new();

        private int _repeatCount = 0;
        private bool _clearCanvas;
        private bool _autoPaused;
        private bool _isDirty = false;
        private SKRect _renderSize;

#if DEBUG_BACKGROUND
#pragma warning disable IDISP006 // Implement IDisposable
        private readonly SKPaint _debugPaint = SKPaintHelper.CreateLinePaint(2d, Colors.Red);
#pragma warning restore IDISP006 // Implement IDisposable
#endif

        public LottieCanvas()
        {
            _resizeTimer.Interval = TimeSpan.FromMilliseconds(50);
            _resizeTimer.Tick += OnResizeTimerTick;

            _invalidationTimer.Tick += OnInvalidationTimerTick;

            IsVisibleChanged += OnIsVisibleChanged;
        }

        public Animation Animation
        {
            get => (Animation)GetValue(AnimationProperty);
            set => throw Log.ErrorAndCreateException<InvalidOperationException>($"An attempt to modify read-only property \"{nameof(Animation)}\". Use \"OneWayToSource\" Mode Binding");
        }

        public static readonly DependencyProperty AnimationProperty =
            DependencyProperty.Register(nameof(Animation), typeof(Animation), typeof(LottieCanvas), new PropertyMetadata(null));

        public RepeatBehavior Repeat
        {
            get => (RepeatBehavior)GetValue(RepeatProperty);
            set => SetValue(RepeatProperty, value);
        }

        public static readonly DependencyProperty RepeatProperty =
            DependencyProperty.Register(nameof(Repeat), typeof(RepeatBehavior), typeof(LottieCanvas), new PropertyMetadata(RepeatBehavior.Forever));

        public AnimationMouseOverBehavior MouseOver
        {
            get => (AnimationMouseOverBehavior)GetValue(MouseOverProperty);
            set => SetValue(MouseOverProperty, value);
        }

        public static readonly DependencyProperty MouseOverProperty =
         DependencyProperty.Register(nameof(MouseOver), typeof(AnimationMouseOverBehavior), typeof(LottieCanvas), new PropertyMetadata(AnimationMouseOverBehavior.None));

        public bool IsPlaying
        {
            get => (bool)GetValue(IsPlayingProperty);
            set => throw Log.ErrorAndCreateException<InvalidOperationException>($"An attempt to modify read-only property \"{nameof(IsPlaying)}\". Use \"OneWayToSource\" Mode Binding");
        }

        public static readonly DependencyProperty IsPlayingProperty =
            DependencyProperty.Register(nameof(IsPlaying), typeof(bool), typeof(LottieCanvas), new PropertyMetadata(false));

        public Uri? UriSource
        {
            get => (Uri?)GetValue(UriSourceProperty);
            set => SetValue(UriSourceProperty, value);
        }

        public static readonly DependencyProperty UriSourceProperty =
            DependencyProperty.Register(nameof(UriSource), typeof(Uri), typeof(LottieCanvas), new PropertyMetadata((s, e) => ((LottieCanvas)s).OnUriSourceChanged()));

        private void OnUriSourceChanged()
        {
            try
            {
                var uri = UriSource;

                StopAnimation();

                // Sync sources
                SetCurrentValue(StreamSourceProperty, null);
                SetCurrentValue(AnimationProperty, null);

                _clearCanvas = true;
                Update();

                if (uri is null)
                {
                    return;
                }

                if (uri.IsFile)
                {
                    using (var fileStream = File.OpenRead(HttpUtility.UrlDecode(uri.AbsolutePath)))
                    {
                        InitializeAnimationFromSource(fileStream);
                        return;
                    }
                }

                var resourceStreamInfo = Application.GetResourceStream(uri);
                using (resourceStreamInfo.Stream)
                {
                    InitializeAnimationFromSource(resourceStreamInfo.Stream);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public Stream? StreamSource
        {
            get => (Stream?)GetValue(StreamSourceProperty);
            set => SetValue(StreamSourceProperty, value);
        }

        public static readonly DependencyProperty StreamSourceProperty =
            DependencyProperty.Register(nameof(StreamSource), typeof(Stream), typeof(LottieCanvas), new PropertyMetadata((s, e) => ((LottieCanvas)s).OnStreamSourceChanged()));

        private void OnStreamSourceChanged()
        {
            try
            {
                StopAnimation();

                // Sync sources
                SetCurrentValue(UriSourceProperty, null);
                SetCurrentValue(AnimationProperty, null);

                _clearCanvas = true;
                Update();

                var animationFromStreamSource = StreamSource;
                if (animationFromStreamSource is not null)
                {
                    InitializeAnimationFromSource(animationFromStreamSource);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private void InitializeAnimationFromSource(Stream source)
        {
            using (var skiaStream = new SKManagedStream(source))
            {
                if (Animation.TryCreate(skiaStream, out var animation))
                {
                    SetAnimation(animation);
                }
            }
        }

        public void SetAnimation(Animation animation)
        {
            ArgumentNullException.ThrowIfNull(animation);

            animation.Seek(0, null);

            SetCurrentValue(AnimationProperty, animation);

            CalculateRenderOffset();

            _invalidationTimer.Interval = TimeSpan.FromSeconds(Math.Max(1 / FramesPerSecond, 1 / animation.Fps));


            StartAnimation();
        }

        public void StartAnimation()
        {
            if (Animation is null)
            {
                return;
            }

            SetCurrentValue(IsPlayingProperty, true);

            CalculateRenderOffset();

            _invalidationTimer.Start();
            _frameWatcher.Restart();
        }

        public void ResumeAnimation()
        {
            if (Animation is null)
            {
                return;
            }

            if (!CanRestart())
            {
                return;
            }

            SetCurrentValue(IsPlayingProperty, true);

            CalculateRenderOffset();

            _invalidationTimer.Start();
            _frameWatcher.Start();
        }

        public void StopAnimation()
        {
            SetCurrentValue(IsPlayingProperty, false);

            _invalidationTimer.Stop();
            _frameWatcher.Stop();
        }

        private void CalculateRenderOffset()
        {
            var animation = Animation;
            if (animation is null)
            {
                return;
            }

            var left = 0f;
            var top = 0f;
            var width = 0f;
            var height = 0f;

            var renderSizeDip = RenderSize;
            _ = CreateSize(out var scaleX, out var scaleY);

            var animationSize = animation.Size;
            if (animationSize != default)
            {
                var correctAnimationSize = new SKSize(animationSize.Width * (float)scaleX,
                    animationSize.Height * (float)scaleY);

                var ratioX = (float)(renderSizeDip.Width / correctAnimationSize.Width);
                var ratioY = (float)(renderSizeDip.Height / correctAnimationSize.Height);

                var ratio = Math.Min(ratioX, ratioY);

                width = correctAnimationSize.Width * ratio;
                height = correctAnimationSize.Height * ratio;

                left = ((float)renderSizeDip.Width - width) / 2f;
                top = ((float)renderSizeDip.Height - height) / 2f;
            }

            var leftScaled = left * (float)scaleX;
            var topScaled = top * (float)scaleY;
            var widthScaled = (left + width) * (float)scaleX;
            var heightScaled = (top + height) * (float)scaleY;

            _renderSize = new SKRect(leftScaled, topScaled, widthScaled, heightScaled);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            ScheduleRenderSizeUpdate();
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
            {
                if (_autoPaused)
                {
                    _autoPaused = false;

#if DEBUG_LOGGING
                    Log.Debug("Resuming animation, canvas became visible");
#endif

                    ResumeAnimation();
                }
            }
            else
            {
                if (_invalidationTimer.IsEnabled)
                {
                    _invalidationTimer.Stop();
                    _autoPaused = true;

#if DEBUG_LOGGING
                    Log.Debug("Pausing animation, canvas became invisible");
#endif
                }
            }
        }

        protected override void Render(SKCanvas canvas, bool isClearCanvas)
        {
            if (_clearCanvas)
            {
                canvas.Clear();
                _clearCanvas = false;
                return;
            }

            var animation = Animation;
            if (animation is null)
            {
                return;
            }

            if (!IsVisible)
            {
                return;
            }

            var halt = false;

            // Check mouse
            if (MouseOver == AnimationMouseOverBehavior.Start)
            {
                if (IsMouseOver)
                {
                    ResumeAnimation();
                }
                else
                {
                    StopAnimation();
                    halt = true;
                }
            }

            if (MouseOver == AnimationMouseOverBehavior.Stop)
            {
                if (IsMouseOver)
                {
                    StopAnimation();
                    halt = true;
                }
                else
                {
                    ResumeAnimation();
                }
            }

            if (MouseOver == AnimationMouseOverBehavior.None)
            {
                // Always try resume, ResumeAnimation() handles cases when animation cannot continue
                ResumeAnimation();
            }

            if (!halt && _frameWatcher.Elapsed > animation.Duration)
            {
                if (CanRestart())
                {
                    _repeatCount++;

                    StartAnimation();
                }
                else
                {
                    StopAnimation();
                }
            }

            if (!_isDirty)
            {
                return;
            }

            RenderAnimation(animation, canvas);

            _isDirty = false;
        }

        private void ScheduleRenderSizeUpdate()
        {
            _resizeTimer.Stop();
            _resizeTimer.Start();
        }

        private void OnResizeTimerTick(object? sender, EventArgs e)
        {
            _resizeTimer.Stop();

            CalculateRenderOffset();
        }

        private void OnInvalidationTimerTick(object? sender, EventArgs e)
        {
#if DEBUG_LOGGING
            Log.Debug("Invalidating animation frame");
#endif

            if (!IsVisible)
            {
#if DEBUG_LOGGING
                Log.Debug("Pausing animation, canvas is invisible");
#endif

                _autoPaused = IsPlaying;
                StopAnimation();
            }

            if (IsPlaying)
            {
                _isDirty = true;
                Update();
            }
        }

        /// <summary>
        /// Support different Repeat behaviors.
        /// Repeat count prevail over time
        /// </summary>
        /// <returns></returns>
        private bool CanRestart()
        {
            if (Repeat == RepeatBehavior.Forever)
            {
                return true;
            }

            if (Repeat.HasCount && Repeat.Count >= _repeatCount)
            {
                return true;
            }

            if (Repeat.HasDuration && Repeat.Duration >= _frameWatcher.Elapsed)
            {

                return true;
            }

            return false;
        }

        private void RenderAnimation(Animation animation, SKCanvas canvas)
        {
#if DEBUG_TIMING
            var renderTimeStart = _frameWatcher.Elapsed.TotalMilliseconds;
#endif

            canvas.Clear();

#if DEBUG_BACKGROUND
            canvas.DrawRect(_renderSize, _debugPaint);
#endif

            animation.SeekFrameTime((float)_frameWatcher.Elapsed.TotalSeconds, null);
            animation.Render(canvas, _renderSize);

#if DEBUG_TIMING
            var renderTime = _frameWatcher.Elapsed.TotalMilliseconds - renderTimeStart;

            Log.Debug($"Frame render time: {renderTime} ms");
#endif
        }
    }
}
