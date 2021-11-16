[assembly: System.Resources.NeutralResourcesLanguage("en-US")]
[assembly: System.Runtime.Versioning.TargetFramework(".NETCoreApp,Version=v6.0", FrameworkDisplayName="")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.wildgums.com/orc/skia", "Orc.Skia")]
[assembly: System.Windows.Markup.XmlnsPrefix("http://schemas.wildgums.com/orc/skia", "orcskia")]
[assembly: System.Windows.ThemeInfo(System.Windows.ResourceDictionaryLocation.None, System.Windows.ResourceDictionaryLocation.SourceAssembly)]
public static class ModuleInitializer
{
    public static void Initialize() { }
}
namespace Orc.Skia
{
    public class CanvasRenderingEventArgs : System.EventArgs
    {
        public CanvasRenderingEventArgs(SkiaSharp.SKCanvas canvas) { }
        public SkiaSharp.SKCanvas Canvas { get; }
    }
    public enum LineType
    {
        Solid = 0,
        Dashed = 1,
        Dotted = 2,
    }
    public class Margin
    {
        public Margin(double margin) { }
        public Margin(int margin) { }
        public Margin(double top, double bottom, double left, double right) { }
        public Margin(int top, int bottom, int left, int right) { }
        public bool AddToStandard { get; set; }
        public int Bottom { get; set; }
        public bool IsZero { get; }
        public int Left { get; set; }
        public int Right { get; set; }
        public int Top { get; set; }
        public static Orc.Skia.Margin CurrentOrNew(Orc.Skia.Margin margin, double value = 0) { }
    }
    public static class MatrixExtensions
    {
        public static System.Windows.Media.Matrix Copy(this System.Windows.Media.Matrix matrix) { }
        public static System.Windows.Media.Matrix Multiply(this System.Windows.Media.Matrix matrix, System.Windows.Media.Matrix secondMatrix) { }
        public static System.Windows.Rect TransformRectangle(this System.Windows.Media.Matrix matrix, System.Windows.Rect rectangle) { }
    }
    public static class PointExtensions { }
    public static class RectangleExtensions
    {
        public static System.Windows.Point GetTopLeft(this System.Windows.Rect rect) { }
        public static bool IntersectsAndIsNotNeighbour(this System.Windows.Rect rect, System.Windows.Rect toCheck) { }
        public static System.Windows.Rect OffsetTopLeft(this System.Windows.Rect rect, System.Windows.Point offset) { }
        public static System.Windows.Rect OffsetTopLeft(this System.Windows.Rect rect, double offsetX, double offsetY) { }
        public static System.Windows.Rect ProduceIntersection(this System.Windows.Rect firstRect, System.Windows.Rect rect) { }
        public static System.Windows.Rect SetTopLeft(this System.Windows.Rect rect, System.Windows.Point newLocation) { }
        public static System.Windows.Rect SubtractHeight(this System.Windows.Rect rect, double value) { }
        public static System.Windows.Rect SubtractWidth(this System.Windows.Rect rect, double value) { }
        public static System.Windows.Int32Rect ToInt32Rect(this System.Windows.Rect rect) { }
    }
    public static class SKCanvasExtensions
    {
        public const float DefaultFontSize = 14F;
        public const float DefaultFontWidth = 4F;
        public static void DrawDiagonalLine(this SkiaSharp.SKCanvas canvas, System.Windows.Point begin, System.Windows.Point end, SkiaSharp.SKPaint paint) { }
        public static void DrawHorizontalLine(this SkiaSharp.SKCanvas canvas, System.Windows.Point begin, System.Windows.Point end, SkiaSharp.SKPaint paint) { }
        public static void DrawLine(this SkiaSharp.SKCanvas canvas, System.Windows.Point begin, System.Windows.Point end, SkiaSharp.SKPaint paint) { }
        public static void DrawLineThroughPoints(this SkiaSharp.SKCanvas canvas, System.Collections.Generic.IEnumerable<System.Windows.Point> points, SkiaSharp.SKPaint paint) { }
        public static void DrawText(this SkiaSharp.SKCanvas canvas, string line, System.Windows.Rect rect, System.Windows.Media.Color color, float fontSize = 14, double width = 4, double? lineSpacing = default, SkiaSharp.SKTextAlign textAlign = 0, bool clip = true) { }
        public static void DrawText(this SkiaSharp.SKCanvas canvas, string[] lines, System.Windows.Rect rect, System.Windows.Media.Color color, float fontSize = 14, double width = 4, double? lineSpacing = default, SkiaSharp.SKTextAlign textAlign = 0, bool clip = true) { }
        public static void DrawVerticalLine(this SkiaSharp.SKCanvas canvas, System.Windows.Point begin, System.Windows.Point end, SkiaSharp.SKPaint paint) { }
        public static System.Windows.Rect MeasureTextBounds(this SkiaSharp.SKCanvas canvas, string text, System.Windows.Media.Color color, float fontSize = 14, double width = 4, SkiaSharp.SKTextAlign textAlign = 0) { }
    }
    public static class SKColorExtensions
    {
        public static SkiaSharp.SKColor ToSKColor(this System.Windows.Media.Color color) { }
    }
    public static class SKMatrixExtensions { }
    public static class SKPaintHelper
    {
        public static SkiaSharp.SKPaint CreateLinePaint(double width, System.Windows.Media.Color color, Orc.Skia.LineType lineType = 0) { }
        public static SkiaSharp.SKPaint CreateTextPaint(double fontSize, double width, System.Windows.Media.Color color, SkiaSharp.SKTextAlign textAlign = 0) { }
    }
    public static class SKPointExtensions
    {
        public static System.Windows.Point ToPoint(this SkiaSharp.SKPoint point) { }
        public static System.Windows.Point ToPoint(this SkiaSharp.SKPointI point) { }
        public static SkiaSharp.SKPoint ToSKPoint(this System.Windows.Point point) { }
    }
    public static class SKRectExtensions
    {
        public static System.Windows.Int32Rect ToInt32Rect(this SkiaSharp.SKRectI rect) { }
        public static System.Windows.Rect ToRect(this SkiaSharp.SKRect rect) { }
        public static System.Windows.Rect ToRect(this SkiaSharp.SKRectI rect) { }
        public static SkiaSharp.SKRect ToSKRect(this System.Windows.Rect rect) { }
        public static SkiaSharp.SKRectI ToSKRectI(this System.Windows.Rect rect) { }
    }
    public static class SKSizeExtensions
    {
        public static System.Windows.Size ToSize(this SkiaSharp.SKSize size) { }
        public static System.Windows.Size ToSize(this SkiaSharp.SKSizeI size) { }
    }
    public class SkiaCanvas : System.Windows.Controls.Canvas
    {
        protected double DpiX;
        protected double DpiY;
        public SkiaCanvas() { }
        public bool IgnorePixelScaling { get; set; }
        public event System.EventHandler<Orc.Skia.CanvasRenderingEventArgs> Rendered;
        public event System.EventHandler<Orc.Skia.CanvasRenderingEventArgs> Rendering;
        protected SkiaSharp.SKImageInfo GetImageInfo() { }
        public System.Windows.Rect GetSurfaceBoundary() { }
        protected virtual void Initialize() { }
        public virtual void Invalidate() { }
        protected void InvalidateBitmap(SkiaSharp.SKCanvas canvas) { }
        protected virtual bool IsRenderingAllowed() { }
        protected virtual void OnRendered(SkiaSharp.SKCanvas canvas) { }
        protected virtual void OnRendering(SkiaSharp.SKCanvas canvas) { }
        protected virtual bool RecalculateDpi() { }
        protected virtual void Render(SkiaSharp.SKCanvas canvas, bool isClearCanvas) { }
        protected virtual void Resize() { }
        protected virtual void Terminate() { }
        public void Update() { }
    }
    public static class SkiaEnvironment { }
    public class SkiaException : System.Exception
    {
        public SkiaException(string message) { }
    }
    public static class WriteableBitmapExtensions { }
}