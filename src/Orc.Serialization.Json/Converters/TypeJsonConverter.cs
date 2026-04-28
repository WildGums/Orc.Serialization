namespace Orc.Serialization.Json;

using System;
using System.Text.Json;
using Catel.Reflection;

public class TypeJsonConverter : System.Text.Json.Serialization.JsonConverter<Type?>
{
    public override Type? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();
        if (string.IsNullOrEmpty(stringValue))
        {
            return null;
        }

        return TypeCache.GetType(stringValue, false);
    }
            
    public override void Write(Utf8JsonWriter writer, Type? type, JsonSerializerOptions options)
    {
        if (type is null)
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteStringValue(type.FullName);
        }
    }
}
