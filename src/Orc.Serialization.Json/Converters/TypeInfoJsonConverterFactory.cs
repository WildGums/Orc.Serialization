namespace Orc.Serialization.Json;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Catel.Reflection;

internal sealed class TypeInfoJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        ArgumentNullException.ThrowIfNull(typeToConvert);

        return typeToConvert == typeof(object) || typeToConvert.IsInterface || typeToConvert.IsAbstract;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(typeToConvert);
        ArgumentNullException.ThrowIfNull(options);

        var converterType = typeof(TypeInfoJsonConverter<>).MakeGenericType(typeToConvert);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }

    private sealed class TypeInfoJsonConverter<T> : JsonConverter<T>
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            using var document = JsonDocument.ParseValue(ref reader);
            var rootElement = document.RootElement;

            if (TryGetTypeInfoPayload(rootElement, out var typeName, out var objectElement))
            {
                var runtimeType = TypeCache.GetType(typeName!, false);
                if (runtimeType is null || !typeof(T).IsAssignableFrom(runtimeType))
                {
                    throw new JsonException($"The type '{typeName}' cannot be resolved or is not assignable to '{typeof(T).FullName}'.");
                }

                var value = System.Text.Json.JsonSerializer.Deserialize(objectElement.GetRawText(), runtimeType, CreateOptionsWithoutTypeInfoConverter(options));
                return (T?)value;
            }

            return System.Text.Json.JsonSerializer.Deserialize<T>(rootElement.GetRawText(), CreateOptionsWithoutTypeInfoConverter(options));
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            var declaredType = typeof(T);
            var runtimeType = value.GetType();

            if (!ShouldWriteTypeInfo(declaredType, runtimeType))
            {
                System.Text.Json.JsonSerializer.Serialize(writer, value, CreateOptionsWithoutTypeInfoConverter(options));
                return;
            }

            writer.WriteStartObject();
            writer.WriteString("$type", runtimeType.GetSafeFullName());

            var temp = System.Text.Json.JsonSerializer.SerializeToNode(value, runtimeType, CreateOptionsWithoutTypeInfoConverter(options))!.AsObject();

            foreach (var kvp in temp)
            {
                writer.WritePropertyName(kvp.Key);
                if (kvp.Value is null)
                {
                    writer.WriteNullValue();
                }
                else
                {
                    kvp.Value.WriteTo(writer);
                }
            }

            writer.WriteEndObject();
        }

        private static bool ShouldWriteTypeInfo(Type declaredType, Type runtimeType)
        {
            return declaredType == typeof(object) || declaredType.IsInterface || declaredType.IsAbstract || declaredType != runtimeType;
        }

        private static bool TryGetTypeInfoPayload(JsonElement rootElement, out string? typeName, out JsonElement objectElement)
        {
            typeName = null;
            objectElement = default;

            if (rootElement.ValueKind != JsonValueKind.Object)
            {
                return false;
            }

            if (!rootElement.TryGetProperty("$type", out var typeElement))
            {
                return false;
            }

            objectElement = rootElement;

            typeName = typeElement.GetString();
            return !string.IsNullOrWhiteSpace(typeName);
        }

        private static JsonSerializerOptions CreateOptionsWithoutTypeInfoConverter(JsonSerializerOptions options)
        {
            var clonedOptions = new JsonSerializerOptions(options)
            {
                // Need to skip because of $type
                UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip
            };

            for (var i = clonedOptions.Converters.Count - 1; i >= 0; i--)
            {
                if (clonedOptions.Converters[i] is TypeInfoJsonConverterFactory converter)
                {
                    clonedOptions.Converters.Remove(converter);
                    break;
                }
            }

            return clonedOptions;
        }
    }
}
