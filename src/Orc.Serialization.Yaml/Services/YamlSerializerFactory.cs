namespace Orc.Serialization.Yaml
{
    public class YamlSerializerFactory : IYamlSerializerFactory
    {
        private readonly YamlSerializerSettings _defaultSettings;

        public YamlSerializerFactory()
        {
            _defaultSettings = new YamlSerializerSettings();
        }

        public IYamlSerializer CreateSerializer()
        {
            return CreateSerializer(_defaultSettings);
        }

        public IYamlSerializer CreateSerializer(YamlSerializerSettings settings)
        {
            var serializer = new YamlSerializer(settings);
            return serializer;
        }
    }
}
