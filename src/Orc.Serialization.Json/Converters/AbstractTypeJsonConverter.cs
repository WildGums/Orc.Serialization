namespace Orc.Serialization.Json;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Catel.Reflection;

public abstract class AbstractTypeJsonConverter<T> : JsonConverter<T>
{
    public override bool CanConvert(Type typeToConvert) =>
        typeof(T).IsAssignableFrom(typeToConvert);

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        reader.Read();
        if (reader.TokenType != JsonTokenType.PropertyName)
        {
            throw new JsonException();
        }

        var propertyName = reader.GetString();
        if (propertyName != "__type")
        {
            throw new JsonException();
        }

        reader.Read();
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException();
        }

        var itemType = TypeCache.GetType(reader.GetString()!);
        if (itemType is null)
        {
            throw new JsonException();
        }

        // Safety check, we don't want to instantiate any type
        if (!typeof(T).IsAssignableFrom(itemType))
        {
            throw new JsonException();
        }

        reader.Read();
        if (reader.TokenType != JsonTokenType.PropertyName)
        {
            throw new JsonException();
        }

        propertyName = reader.GetString();
        if (propertyName != "__object")
        {
            throw new JsonException();
        }

        reader.Read();
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        var typeInfo = options.GetTypeInfo(itemType);

        var item = System.Text.Json.JsonSerializer.Deserialize(ref reader, typeInfo);
        if (item is not T typedValue)
        {
            throw new JsonException();
        }

        reader.Read();

        return typedValue;
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();

        writer.WriteString("__type", value.GetType().GetSafeFullName());

        writer.WritePropertyName("__object");

        var typeInfo = options.GetTypeInfo(value.GetType());

        System.Text.Json.JsonSerializer.Serialize(writer, value, typeInfo);

        writer.WriteEndObject();
    }
}
