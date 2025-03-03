using System;
using System.Collections.Generic;
using OpenAI.Chat;

namespace Brain;

public abstract class OpenAiTool : ITool
{
    public abstract string Name { get; }
    
    public abstract ChatTool ChatTool { get; }

    public abstract string Execute(BinaryData arguments);
}