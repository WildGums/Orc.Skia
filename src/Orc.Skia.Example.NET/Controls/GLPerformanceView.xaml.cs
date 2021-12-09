namespace Orc.Skia.Example.Controls
{
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;

    using UiEventArgs = System.EventArgs;

    /// <summary>
    /// Interaction logic for GLPerformanceView.xaml
    /// </summary>
    public partial class GLPerformanceView
    {
        #region Constructors
        public GLPerformanceView()
        {
            InitializeComponent();

            GLSkCanvas.Rendering += OnSkiaCanvasRendering;
        }
        #endregion

        #region Methods
        private void RenderClick(object sender, UiEventArgs e)
        {
            var count = int.Parse((string)((Button)sender).Tag);

            var stopwatch = Stopwatch.StartNew();

            for (var i = 0; i < count; i++)
            {
                GLSkCanvas.Update();
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
        #endregion
    }
}
