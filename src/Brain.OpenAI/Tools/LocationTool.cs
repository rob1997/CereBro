using System.Threading.Tasks;
using Brain.Core.Tools;
using Newtonsoft.Json.Linq;

namespace Brain.OpenAI.Tools;

public class LocationTool : OpenAiTool
{
    public override string Name => "GetCurrentLocation";
    
    public override string DisplayName => "Get Current Location";

    public override string Description => "Get the user's current location";
    
    public override ToolParameters? Parameters => null;

    public override Task<string> Execute(JToken arguments)
    {
        return Task.FromResult("Austin, TX");
    }
}