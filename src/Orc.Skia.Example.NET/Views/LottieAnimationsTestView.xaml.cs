namespace Orc.Skia.Example.Views
{
    using Orc.Skia.Example.ViewModels;

    public partial class LottieAnimationsTestView
    {
        public LottieAnimationsTestView()
        {
            InitializeComponent();
        }

        protected override void OnViewModelChanged()
        {
            base.OnViewModelChanged();

            if (ViewModel is LottieAnimationsTestViewModel vm)
            {
                vm.AnimatedCanvas = canvas;
            }
        }
    }
}
