namespace Orc.Serialization.Json;

public class JsonSerializerSettings
{
    public JsonSerializerSettings()
    {
    }

    public int MaxDepth { get; set; } = 0;

    public bool IncludeFields { get; set; } = false;

    public bool PropertyNameCaseInsensitive { get; set; } = false;

    public bool WriteIndented { get; set; } = true;

    public bool SerializeEnumsAsStrings { get; set; } = true;
}
