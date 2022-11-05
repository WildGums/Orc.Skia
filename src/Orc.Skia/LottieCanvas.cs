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

        private const float FramePerSeconds = 60;

        private readonly DispatcherTimer _invalidationTimer = new();
        private readonly Stopwatch _frameWatcher = new();

        private int _repeatCount = 0;

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

        public bool IsPlaying
        {
            get => (bool)GetValue(IsPlayingProperty);
            set => throw Log.ErrorAndCreateException<InvalidOperationException>($"An attempt to modify read-only property \"{nameof(IsPlaying)}\". Use \"OneWayToSource\" Mode Binding");
        }

        public static readonly DependencyProperty IsPlayingProperty =
            DependencyProperty.Register(nameof(IsPlaying), typeof(bool), typeof(LottieCanvas), new PropertyMetadata(false));

        public Uri UriSource
        {
            get => (Uri)GetValue(UriSourceProperty);
            set => SetValue(UriSourceProperty, value);
        }

        public static readonly DependencyProperty UriSourceProperty =
            DependencyProperty.Register(nameof(UriSource), typeof(Uri), typeof(LottieCanvas), new PropertyMetadata((s, e) => ((LottieCanvas)s).OnUriSourceChanged()));

        private void OnUriSourceChanged()
        {
            try
            {
                var uri = UriSource;

                // Sync sources
                SetCurrentValue(StreamSourceProperty, null);

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

        public Stream StreamSource
        {
            get => (Stream)GetValue(StreamSourceProperty);
            set => SetValue(StreamSourceProperty, value);
        }

        public static readonly DependencyProperty StreamSourceProperty =
            DependencyProperty.Register(nameof(StreamSource), typeof(Stream), typeof(LottieCanvas), new PropertyMetadata((s, e) => ((LottieCanvas)s).OnStreamSourceChanged()));

        private void OnStreamSourceChanged()
        {
            try
            {
                // Sync sources
                SetCurrentValue(UriSourceProperty, null);
                InitializeAnimationFromSource(StreamSource);
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
            Argument.IsNotNull(() => animation);

            animation.Seek(0, null);

            SetCurrentValue(AnimationProperty, animation);

            _invalidationTimer.Interval = TimeSpan.FromSeconds(Math.Max(1 / FramePerSeconds, 1 / animation.Fps));
            _invalidationTimer.Tick += (s, e) => Invalidate();

            StartAnimation();
        }

        public void StartAnimation()
        {
            _repeatCount++;

            SetCurrentValue(IsPlayingProperty, true);

            _invalidationTimer.Start();
            _frameWatcher.Restart();
        }


        public void StopAnimation()
        {
            SetCurrentValue(IsPlayingProperty, false);

            _invalidationTimer.Stop();
            _frameWatcher.Stop();
        }

        protected override void Render(SKCanvas canvas, bool isClearCanvas)
        {
            var animation = Animation;
            if (animation is null)
            {
                return;
            }

            if (_frameWatcher.Elapsed > animation.Duration)
            {
                if (CanRestart())
                {
                    StartAnimation();
                }
                else
                {
                    StopAnimation();
                }
            }

            RenderAnimation(animation, canvas);
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
            else
            {
                if (Repeat.HasCount && Repeat.Count < _repeatCount)
                {
                    return true;
                }

                if (Repeat.HasDuration && Repeat.Duration < _frameWatcher.Elapsed)
                {

                    return true;
                }

                return false;
            }
        }

        private void RenderAnimation(Animation animation, SKCanvas canvas)
        {
            Argument.IsNotNull(() => animation);

            var renderTimeStart = _frameWatcher.Elapsed.TotalMilliseconds;

            animation.SeekFrameTime((float)_frameWatcher.Elapsed.TotalSeconds, null);

            animation.Render(canvas, new SKRect(0, 0, (float)ActualWidth, (float)ActualHeight));

#if DEBUG
            var renderTime = _frameWatcher.Elapsed.TotalMilliseconds - renderTimeStart;

            Log.Debug($"Frame render time: {renderTime} ms");
#endif
        }
    }
}
