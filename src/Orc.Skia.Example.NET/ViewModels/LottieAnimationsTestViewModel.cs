namespace Orc.Skia.Example.ViewModels
{
    using System;
    using Catel;
    using Catel.MVVM;
    using Catel.Services;
    using Orc.FileSystem;
    using Orc.Skia.Example.Controls;
    using SkiaSharp.Skottie;

    internal sealed class LottieAnimationsTestViewModel : ViewModelBase
    {
        public LottieAnimationsTestViewModel()
        {

        }

        public string SelectedFile { get; set; }

        public Animation AnimationInPlayback { get; set; }
    }
}
