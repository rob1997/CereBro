using System;
using OpenAI.Chat;

namespace Brain.Tools;

public class DateTool : OpenAiTool
{
    public override string Name => "GetCurrentDate";
    
    public override string Description => "Get the current date";
    
    public override ChatTool ChatTool => ChatTool.CreateFunctionTool(
        functionName: Name,
        functionDescription: Description
    );
    
    public override string Execute(BinaryData arguments)
    {
        return "January 1, 2022";
    }
}