namespace Orc.Serialization.Json;

using System.IO;
using System.Text;

public static class IJsonSerializerExtensions
{
    /// <summary>
    /// Deserializes an object of type <typeparamref name="T"/> from the specified stream.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize.</typeparam>
    /// <param name="jsonSerializer">The JSON serializer.</param>
    /// <param name="stream">The stream containing the JSON data.</param>
    /// <returns>The deserialized object, or <c>null</c> if deserialization returns no result.</returns>
    public static T? Deserialize<T>(this IJsonSerializer jsonSerializer, Stream stream)
    {
        return (T?)jsonSerializer.Deserialize(stream, typeof(T));
    }

    /// <summary>
    /// Serializes the specified instance to a JSON string.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="jsonSerializer">The JSON serializer.</param>
    /// <param name="instance">The instance to serialize.</param>
    /// <returns>A JSON string representation of the instance.</returns>
    public static string SerializeToString<T>(this IJsonSerializer jsonSerializer, T instance)
    {
        using var stream = new MemoryStream();
        jsonSerializer.Serialize(stream, instance!);
        return Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
    }
}
