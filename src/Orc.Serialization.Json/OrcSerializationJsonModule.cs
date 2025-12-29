namespace Orc.Serialization.Json
{
    using Catel.Services;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    /// <summary>
    /// Core module which allows the registration of default services in the service collection.
    /// </summary>
    public static class OrcSerializationJsonModule
    {
        public static IServiceCollection AddOrcSerializationJson(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IJsonSerializerFactory, JsonSerializerFactory>();

            serviceCollection.AddSingleton<ILanguageSource>(new LanguageResourceSource("Orc.Serialization.Json", "Orc.Serialization.Json.Properties", "Resources"));

            return serviceCollection;
        }
    }
}
