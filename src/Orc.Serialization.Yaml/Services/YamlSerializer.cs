namespace Orc.Serialization.Yaml;

using System;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class YamlSerializer : IYamlSerializer
{
    private readonly ISerializer _innerSerializer;
    private readonly IDeserializer _innerDeserializer;

    public YamlSerializer(YamlSerializerSettings settings)
    {
        var serializerBuilder = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance);

        if (!settings.IncludeFields)
        {
            serializerBuilder = serializerBuilder
                .IgnoreFields();
        }

        _innerSerializer = serializerBuilder.Build();

        var deserializerBuilder = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance);

        if (!settings.IncludeFields)
        {
            deserializerBuilder = deserializerBuilder
                .IgnoreFields();
        }

        _innerDeserializer = deserializerBuilder.Build();
    }

    public object? Deserialize(Stream stream, Type targetType)
    {
        // Don't dispose, we don't own the stream
#pragma warning disable IDISP001 // Dispose created
        var textReader = new StreamReader(stream);
#pragma warning restore IDISP001 // Dispose created

        return _innerDeserializer.Deserialize(textReader, targetType);
    }

    public void Serialize(Stream stream, object obj)
    {
        // Don't dispose, we don't own the stream
#pragma warning disable IDISP001 // Dispose created
        var textWriter = new StreamWriter(stream);
#pragma warning restore IDISP001 // Dispose created

        _innerSerializer.Serialize(textWriter, obj);
    }
}
