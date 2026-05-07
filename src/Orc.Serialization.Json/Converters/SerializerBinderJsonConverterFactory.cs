namespace Orc.Serialization.Json;

using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

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
            EnsureTypeIsAllowed(typeof(T), "deserialize");

            return System.Text.Json.JsonSerializer.Deserialize<T>(ref reader, CreateOptionsWithoutBinderConverter(options));
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

        private static JsonSerializerOptions CreateOptionsWithoutBinderConverter(JsonSerializerOptions options)
        {
            var clonedOptions = new JsonSerializerOptions(options);
            var converter = clonedOptions.Converters.OfType<SerializerBinderJsonConverterFactory>().FirstOrDefault();
            if (converter is not null)
            {
                clonedOptions.Converters.Remove(converter);
            }

            return clonedOptions;
        }
    }
}
