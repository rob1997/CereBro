using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Brain.Tools;

public interface ITool
{
    public string Name { get; }
    
    public string DisplayName { get; }
    
    public string Description { get; }

    public ToolParameters? Parameters { get; }
    
    Task<string> Execute(JToken arguments);
}