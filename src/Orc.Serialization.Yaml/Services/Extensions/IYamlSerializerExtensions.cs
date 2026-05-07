namespace Orc.Serialization.Yaml;

using System.IO;
using System.Text;

public static class IYamlSerializerExtensions
{
    /// <summary>
    /// Deserializes an object of type <typeparamref name="T"/> from the specified stream.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize.</typeparam>
    /// <param name="yamlSerializer">The YAML serializer.</param>
    /// <param name="stream">The stream containing the YAML data.</param>
    /// <returns>The deserialized object, or <c>null</c> if deserialization returns no result.</returns>
    public static T? Deserialize<T>(this IYamlSerializer yamlSerializer, Stream stream)
    {
        return (T?)yamlSerializer.Deserialize(stream, typeof(T));
    }

    /// <summary>
    /// Serializes the specified instance to a YAML string.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="yamlSerializer">The YAML serializer.</param>
    /// <param name="instance">The instance to serialize.</param>
    /// <returns>A YAML string representation of the instance.</returns>
    public static string SerializeToString<T>(this IYamlSerializer yamlSerializer, T instance)
    {
        using var stream = new MemoryStream();
        yamlSerializer.Serialize(stream, instance!);
        return Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
    }
}
