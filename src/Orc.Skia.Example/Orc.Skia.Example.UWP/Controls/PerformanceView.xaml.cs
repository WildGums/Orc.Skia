// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PerformanceView.xaml.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Skia.Example.Controls
{
    using System.Diagnostics;

#if NETFX_CORE
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using UiEventArgs = Windows.UI.Xaml.RoutedEventArgs;
#else
    using System.Windows;
    using System.Windows.Controls;
    using UiEventArgs = System.EventArgs;
#endif

    public sealed partial class PerformanceView
    {
        #region Constructors
        public PerformanceView()
        {
            InitializeComponent();

            skiaCanvas.Rendering += OnSkiaCanvasRendering;
        }
        #endregion

        private void RenderClick(object sender, UiEventArgs e)
        {
            var count = int.Parse((string)((Button)sender).Tag);

            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < count; i++)
            {
                skiaCanvas.Invalidate();
            }

            stopwatch.Stop();

            var average = (stopwatch.ElapsedMilliseconds / (double)count);
            var fps = 1000 / average;

            durationAverageTextBlock.Text = $"{average} ms";
            durationTotalTextBlock.Text = $"{(stopwatch.ElapsedMilliseconds)} ms";
            fpsTextBlock.Text = $"{fps} fps";
        }

        private void OnSkiaCanvasRendering(object sender, CanvasRenderingEventArgs e)
        {
            CanvasTest.RunTests(e.Canvas);
        }
    }
}