using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenAI.Chat;

namespace Brain.Tools;

public abstract class OpenAiTool : ITool
{
    public abstract string Name { get; }
    
    public abstract string Description { get; }
    
    public abstract ToolParameters? Parameters { get; }

    public ChatTool ChatTool => ChatTool.CreateFunctionTool(
        functionName: Name,
        functionDescription: Description,
        functionParameters: Parameters != null ? BinaryData.FromString(JsonConvert.SerializeObject(Parameters.Value)) : null
    );

    public Task<string> Execute(BinaryData raw)
    {
        JToken arguments = ResolveArguments(raw);
        
        return Execute(arguments);
    }
    
    protected abstract Task<string> Execute(JToken arguments);
    
    private JToken ResolveArguments(BinaryData raw)
    {
        JToken arguments = null;
        
        if (Parameters != null)
        {
            // Parse the arguments
            arguments = JToken.Parse(raw.ToString());

            var properties = Parameters.Value.Properties;
            
            string[] required = Parameters.Value.Required;
            
            for (int i = 0; i < properties.Count; i++)
            {
                var property = properties.ElementAt(i);
                
                string name = property.Key;
                
                var value = arguments[name];
                
                if (required.Contains(name) && value == null)
                {
                    throw new ArgumentNullException(name, $"Failed to execute {Name} tool, the {name} argument is required");
                }
            }
        }

        return arguments;
    }
}