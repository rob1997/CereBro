using System;
using Newtonsoft.Json.Linq;

namespace Brain.Tools;

public struct ToolCall
{
    public string Name { get; private set; }
    
    public JToken Arguments { get; private set; }

    public Action<string> Response { get; private set; }

    public ToolCall(string name, Action<string> response, JToken arguments = null)
    {
        Name = name;
        
        Response = response;
        
        Arguments = arguments;
    }
}