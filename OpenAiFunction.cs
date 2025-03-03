using Newtonsoft.Json;

namespace Brain;

public struct OpenAiFunction
{
    [JsonProperty("type")]
    public string Type { get; private set; }

    [JsonProperty("properties")]
    public object[] Properties { get; set; }
    
    [JsonProperty("required")]
    public string[] Required { get; private set; }

    public OpenAiFunction(string type, Property[] properties, string[] required)
    {
        
    }
    
    public struct Property
    {
        [JsonProperty("type")]
        public string Type { get; private set; }

        [JsonProperty("enum")]
        public string[] Enum { get; private set; }
        
        [JsonProperty("description")]
        public string Description { get; private set; }
    }
}