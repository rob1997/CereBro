using System;
using OpenAI.Chat;

namespace Brain.Tools;

public abstract class OpenAiTool : ITool
{
    public abstract string Name { get; }
    
    public abstract string Description { get; }

    public abstract ChatTool ChatTool { get; }

    public abstract string Execute(BinaryData arguments);
}