using System;
using Brain.Tools;
using OpenAI.Chat;

namespace Brain.Utilities;

public static class Extensions
{
    public static BinaryData Combine(this BinaryData[] array)
    {
        int length = 0;
        
        foreach (var data in array)
        {
            length += data.ToArray().Length;
        }

        byte[] combined = new byte[length];
        
        int offset = 0;
        
        foreach (var data in array)
        {
            byte[] bytes = data.ToArray();
            
            Buffer.BlockCopy(bytes, 0, combined, offset, bytes.Length);
            
            offset += bytes.Length;
        }

        return new BinaryData(combined);
    }

    public static ChatTool ChatTool(this ITool tool)
    {
        if (tool is OpenAiTool openAiTool)
        {
            return openAiTool.ChatTool;
        }
        
        throw new Exception($"The {tool.GetType().Name} must be an instance of {typeof(OpenAiTool)}");
    }
}