namespace Orc.Serialization.Yaml
{
    using System.Xml.Serialization;
    using Catel.Services;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    /// <summary>
    /// Core module which allows the registration of default services in the service collection.
    /// </summary>
    public static class OrcSerializationYamlModule
    {
        public static IServiceCollection AddOrcSerializationYaml(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IYamlSerializerFactory, YamlSerializerFactory>();

            serviceCollection.AddSingleton<ILanguageSource>(new LanguageResourceSource("Orc.Serialization.Yaml", "Orc.Serialization.Yaml.Properties", "Resources"));

            return serviceCollection;
        }
    }
}
