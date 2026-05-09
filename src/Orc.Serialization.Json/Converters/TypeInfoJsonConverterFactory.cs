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

                // The factory stays in options so any nested abstract/interface members trigger it again.
                // The factory's CanConvert returns false for the concrete runtimeType, so no recursion happens here.
                // UnmappedMemberHandling is set to Skip because the JSON contains $type which has no mapping on runtimeType.
                var value = System.Text.Json.JsonSerializer.Deserialize(objectElement.GetRawText(), runtimeType, CloneWithSkipUnmapped(options));
                return (T?)value;
            }

            // No $type marker — defer to default STJ which throws "abstract types not supported" for interface/abstract T.
            // Remove the factory so it doesn't re-enter for the same T.
            return System.Text.Json.JsonSerializer.Deserialize<T>(rootElement.GetRawText(), CloneWithoutFactory(options));
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
                System.Text.Json.JsonSerializer.Serialize(writer, value, CloneWithoutFactory(options));
                return;
            }

            writer.WriteStartObject();
            writer.WriteString("$type", runtimeType.GetSafeFullName());

            // The factory stays in options so any nested abstract/interface members trigger it again.
            // For typeof(object) (the only instantiable type the factory matches), strip the factory to avoid infinite recursion.
            var nestedOptions = runtimeType == typeof(object) ? CloneWithoutFactory(options) : options;
            var temp = System.Text.Json.JsonSerializer.SerializeToNode(value, runtimeType, nestedOptions)!.AsObject();

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

        private static JsonSerializerOptions CloneWithSkipUnmapped(JsonSerializerOptions options)
        {
            return new JsonSerializerOptions(options)
            {
                UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip
            };
        }

        private static JsonSerializerOptions CloneWithoutFactory(JsonSerializerOptions options)
        {
            var clonedOptions = new JsonSerializerOptions(options);

            for (var i = clonedOptions.Converters.Count - 1; i >= 0; i--)
            {
                if (clonedOptions.Converters[i] is TypeInfoJsonConverterFactory)
                {
                    clonedOptions.Converters.RemoveAt(i);
                    break;
                }
            }

            return clonedOptions;
        }
    }
}
