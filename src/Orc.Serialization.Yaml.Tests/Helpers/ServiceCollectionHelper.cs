namespace Orc.Serialization.Yaml.Tests;

using Catel;
using Microsoft.Extensions.DependencyInjection;
using Orc.Serialization.Yaml;

internal static class ServiceCollectionHelper
{
    public static IServiceCollection CreateServiceCollection()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddLogging();
        serviceCollection.AddCatelCore();
        serviceCollection.AddOrcSerializationYaml();

        return serviceCollection;
    }
}
