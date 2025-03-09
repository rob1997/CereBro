using System.Collections.Generic;
using Newtonsoft.Json;

namespace Brain.Tools;

public struct ToolParameters
{
    [JsonProperty("type")]
    public string Type { get; private set; }
    
    [JsonProperty("properties")]
    public Dictionary<string, ToolProperty> Properties { get; private set; }
    
    [JsonProperty("required", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string[] Required { get; private set; }

    public ToolParameters(Dictionary<string, ToolProperty> properties, string type = "object", string[] required = null)
    {
        Type = type;
        Properties = properties;
        Required = required;
    }
}