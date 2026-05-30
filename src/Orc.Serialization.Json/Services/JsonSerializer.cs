namespace Orc.Serialization.Json;

using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Catel.Collections;

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
            PreferredObjectCreationHandling = settings.PreferredObjectCreationHandling
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

        _options.Converters.AddRange(settings.Converters);
    }

    public object? Deserialize(Stream stream, Type targetType)
    {
        return System.Text.Json.JsonSerializer.Deserialize(stream, targetType, _options);
    }

    public T? Deserialize<T>(Stream stream)
    {
        return System.Text.Json.JsonSerializer.Deserialize<T>(stream, _options);
    }

    public void Serialize(Stream stream, object obj)
    {
        System.Text.Json.JsonSerializer.Serialize(stream, obj, _options);
    }

    public void Serialize<T>(Stream stream, T obj)
    {
        System.Text.Json.JsonSerializer.Serialize(stream, obj, _options);
    }

    public void PopulateObject(Stream stream, object target)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(target);

        using var document = JsonDocument.Parse(stream);
        var root = document.RootElement;

        if (root.ValueKind != JsonValueKind.Object)
        {
            return;
        }

        var type = target.GetType();
        using var enumerator = root.EnumerateObject();
        while (enumerator.MoveNext())
        {
            var jsonProperty = enumerator.Current;
            var property = type.GetProperty(jsonProperty.Name,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (property is null || !property.CanWrite)
            {
                continue;
            }

            var value = jsonProperty.Value.Deserialize(property.PropertyType, _options);
            property.SetValue(target, value);
        }
    }
}
