// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LineCanvasView.xaml.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Skia.Example.Controls
{
#if NETFX_CORE
    using Windows.UI.Xaml;
    using UiEventArgs = Windows.UI.Xaml.RoutedEventArgs;
#else
    using UiEventArgs = System.EventArgs;
#endif

    public partial class LineCanvasView
    {
        #region Constructors
        public LineCanvasView()
        {
            InitializeComponent();
        }
        #endregion

        #region Methods
        private void Clear(object sender, UiEventArgs e)
        {
            //LineCanvasTest.Clear();
        }

        private void RunTests(object sender, UiEventArgs e)
        {
            //CanvasTest.RunTests(LineCanvasTest);
        }

        private void RunZigZagTests(object sender, UiEventArgs e)
        {
            //CanvasTest.RunZigZagTests(LineCanvasTest);
        }
        #endregion
    }
}