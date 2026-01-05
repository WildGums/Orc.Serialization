namespace Orc.Serialization.Tests
{
    using Catel;
    using Microsoft.Extensions.DependencyInjection;
    using Orc.Serialization.Json;

    internal static class ServiceCollectionHelper
    {
        public static IServiceCollection CreateServiceCollection()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddLogging();
            serviceCollection.AddCatelCore();
            serviceCollection.AddOrcSerializationJson();

            return serviceCollection;
        }
    }
}
