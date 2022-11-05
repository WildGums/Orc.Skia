namespace Orc.Skia.Example.ViewModels
{
    using Catel.MVVM;
    using SkiaSharp.Skottie;

    internal sealed class LottieAnimationsTestViewModel : ViewModelBase
    {
        public string SelectedFile { get; set; }

        public Animation AnimationInPlayback { get; set; }
    }
}
