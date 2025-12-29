namespace Orc.Serialization.Json
{
    using System.IO;

    public static class IJsonSerializerExtensions
    {
        public static T? Deserialize<T>(this IJsonSerializer jsonSerializer, Stream stream)
        {
            return (T?)jsonSerializer.Deserialize(stream, typeof(T));
        }
    }
}
