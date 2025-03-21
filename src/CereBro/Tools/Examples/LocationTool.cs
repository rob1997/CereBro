using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CereBro.Tools.Examples;

public class LocationTool : ITool
{
    public string Name => "GetCurrentLocation";
    
    public string DisplayName => "Get Current Location";

    public string Description => "Get the user's current location";
    
    public ToolParameters? Parameters => null;

    public Task<string> Execute(JToken arguments)
    {
        return Task.FromResult("Austin, TX");
    }
}