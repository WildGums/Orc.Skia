namespace Orc.Skia.Example
{
    using Catel.Data;

    public class PerformanceTest : ModelBase
    {
        public string Name { get; set; }

        public ISkiaElement CanvasElement { get; set; }

        public PerformanceTestResult Result { get; set; }

        public bool IsRunningTests { get; set; }
    }
}
