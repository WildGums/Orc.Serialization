namespace Orc.Serialization.Json;

using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

public class JsonSerializer : IJsonSerializer
{
    private readonly JsonSerializerOptions _options;
    private readonly ISerializerBinder? _serializerBinder;

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
        _serializerBinder = settings.SerializerBinder;

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
        EnsureTypeIsAllowed(targetType, "deserialize");
        return System.Text.Json.JsonSerializer.Deserialize(stream, targetType, _options);
    }

    public void Serialize(Stream stream, object obj)
    {
        if (obj is not null)
        {
            EnsureTypeIsAllowed(obj.GetType(), "serialize");
        }

        System.Text.Json.JsonSerializer.Serialize(stream, obj, _options);
    }

    private void EnsureTypeIsAllowed(Type type, string operation)
    {
        if (_serializerBinder is null)
        {
            return;
        }

        if (!_serializerBinder.IsTypeAllowed(type))
        {
            throw new NotSupportedException($"Cannot {operation} type '{type.FullName}' because it is not allowed by the serializer binder.");
        }
    }
}
