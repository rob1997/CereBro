using System;
using OpenAI.Chat;

namespace Brain.Tools;

public class LocationTool : OpenAiTool
{
    public override string Name => "GetCurrentLocation";
    
    public override string Description => "Get the user's current location";

    public override ChatTool ChatTool => ChatTool.CreateFunctionTool(
        functionName: Name,
        functionDescription: Description
    );
    public override string Execute(BinaryData arguments)
    {
        return "Austin, TX";
    }
}