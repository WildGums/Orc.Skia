namespace Orc.Skia.Example.ViewModels;

using System;
using System.Threading.Tasks;
using Catel.MVVM;
using SkiaSharp.Skottie;

internal sealed class LottieAnimationsTestViewModel : ViewModelBase
{
    public LottieAnimationsTestViewModel(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        SetMouseOverBehaviorCommand = new TaskCommand<string>(serviceProvider, OnSetMouseOverBehaviorCommandExecuteAsync);
        SelectedFile = "/Orc.Skia.Example;component/Resources/Lottie/loading-files.json";
    }

    public string SelectedFile { get; set; }

    public Animation AnimationInPlayback { get; set; }

    public AnimationMouseOverBehavior MouseOverBehavior { get; set; }

    public TaskCommand<string> SetMouseOverBehaviorCommand { get; set; }

    private Task OnSetMouseOverBehaviorCommandExecuteAsync(string arg)
    {
        MouseOverBehavior = System.Enum.Parse<AnimationMouseOverBehavior>(arg);
        return Task.CompletedTask;
    }
}
