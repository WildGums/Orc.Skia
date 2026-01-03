namespace Orc.Skia
{
    using Catel.Services;
    using Catel.ThirdPartyNotices;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Core module which allows the registration of default services in the service collection.
    /// </summary>
    public static class OrcSkiaModule
    {
        public static IServiceCollection AddOrcSkia(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ILanguageSource>(new LanguageResourceSource("Orc.Skia", "Orc.Skia.Properties", "Resources"));

            serviceCollection.AddSingleton<IThirdPartyNotice>((x) => new ResourceBasedThirdPartyNotice("SkiaSharp", "https://github.com/mono/SkiaSharp", "Orc.Skia", "Orc.Skia", "Resources.ThirdPartyNotices.catel.txt"));
            serviceCollection.AddSingleton<IThirdPartyNotice>((x) => new LibraryThirdPartyNotice("Orc.Skia", "https://github.com/wildgums/orc.skia"));

            return serviceCollection;
        }
    }
}
