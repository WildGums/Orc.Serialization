namespace Orc.Serialization.Yaml;

using System.Globalization;

public class YamlSerializerSettings
{
    public YamlSerializerSettings()
    {
    }

    public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

    public bool IncludeFields { get; set; } = false;

    //public bool PropertyNameCaseInsensitive { get; set; } = false;
}
