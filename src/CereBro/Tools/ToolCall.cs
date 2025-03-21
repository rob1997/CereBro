using Newtonsoft.Json.Linq;

namespace CereBro.Tools;

public struct ToolCall
{
    public string Name { get; private set; }
    
    public JToken Arguments { get; private set; }

    public ToolCall(string name, JToken arguments = null)
    {
        Name = name;
        
        Arguments = arguments;
    }
}