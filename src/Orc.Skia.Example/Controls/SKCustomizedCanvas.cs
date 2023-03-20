namespace Orc.Skia.Example.Controls;

using SkiaSharp;
using Catel.Windows.Data;
using System.Windows;

public class CustomizedCanvas : SkiaCanvas
{
    public CustomizedCanvas()
    {
        this.SubscribeToDependencyProperty(nameof(RenderingProgram), OnRenderingProgramChanged);
    }

    public string RenderingProgram
    {
        get { return (string)GetValue(RenderingProgramProperty); }
        set { SetValue(RenderingProgramProperty, value); }
    }

    public static readonly DependencyProperty RenderingProgramProperty = DependencyProperty.Register(nameof(RenderingProgram), typeof(string),
        typeof(CustomizedCanvas), new PropertyMetadata(null));

    protected override Size MeasureOverride(Size constraint)
    {
        Invalidate();

        return base.MeasureOverride(constraint);
    }

    protected override void Render(SKCanvas canvas, bool isClearCanvas)
    {
        CanvasTest.RunTests(canvas);
    }

    private void OnRenderingProgramChanged(object sender, DependencyPropertyValueChangedEventArgs e)
    {
        var canvas = sender as CustomizedCanvas;

        canvas?.Invalidate();
    }
}
