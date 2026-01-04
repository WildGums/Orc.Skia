namespace Orc.Skia.Tests
{
    using Catel;
    using Microsoft.Extensions.DependencyInjection;
    using Orc.Skia;

    internal static class ServiceCollectionHelper
    {
        public static IServiceCollection CreateServiceCollection()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddLogging();
            serviceCollection.AddCatelCore();
            serviceCollection.AddOrcSkia();

            return serviceCollection;
        }
    }
}
