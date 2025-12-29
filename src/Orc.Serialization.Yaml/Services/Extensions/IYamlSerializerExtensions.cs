namespace Orc.Serialization.Yaml
{
    using System.IO;

    public static class IYamlSerializerExtensions
    {
        public static T? Deserialize<T>(this IYamlSerializer yamlSerializer, Stream stream)
        {
            return (T?)yamlSerializer.Deserialize(stream, typeof(T));
        }
    }
}
