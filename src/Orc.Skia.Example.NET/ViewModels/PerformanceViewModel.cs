namespace Orc.Skia.Example.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System.Windows;
    using Catel.MVVM;

    public class PerformanceViewModel : ViewModelBase
    {
        private readonly Random _random = new Random();

        public PerformanceViewModel()
        {
            RunTests = new TaskCommand<string>(OnRunTestsExecuteAsync);

            PerformanceTests = new List<PerformanceTest>(new[]
            {
                new PerformanceTest
                {
                    Name = "SkiaElement (SkiaSharp)",
                    CanvasElement = new SkiaElement()
                },

                new PerformanceTest
                {
                    Name = "SkiaCanvas (Raster)",
                    CanvasElement = new SkiaCanvas
                    {
                        FrameDelayInMilliseconds = 0, // for performance test only
                        RenderingType = RenderingType.Raster
                    }
                },

                new PerformanceTest
                {
                    Name = "SkiaCanvas (Vulkan)",
                    CanvasElement = new SkiaCanvas
                    {
                        FrameDelayInMilliseconds = 0, // for performance test only
                        RenderingType = RenderingType.Vulkan
                    }
                },

                new PerformanceTest
                {
                    Name = "SkiaCanvas (OpenGL)",
                    CanvasElement = new SkiaCanvas
                    {
                        FrameDelayInMilliseconds = 0, // for performance test only
                        RenderingType = RenderingType.OpenGL
                    }
                }
            });
        }

        public List<PerformanceTest> PerformanceTests { get; private set; }

        public TaskCommand<string> RunTests { get; private set; }

        private async Task OnRunTestsExecuteAsync(string numberOfFramesAsString)
        {
            var numberOfFrames = int.Parse(numberOfFramesAsString);

            foreach (var performanceTest in PerformanceTests)
            {
                performanceTest.IsRunningTests = true;

                var skiaElement = performanceTest.CanvasElement;
                var skiaFxElement = (FrameworkElement)skiaElement;
                var performanceTestResult = new PerformanceTestResult();

                TestTemp.CurrentRenderingType = (skiaElement as SkiaCanvas)?.RenderingType;

                skiaElement.Rendering += OnSkiaRendering;

                for (var i = 0; i < numberOfFrames; i++)
                {
                    var tsc = new TaskCompletionSource();

                    EventHandler<CanvasRenderingEventArgs> handler = null;
                    handler = (sender, e) =>
                    {
                        skiaElement.Rendered -= handler;
                        tsc.SetResult();
                    };

                    skiaElement.Rendered += handler;

                    var stopwatch = Stopwatch.StartNew();

                    // Instead of calling Update, call InvalidateRect for fair comparison
                    skiaFxElement.InvalidateVisual();
                    //skiaElement.Update();

                    await tsc.Task;

                    stopwatch.Stop();

                    performanceTestResult.RegisterFrame((int)stopwatch.ElapsedMilliseconds);
                }

                skiaElement.Rendering -= OnSkiaRendering;

                performanceTest.Result = performanceTestResult;

                // Pause for better results
                await Task.Delay(100);

                performanceTest.IsRunningTests = false;
            }
        }

        private void OnSkiaRendering(object sender, CanvasRenderingEventArgs e)
        {
            var left = _random.Next(0, 100);
            var top = _random.Next(0, 100);

            CanvasTest.RunTests(e.Canvas, left, top);
        }
    }
}
