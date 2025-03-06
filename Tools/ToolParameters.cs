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