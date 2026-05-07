namespace Orc.Serialization.Json;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Catel.Reflection;

internal sealed class SerializerBinderJsonConverterFactory : JsonConverterFactory
{
    private readonly ISerializerBinder _serializerBinder;

    public SerializerBinderJsonConverterFactory(ISerializerBinder serializerBinder)
    {
        ArgumentNullException.ThrowIfNull(serializerBinder);

        _serializerBinder = serializerBinder;
    }

    public override bool CanConvert(Type typeToConvert)
    {
        ArgumentNullException.ThrowIfNull(typeToConvert);

        // Intentionally broad so binder validation applies to all payloads.
        return typeToConvert != typeof(Type);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(typeToConvert);
        ArgumentNullException.ThrowIfNull(options);

        var converterType = typeof(SerializerBinderJsonConverter<>).MakeGenericType(typeToConvert);
        return (JsonConverter)Activator.CreateInstance(converterType, _serializerBinder)!;
    }

    private sealed class SerializerBinderJsonConverter<T> : JsonConverter<T>
    {
        private readonly ISerializerBinder _serializerBinder;

        public SerializerBinderJsonConverter(ISerializerBinder serializerBinder)
        {
            _serializerBinder = serializerBinder;
        }

        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);
            var rootElement = document.RootElement;
            var typeToValidate = ResolveTypeToValidate(rootElement);

            EnsureTypeIsAllowed(typeToValidate, "deserialize");

            return System.Text.Json.JsonSerializer.Deserialize<T>(rootElement.GetRawText(), CreateOptionsWithoutBinderConverter(options));
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (value is not null)
            {
                EnsureTypeIsAllowed(value.GetType(), "serialize");
            }

            System.Text.Json.JsonSerializer.Serialize(writer, value, CreateOptionsWithoutBinderConverter(options));
        }

        private void EnsureTypeIsAllowed(Type type, string operation)
        {
            if (!_serializerBinder.IsTypeAllowed(type))
            {
                throw new NotSupportedException($"Cannot {operation} type '{type.FullName}' because it is not allowed by the serializer binder.");
            }
        }

        private static Type ResolveTypeToValidate(JsonElement rootElement)
        {
            if (rootElement.ValueKind == JsonValueKind.Object &&
                rootElement.TryGetProperty("__type", out var typeElement))
            {
                var runtimeTypeName = typeElement.GetString();
                if (!string.IsNullOrWhiteSpace(runtimeTypeName))
                {
                    var runtimeType = TypeCache.GetType(runtimeTypeName, false);
                    if (runtimeType is not null)
                    {
                        return runtimeType;
                    }

                    throw new JsonException($"The type '{runtimeTypeName}' cannot be resolved.");
                }
            }

            return typeof(T);
        }

        private static JsonSerializerOptions CreateOptionsWithoutBinderConverter(JsonSerializerOptions options)
        {
            var clonedOptions = new JsonSerializerOptions(options);

            if (clonedOptions.Converters.Count > 0 && clonedOptions.Converters[0] is SerializerBinderJsonConverterFactory converterAtFirstPosition)
            {
                clonedOptions.Converters.Remove(converterAtFirstPosition);
                return clonedOptions;
            }

            for (var i = 0; i < clonedOptions.Converters.Count; i++)
            {
                if (clonedOptions.Converters[i] is SerializerBinderJsonConverterFactory converter)
                {
                    clonedOptions.Converters.Remove(converter);
                    break;
                }
            }

            return clonedOptions;
        }
    }
}
