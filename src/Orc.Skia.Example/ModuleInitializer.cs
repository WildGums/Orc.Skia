using Catel.IoC;
using Orchestra;
using Orchestra.Services;
/// <summary>
/// Used by the ModuleInit. All code inside the Initialize method is ran as soon as the assembly is loaded.
/// </summary>
public static class ModuleInitializer
{
    /// <summary>
    /// Initializes the module.
    /// </summary>
    public static void Initialize()
    {
        var serviceLocator = ServiceLocator.Default;

        var thirdPartyNoticesService = serviceLocator.ResolveType<IThirdPartyNoticesService>();
        thirdPartyNoticesService.AddWithTryCatch(() => new FontThirdPartyNotice("Lottie: Loading files by ElHanif", "https://lottiefiles.com/99297-loading-files"));
    }
}
