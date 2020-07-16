// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomizedCanvas.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Skia.Example.Controls
{
    using SkiaSharp;
    using Catel.Windows.Data;

#if NETFX_CORE
    using Windows.Foundation;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;
#else
    using System.Windows;
    using System.Windows.Media;
#endif

    public class CustomizedCanvas : SkiaCanvas
    {
        #region Constants
        #endregion

        public CustomizedCanvas()
        {
            this.SubscribeToDependencyProperty(nameof(RenderingProgram), OnRenderingProgramChanged);
        }

        #region Properties
        public string RenderingProgram
        {
            get { return (string)GetValue(RenderingProgramProperty); }
            set { SetValue(RenderingProgramProperty, value); }
        }

        public static readonly DependencyProperty RenderingProgramProperty = DependencyProperty.Register(nameof(RenderingProgram), typeof(string),
            typeof(CustomizedCanvas), new PropertyMetadata(null));
        #endregion

        #region Methods
        protected override Size MeasureOverride(Size constraint)
        {
            Invalidate();

            return base.MeasureOverride(constraint);
        }

        protected override void Render(SKCanvas canvas, bool isClearCanvas)
        {
            if (canvas != null)
            {
                CanvasTest.RunTests(canvas);
            }
        }

        private void OnRenderingProgramChanged(object sender, DependencyPropertyValueChangedEventArgs e)
        {
            var canvas = sender as CustomizedCanvas;

            canvas?.Invalidate();
        }
        #endregion
    }
}
