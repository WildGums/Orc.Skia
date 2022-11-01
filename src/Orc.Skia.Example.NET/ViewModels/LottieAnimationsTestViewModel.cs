namespace Orc.Skia.Example.ViewModels
{
    using System;
    using Catel;
    using Catel.MVVM;
    using Catel.Services;
    using Orc.FileSystem;
    using Orc.Skia.Example.Controls;

    internal sealed class LottieAnimationsTestViewModel : ViewModelBase
    {
        private readonly IFileService _fileService;
        private readonly IMessageService _messageService;

        public LottieAnimationsTestViewModel(IFileService fileService, IMessageService messageService)
        {
            Argument.IsNotNull(() => fileService);
            Argument.IsNotNull(() => messageService);

            _fileService = fileService;
            _messageService = messageService;
        }

        public string SelectedFile { get; set; }

        public string Version { get; private set; }
        public TimeSpan Duration { get; private set; }
        public double Fps { get; private set; }

        public LottieCanvas AnimatedCanvas { get; set; }

        private void OnSelectedFileChanged()
        {
            using var fileStream = _fileService.OpenRead(SelectedFile);
            using var skiaStream = new SkiaSharp.SKManagedStream(fileStream);

            if (SkiaSharp.Skottie.Animation.TryCreate(skiaStream, out var animation))
            {
                Version = animation.Version;
                Duration = animation.Duration;
                Fps = animation.Fps;
            }
            else
            {
                _messageService.ShowErrorAsync("File content cannot be recognized as Lottie format");
                return;
            }

            AnimatedCanvas.SetAnimation(animation);
        }
    }
}
