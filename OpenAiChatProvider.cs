using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using Brain.Tools;
using Brain.Utilities;
using OpenAI.Chat;

namespace Brain;

public class OpenAiChatProvider : IChatProvider
{
    private ChatClient _client;
    
    private ChatCompletionOptions _options;

    private readonly List<ChatMessage> _messages = new List<ChatMessage>();

    private AsyncCollectionResult<StreamingChatCompletionUpdate> _completion;

    public Func<Role, Task> Completed { get; private set; }

    public ITool[] Tools => new ITool[]
    {
        new WeatherTool(),
        new LocationTool(),
        new DateTool(),
    };

    public Queue<ToolCall> CallQueue { get; private set; } = new Queue<ToolCall>();

    public OpenAiChatProvider()
    {
        _client = new ChatClient(model: "gpt-4o", Store.Instance.ApiKey);

        _options = new ChatCompletionOptions();
        
        foreach (var tool in Tools)
        {
            _options.Tools.Add(tool.ChatTool());
        }
        
        Completed += role =>
        {
            switch (role)
            {
                case Role.User: case Role.Tool:
                    return CompleteBrainChat();
                case Role.Brain:
                    if (CallQueue.Count != 0)
                    {
                        return (this as IChatProvider).UseTools();
                    }
                    else
                    {
                        return CompleteUserChat();
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(role), role, null);
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
        
        return Completed?.Invoke(Role.User);
    }
    
    public async Task CompleteBrainChat()
    {
        _completion = _client.CompleteChatStreamingAsync(_messages, _options);
        
        List<ChatMessageContentPart> parts = new List<ChatMessageContentPart>();

        List<ChatToolCall> chatToolCalls = new List<ChatToolCall>();
        
        await foreach (var completionUpdate in _completion)
        {
            if (completionUpdate.ContentUpdate.Count > 0)
            {
                string part = completionUpdate.ContentUpdate[0].Text;
             
                parts.Add(ChatMessageContentPart.CreateTextPart(part));

                if (parts.Count == 1)
                {
                    Console.Write("[BRAIN]: ");
                }
                
                Console.Write(part);
            }

            if (completionUpdate.ToolCallUpdates.Count > 0)
            {
                completionUpdate.ToolCallUpdates.ChatToolCalls(ref chatToolCalls);
            }
        }

        if (parts.Count > 0)
        {
            _messages.Add(new AssistantChatMessage(parts));
        }

        if (chatToolCalls.Count > 0)
        {
            foreach (var chatToolCall in chatToolCalls)
            {
                CallQueue.Enqueue(chatToolCall.ToolCall(result =>
                {
                    _messages.Add(new ToolChatMessage(chatToolCall.Id, result));
                }));
            }
            
            _messages.Add(new AssistantChatMessage(chatToolCalls));
        }
        
        var task = Completed?.Invoke(Role.Brain);
        
        if (task != null) await task;
    }
}