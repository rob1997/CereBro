using System;
using OpenAI.Chat;

namespace Brain;

public class DateTool : OpenAiTool
{
    public override string Name => "GetCurrentDate";
    public override ChatTool ChatTool => ChatTool.CreateFunctionTool(
        functionName: Name,
        functionDescription: "Get the current date"
    );
    public override string Execute(BinaryData arguments)
    {
        return "January 1, 2022";
    }
}