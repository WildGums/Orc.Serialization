namespace Orc.Serialization.Json;

using System;
using System.IO;

public interface IJsonSerializer
{
    object? Deserialize(Stream stream, Type targetType);

    T? Deserialize<T>(Stream stream);

    void Serialize(Stream stream, object obj);

    void Serialize<T>(Stream stream, T obj);

    void PopulateObject(Stream stream, object target);
}
