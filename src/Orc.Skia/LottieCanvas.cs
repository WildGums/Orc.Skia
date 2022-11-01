namespace Orc.Skia
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Web;
    using System.Windows;
    using System.Windows.Threading;
    using Catel;
    using Catel.Logging;
    using SkiaSharp;
    using SkiaSharp.Skottie;

    public class LottieCanvas : SkiaCanvas
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private const float FramePerSeconds = 60;

        private readonly DispatcherTimer _timer = new();
        private readonly Stopwatch _watch = new();

        public Animation Animation
        {
            get
            {
                return (Animation)GetValue(AnimationProperty);
            }
            set
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("An attempt ot modify read-only property. Use \"OneWayToSource\" Mode Binding");
            }
        }

        public static readonly DependencyProperty AnimationProperty =
            DependencyProperty.Register(nameof(Animation), typeof(Animation), typeof(LottieCanvas), new PropertyMetadata(null));

        public Uri UriSource
        {
            get { return (Uri)GetValue(UriSourceProperty); }
            set { SetValue(UriSourceProperty, value); }
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
            get { return (Stream)GetValue(StreamSourceProperty); }
            set { SetValue(StreamSourceProperty, value); }
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

            _timer.Interval = TimeSpan.FromSeconds(Math.Max(1 / FramePerSeconds, 1 / animation.Fps));
            _timer.Tick += (s, e) => Invalidate();

            _timer.Start();
            _watch.Start();
        }

        protected override void Render(SKCanvas canvas, bool isClearCanvas)
        {
            var animation = Animation;
            if (animation is null)
            {
                return;
            }

            if (_watch.Elapsed > animation.Duration)
            {
                _watch.Restart();
            }

            RenderAnimation(animation, canvas);
        }

        private void RenderAnimation(Animation animation, SKCanvas canvas)
        {
            Argument.IsNotNull(() => animation);

            var renderTimeStart = _watch.Elapsed.TotalMilliseconds;

            animation.SeekFrameTime((float)_watch.Elapsed.TotalSeconds, null);

            animation.Render(canvas, new SKRect(0, 0, (float)ActualWidth, (float)ActualHeight));

            var renderTime = _watch.Elapsed.TotalMilliseconds - renderTimeStart;

            Log.Debug($"Frame render time: {renderTime} ms");
        }
    }
}
