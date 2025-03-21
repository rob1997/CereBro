using System;
using System.Collections.Generic;
using System.Linq;
using Brain.Core.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenAI.Chat;

namespace Brain.OpenAI.Utilities;

public static class Extensions
{
    private static BinaryData AggregateBinaryData(params BinaryData[] arguments)
    {
        int length = 0;
        
        foreach (var data in arguments)
        {
            length += data.ToArray().Length;
        }

        byte[] combined = new byte[length];
        
        int offset = 0;
        
        foreach (var data in arguments)
        {
            byte[] bytes = data.ToArray();
            
            Buffer.BlockCopy(bytes, 0, combined, offset, bytes.Length);
            
            offset += bytes.Length;
        }

        return new BinaryData(combined);
    }

    public static ChatTool ChatTool(this ITool tool)
    {
        return global::OpenAI.Chat.ChatTool.CreateFunctionTool(
            functionName: tool.Name,
            functionDescription: tool.Description,
            functionParameters: tool.Parameters != null ? BinaryData.FromString(JsonConvert.SerializeObject(tool.Parameters.Value)) : null
        );
    }

    private static ChatToolCall Aggregate(this ChatToolCall chatToolCall, StreamingChatToolCallUpdate update)
    {
        return ChatToolCall.CreateFunctionToolCall(chatToolCall.Id,
            chatToolCall.FunctionName, AggregateBinaryData(chatToolCall.FunctionArguments, update.FunctionArgumentsUpdate));
    }
    
    public static void ChatToolCalls(this IEnumerable<StreamingChatToolCallUpdate> updates, ref List<ChatToolCall> chatToolCalls)
    {
        foreach (var update in updates)
        {
            int index = - 1;

            Predicate<ChatToolCall> match = chatToolCall => update.ToolCallId == chatToolCall.Id;
            
            if (chatToolCalls.Any(match.Invoke))
            {
                index = chatToolCalls.FindIndex(match);
            }
            else if (string.IsNullOrEmpty(update.ToolCallId))
            {
                index = chatToolCalls.Count - 1;
            }

            if (index > - 1)
            {
                ChatToolCall toolCall = chatToolCalls[index];

                chatToolCalls[index] = toolCall.Aggregate(update);
            }
            
            else
            {
                chatToolCalls.Add(ChatToolCall.CreateFunctionToolCall(update.ToolCallId,
                        update.FunctionName, update.FunctionArgumentsUpdate));
            }
        }
    }
    
    public static ToolCall ToolCall(this ChatToolCall chatToolCall)
    {
        JToken arguments = JToken.Parse(chatToolCall.FunctionArguments.ToString());
        
        return new ToolCall(chatToolCall.FunctionName, arguments);
    }
}