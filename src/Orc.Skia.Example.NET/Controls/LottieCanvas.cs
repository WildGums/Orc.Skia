namespace Orc.Skia.Example.Controls
{
    using System;
    using System.Diagnostics;
    using System.Windows.Threading;
    using Catel.Logging;
    using SkiaSharp;
    using SkiaSharp.Skottie;

    public class LottieCanvas : SkiaCanvas
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private Animation _animation;
        private readonly DispatcherTimer _timer = new();
        private readonly Stopwatch _watch = new();

        public void SetAnimation(Animation animation)
        {
            ArgumentNullException.ThrowIfNull(animation);

            _animation = animation;
            _animation.Seek(0, null);
           
            _timer.Interval = TimeSpan.FromSeconds(Math.Max(1 / 60.0, 1 / animation.Fps));
            _timer.Tick += (s, e) => Invalidate();

            _timer.Start();
            _watch.Start();
        }

        protected override void Render(SKCanvas canvas, bool isClearCanvas)
        {
            if (_animation is null)
            {
                return;
            }

            _animation.SeekFrameTime((float)_watch.Elapsed.TotalSeconds, null);

            if (_watch.Elapsed > _animation.Duration)
            {
                _watch.Restart();
            }

            var renderTimeStart = _watch.Elapsed.TotalMilliseconds;

            _animation.Render(canvas, new SKRect(0, 0, (float)ActualWidth, (float)ActualHeight));

            var renderTime = _watch.Elapsed.TotalMilliseconds - renderTimeStart;

            Log.Info($"Frame render time: {renderTime} ms");
        }
    }
}
