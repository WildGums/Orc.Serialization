namespace Orc.Serialization.Json;

using System;
using System.IO;

public interface IJsonSerializer
{
    object? Deserialize(Stream stream, Type targetType);

    void Serialize(Stream stream, object obj);
}
