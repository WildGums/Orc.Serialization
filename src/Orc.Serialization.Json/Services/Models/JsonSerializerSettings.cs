namespace Orc.Serialization.Json;

using System.Collections.Generic;
using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

public class JsonSerializerSettings
{
    public JsonSerializerSettings()
    {
    }

    public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

    public int MaxDepth { get; set; } = 0;

    public bool IncludeFields { get; set; } = false;

    public bool PropertyNameCaseInsensitive { get; set; } = false;

    public bool WriteIndented { get; set; } = true;

    public bool SerializeEnumsAsStrings { get; set; } = true;

    /// <summary>
    /// Gets the type info resolver chain used during (de)serialization.
    /// </summary>
    public IList<IJsonTypeInfoResolver> TypeInfoResolverChain { get; } = [];

    /// <summary>
    /// Gets or sets the serializer binder that validates whether a type is allowed to be serialized or deserialized.
    /// </summary>
    public ISerializerBinder? SerializerBinder { get; set; }

    public JsonObjectCreationHandling PreferredObjectCreationHandling { get; set; } = JsonObjectCreationHandling.Replace;

    /// <summary>
    /// Gets or sets a value indicating whether runtime type info metadata should be written and read during (de)serialization.
    /// </summary>
    public bool UseTypeInfoConverter { get; set; }
}
