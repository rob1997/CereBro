using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CereBro.Core.Tools.Examples;

public class DateTool : ITool
{
    public string Name => "GetCurrentDate";
    
    public string DisplayName => "Get Current Date";

    public string Description => "Get the current date";
    
    public ToolParameters? Parameters => null;

    public Task<string> Execute(JToken arguments)
    {
        return Task.FromResult("January 1, 2025");
    }
}