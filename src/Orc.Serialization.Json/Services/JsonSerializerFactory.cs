namespace Orc.Serialization.Json
{
    public class JsonSerializerFactory : IJsonSerializerFactory
    {
        private readonly JsonSerializerSettings _defaultSettings;

        public JsonSerializerFactory()
        {
            _defaultSettings = new JsonSerializerSettings();
        }

        public IJsonSerializer CreateSerializer()
        {
            return CreateSerializer(_defaultSettings);
        }

        public IJsonSerializer CreateSerializer(JsonSerializerSettings settings)
        {
            var serializer = new JsonSerializer(settings);
            return serializer;
        }
    }
}
