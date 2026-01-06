namespace Orc
{
    using Catel.Services;
    using Catel.ThirdPartyNotices;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Orc.Serialization.Yaml;

    /// <summary>
    /// Core module which allows the registration of default services in the service collection.
    /// </summary>
    public static class OrcSerializationYamlModule
    {
        public static IServiceCollection AddOrcSerializationYaml(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IYamlSerializerFactory, YamlSerializerFactory>();

            serviceCollection.AddSingleton<ILanguageSource>(new LanguageResourceSource("Orc.Serialization.Yaml", "Orc.Serialization.Yaml.Properties", "Resources"));

            serviceCollection.AddSingleton<IThirdPartyNotice>((x) => new ResourceBasedThirdPartyNotice("YamlDotNet", "https://github.com/aaubry/YamlDotNet", "Orc.Serialization.Yaml", "Orc.Serialization.Yaml", "Resources.ThirdPartyNotices.yamldotnet.txt"));
            serviceCollection.AddSingleton<IThirdPartyNotice>((x) => new LibraryThirdPartyNotice("Orc.Serialization.Yaml", "https://github.com/wildgums/orc.serialization"));

            return serviceCollection;
        }
    }
}
