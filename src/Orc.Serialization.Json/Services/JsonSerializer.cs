namespace Orc.Serialization.Json;

using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

public class JsonSerializer : IJsonSerializer
{
    private readonly JsonSerializerOptions _options;

    public JsonSerializer(JsonSerializerSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        _options = new JsonSerializerOptions
        {
            IncludeFields = settings.IncludeFields,
            MaxDepth = settings.MaxDepth,
            WriteIndented = settings.WriteIndented,
            PropertyNameCaseInsensitive = settings.PropertyNameCaseInsensitive,
        };

        if (settings.SerializerBinder is not null)
        {
            _options.Converters.Insert(0, new SerializerBinderJsonConverterFactory(settings.SerializerBinder));
        }

        if (settings.UseTypeInfoConverter)
        {
            _options.Converters.Add(new TypeInfoJsonConverterFactory());
        }

        if (settings.SerializeEnumsAsStrings)
        {
            _options.Converters.Add(new JsonStringEnumConverter());
        }

        foreach (var typeInfoResolver in settings.TypeInfoResolverChain)
        {
            _options.TypeInfoResolverChain.Add(typeInfoResolver);
        }
    }

    public object? Deserialize(Stream stream, Type targetType)
    {
        return System.Text.Json.JsonSerializer.Deserialize(stream, targetType, _options);
    }

    public void Serialize(Stream stream, object obj)
    {
        System.Text.Json.JsonSerializer.Serialize(stream, obj, _options);
    }

    public void Serialize<T>(Stream stream, T obj)
    {
        System.Text.Json.JsonSerializer.Serialize(stream, obj, _options);
    }
}
