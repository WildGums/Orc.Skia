// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenderingEnginePlayGroundView.xaml.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Skia.Example.Controls
{
    using System;

#if NETFX_CORE
    using Windows.Foundation;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;
    using UiEventArgs = Windows.UI.Xaml.RoutedEventArgs;
#else
    using System.Windows;
    using System.Windows.Media;
    using UiEventArgs = System.EventArgs;
#endif

    public partial class RenderingEnginePlayGroundView
    {
        #region Constructors
        public RenderingEnginePlayGroundView()
        {
            InitializeComponent();
        }
        #endregion

        #region Methods
        private void Clear(object sender, UiEventArgs e)
        {
            //skiaCanvas.Clear();
        }

        private void OnDraw10pxLineClick(object sender, UiEventArgs e)
        {
            //skiaCanvas.Clear();
            //skiaCanvas.DrawHorizontalLine(new Point(0, 0), new Point(1, 0), 10, Colors.Black);
        }

        private void OnDraw1pxLineClick(object sender, UiEventArgs e)
        {
            //skiaCanvas.Clear();
            //skiaCanvas.DrawHorizontalLine(new Point(0, 0), new Point(1, 0), 1, Colors.Black);
        }

        private void OnDraw1pxWithoutTransformClick(object sender, UiEventArgs e)
        {
            //skiaCanvas.Clear();
            //var y = skiaCanvas.OnePixelHeigthIn01Range*10;
            //skiaCanvas.Transform = new Matrix(1, 0, 0, 10, 0, 0);
            //skiaCanvas.DrawHorizontalLine(new Point(0, y), new Point(1, y), 10, Colors.Black);
            //skiaCanvas.Transform = null;
        }

        private void OnDraw1pxWithTransformClick(object sender, UiEventArgs e)
        {
            //skiaCanvas.Clear();
            //skiaCanvas.Transform = new Matrix(1, 0, 0, 10, 0, 0);
            //skiaCanvas.DrawHorizontalLine(new Point(0, skiaCanvas.OnePixelHeigthIn01Range*10), new Point(1, skiaCanvas.OnePixelHeigthIn01Range*10), 10, Colors.Black);
            //skiaCanvas.Transform = null;
        }
        #endregion
    }
}