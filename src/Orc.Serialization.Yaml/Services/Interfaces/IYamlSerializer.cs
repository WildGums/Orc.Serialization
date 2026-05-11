namespace Orc.Serialization.Yaml;

using System;
using System.IO;

public interface IYamlSerializer
{
    object? Deserialize(Stream stream, Type targetType);

    T? Deserialize<T>(Stream stream);

    void Serialize(Stream stream, object obj);

    void Serialize<T>(Stream stream, T obj);
}
