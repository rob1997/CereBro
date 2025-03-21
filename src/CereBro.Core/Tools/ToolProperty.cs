using Newtonsoft.Json;

namespace CereBro.Core.Tools;

public struct ToolProperty
{
    [JsonProperty("type")]
    public string Type { get; private set; }

    [JsonProperty("description")]
    public string Description { get; private set; }

    [JsonProperty("enum", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string[] Options { get; private set; }
    
    public ToolProperty(string type, string description, string[] options = null)
    {
        Type = type;
        Description = description;
        Options = options;
    }
}