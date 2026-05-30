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
    /// Deserializes an object of type <typeparamref name="T"/> from a JSON string.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize.</typeparam>
    /// <param name="jsonSerializer">The JSON serializer.</param>
    /// <param name="value">The JSON string to deserialize.</param>
    /// <returns>The deserialized object, or <c>null</c> if deserialization returns no result.</returns>
    public static T? DeserializeFromString<T>(this IJsonSerializer jsonSerializer, string value)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(value));
        return jsonSerializer.Deserialize<T>(stream);
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

    /// <summary>
    /// Populates the properties of an existing object from the specified stream containing JSON data.
    /// Only properties present in the JSON are updated; all other properties remain unchanged.
    /// </summary>
    /// <typeparam name="T">The type of the object to populate.</typeparam>
    /// <param name="jsonSerializer">The JSON serializer.</param>
    /// <param name="stream">The stream containing the JSON data.</param>
    /// <param name="target">The existing object whose properties will be updated.</param>
    public static void PopulateObject<T>(this IJsonSerializer jsonSerializer, Stream stream, T target)
        where T : class
    {
        jsonSerializer.PopulateObject(stream, (object)target);
    }

    /// <summary>
    /// Populates the properties of an existing object from a JSON string.
    /// Only properties present in the JSON are updated; all other properties remain unchanged.
    /// </summary>
    /// <typeparam name="T">The type of the object to populate.</typeparam>
    /// <param name="jsonSerializer">The JSON serializer.</param>
    /// <param name="value">The JSON string containing the properties to update.</param>
    /// <param name="target">The existing object whose properties will be updated.</param>
    public static void PopulateObjectFromString<T>(this IJsonSerializer jsonSerializer, string value, T target)
        where T : class
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(value));
        jsonSerializer.PopulateObject(stream, (object)target);
    }
}
