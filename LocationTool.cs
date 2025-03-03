using System;
using OpenAI.Chat;

namespace Brain;

public class LocationTool : OpenAiTool
{
    public override string Name => "GetCurrentLocation";
    public override ChatTool ChatTool => ChatTool.CreateFunctionTool(
        functionName: Name,
        functionDescription: "Get the user's current location"
    );
    public override string Execute(BinaryData arguments)
    {
        return "Austin, TX";
    }
}