namespace Orc
{
    using Catel.Services;
    using Catel.ThirdPartyNotices;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Orc.Serialization.Json;

    /// <summary>
    /// Core module which allows the registration of default services in the service collection.
    /// </summary>
    public static class OrcSerializationJsonModule
    {
        public static IServiceCollection AddOrcSerializationJson(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IJsonSerializerFactory, JsonSerializerFactory>();

            serviceCollection.AddSingleton<ILanguageSource>(new LanguageResourceSource("Orc.Serialization.Json", "Orc.Serialization.Json.Properties", "Resources"));

            serviceCollection.AddSingleton<IThirdPartyNotice>((x) => new LibraryThirdPartyNotice("Orc.Serialization.Json", "https://github.com/wildgums/orc.serialization"));

            return serviceCollection;
        }
    }
}
