using Newtonsoft.Json.Linq;

namespace Brain.Tools;

public class LocationTool : OpenAiTool
{
    public override string Name => "GetCurrentLocation";
    
    public override string Description => "Get the user's current location";
    
    public override ToolParameters? Parameters => null;

    protected override string Execute(JToken arguments)
    {
        return "Austin, TX";
    }
}