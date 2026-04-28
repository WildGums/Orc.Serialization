namespace Orc.Serialization.Yaml;

public interface IYamlSerializerFactory
{
    IYamlSerializer CreateSerializer();

    IYamlSerializer CreateSerializer(YamlSerializerSettings settings);
}
