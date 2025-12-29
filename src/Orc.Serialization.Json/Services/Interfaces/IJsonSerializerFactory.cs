namespace Orc.Serialization.Json
{
    public interface IJsonSerializerFactory
    {
        IJsonSerializer CreateSerializer();

        IJsonSerializer CreateSerializer(JsonSerializerSettings settings);
    }
}
