using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Brain.Utilities;
using OpenAI.Chat;

namespace Brain;

public class OpenAiChatProvider
{
    public enum Actor
    {
        User,
        Brain,
        Tool
    }
    
    private ChatClient _client;
    
    private ChatCompletionOptions _options;

    private readonly List<ChatMessage> _messages = new List<ChatMessage>();

    private AsyncCollectionResult<StreamingChatCompletionUpdate> _completion;

    public delegate Task ChatCompleted(Actor actor);

    public event ChatCompleted OnChatCompleted;

    public OpenAiTool[] Tools =
    {
        new WeatherTool(),
        new LocationTool(),
        new DateTool(),
    };
    
    public OpenAiChatProvider()
    {
        _client = new ChatClient(model: "gpt-4o", Store.Instance.ApiKey);

        _options = new ChatCompletionOptions();
        
        foreach (var tool in Tools)
        {
            _options.Tools.Add(tool.ChatTool);
        }
        
        OnChatCompleted += actor =>
        {
            switch (actor)
            {
                case Actor.User: case Actor.Tool:
                    return CompleteBrainChat();
                case Actor.Brain:
                    return CompleteUserChat();
                default:
                    throw new ArgumentOutOfRangeException(nameof(actor), actor, null);
            }
        };
    }

    public Task StartChat()
    {
        return CompleteUserChat();
    }
    
    public Task CompleteUserChat()
    {
        Console.WriteLine();
        Console.Write("[YOU]: ");
        
        _messages.Add(Console.ReadLine());
        
        return OnChatCompleted?.Invoke(Actor.User);
    }
    
    public async Task CompleteBrainChat()
    {
        _completion = _client.CompleteChatStreamingAsync(_messages, _options);
        
        List<ChatMessageContentPart> parts = new List<ChatMessageContentPart>();

        Dictionary<string, List<StreamingChatToolCallUpdate>> toolCompletion = new Dictionary<string, List<StreamingChatToolCallUpdate>>();
        
        await foreach (var completionUpdate in _completion)
        {
            if (completionUpdate.ContentUpdate.Count > 0)
            {
                string part = completionUpdate.ContentUpdate[0].Text;
             
                parts.Add(ChatMessageContentPart.CreateTextPart(part));
            }

            if (completionUpdate.ToolCallUpdates.Count > 0)
            {
                foreach (var toolCallUpdate in completionUpdate.ToolCallUpdates)
                {
                    if (string.IsNullOrEmpty(toolCallUpdate.ToolCallId))
                    {
                        if (toolCompletion.Count < 0)
                        {
                            throw new Exception("Tool call update without a tool call ID.");
                        }
                        
                        toolCompletion.Last().Value.Add(toolCallUpdate);
                    }
                    else
                    {
                        if (!toolCompletion.ContainsKey(toolCallUpdate.ToolCallId))
                        {
                            toolCompletion.Add(toolCallUpdate.ToolCallId, new List<StreamingChatToolCallUpdate>{ toolCallUpdate });
                        }
                        else
                        {
                            toolCompletion[toolCallUpdate.ToolCallId].Add(toolCallUpdate);
                        }
                    }
                }
            }
        }

        if (parts.Count > 0)
        {
            Console.Write("[BRAIN]: ");
            
            foreach (var part in parts)
            {
                Console.Write(part.Text);
            }
            
            _messages.Add(new AssistantChatMessage(parts));
        }

        if (toolCompletion.Count > 0)
        {
            UseTools(toolCompletion);

            var completedTask = OnChatCompleted?.Invoke(Actor.Tool);

            if (completedTask != null) await completedTask;

            return;
        }
        
        var task = OnChatCompleted?.Invoke(Actor.Brain);
        
        if (task != null) await task;
    }

    private void UseTools(Dictionary<string, List<StreamingChatToolCallUpdate>> toolCompletion)
    {
        List<ChatToolCall> toolCalls = new List<ChatToolCall>();
        
        foreach (var toolCallUpdates in toolCompletion)
        {
            BinaryData[] arguments = Array.ConvertAll(toolCallUpdates.Value.ToArray(), u => u.FunctionArgumentsUpdate);

            var toolCall = ChatToolCall.CreateFunctionToolCall(toolCallUpdates.Key,
                toolCallUpdates.Value[0].FunctionName, CombineBinaryData(arguments));
            
            toolCalls.Add(toolCall);
        }
        
        _messages.Add(new AssistantChatMessage(toolCalls));
        
        foreach (var toolCall in toolCalls)
        {
            if (Tools.Any(t => t.Name == toolCall.FunctionName))
            {
                var tool = Tools.First(t => t.Name == toolCall.FunctionName);
                
                //TODO add Y/N prompt for tool usage
                
                string message = tool.Execute(toolCall.FunctionArguments);
                
                _messages.Add(new ToolChatMessage(toolCall.Id, message));
            }
        }
    }
    
    public BinaryData CombineBinaryData(params BinaryData[] dataParts)
    {
        int totalLength = 0;
        foreach (var data in dataParts)
        {
            totalLength += data.ToArray().Length;
        }

        byte[] combinedBytes = new byte[totalLength];
        int offset = 0;
        foreach (var data in dataParts)
        {
            byte[] bytes = data.ToArray();
            Buffer.BlockCopy(bytes, 0, combinedBytes, offset, bytes.Length);
            offset += bytes.Length;
        }

        return new BinaryData(combinedBytes);
    }
    
    private void AssertCompletion(ChatFinishReason finishReason)
    {
        switch (finishReason)
        {
            case ChatFinishReason.Stop:
                return;
            case ChatFinishReason.ToolCalls:
                break;
            case ChatFinishReason.Length:
                throw new NotImplementedException("Incomplete model output due to MaxTokens parameter or token limit exceeded.");
            case ChatFinishReason.ContentFilter:
                throw new NotImplementedException("Omitted content due to a content filter flag.");
            case ChatFinishReason.FunctionCall:
                throw new NotImplementedException("Deprecated in favor of tool calls.");
            default:
                throw new ArgumentOutOfRangeException(nameof(finishReason), finishReason, null);
        }
    }
}