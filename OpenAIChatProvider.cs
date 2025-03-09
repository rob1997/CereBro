using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Brain.Tools;
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

    private Queue<ChatToolCall> _toolCallQueue = new Queue<ChatToolCall>();
    
    public delegate Task ChatCompleted(Actor actor);

    public event ChatCompleted OnChatCompleted;

    public OpenAiTool[] Tools =>new OpenAiTool[]
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
                    if (_toolCallQueue.Count != 0)
                    {
                        return UseTools();
                    }
                    else
                    {
                        return CompleteUserChat();
                    }
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
            foreach (var toolCallUpdates in toolCompletion)
            {
                BinaryData[] arguments = Array.ConvertAll(toolCallUpdates.Value.ToArray(), u => u.FunctionArgumentsUpdate);

                var toolCall = ChatToolCall.CreateFunctionToolCall(toolCallUpdates.Key,
                    toolCallUpdates.Value[0].FunctionName, arguments.Combine());
            
                _toolCallQueue.Enqueue(toolCall);
            }
        }
        
        var task = OnChatCompleted?.Invoke(Actor.Brain);
        
        if (task != null) await task;
    }

    private async Task UseTools()
    {
        _messages.Add(new AssistantChatMessage(_toolCallQueue.ToList()));

        while (_toolCallQueue.Count != 0)
        {
            var toolCall = _toolCallQueue.Peek();
            
            if (Tools.Any(t => t.Name == toolCall.FunctionName))
            {
                var tool = Tools.First(t => t.Name == toolCall.FunctionName);
                
                Console.WriteLine($"[TOOL]: Use {tool.Name} tool? (Y/N)");

                switch (Console.ReadLine()?.ToUpper())
                {
                    case "Y":
                        string message = await tool.Execute(toolCall.FunctionArguments);
                
                        _messages.Add(new ToolChatMessage(toolCall.Id, message));
                        break;
                    case "N":
                        _messages.Add(new ToolChatMessage(toolCall.Id, "tool call failed"));
                        break;
                    default:
                        Console.WriteLine("Invalid input.");
                        continue;
                }
                
                _toolCallQueue.Dequeue();
            }

            else
            {
                throw new Exception("Tool not found.");
            }
        }

        var task = OnChatCompleted?.Invoke(Actor.Tool);

        if (task != null) await task;
    }
}