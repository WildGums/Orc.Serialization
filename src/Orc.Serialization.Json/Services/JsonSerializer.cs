namespace Orc.Serialization.Json
{
    using System;
    using System.IO;
    using System.Text.Json;

    public class JsonSerializer : IJsonSerializer
    {
        private readonly JsonSerializerOptions _options;

        public JsonSerializer(JsonSerializerSettings settings)
        {
            _options = new JsonSerializerOptions
            {
                IncludeFields = settings.IncludeFields,
            };
        }

        public object? Deserialize(Stream stream, Type targetType)
        {
            return System.Text.Json.JsonSerializer.Deserialize(stream, targetType, _options);
        }

        public void Serialize(Stream stream, object obj)
        {
            System.Text.Json.JsonSerializer.Serialize(stream, obj, _options);
        }
    }
}
