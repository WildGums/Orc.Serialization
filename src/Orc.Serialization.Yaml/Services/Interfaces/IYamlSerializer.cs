namespace Orc.Serialization.Yaml
{
    using System;
    using System.IO;

    public interface IYamlSerializer
    {
        object? Deserialize(Stream stream, Type targetType);

        void Serialize(Stream stream, object obj);
    }
}
