using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CereBro.Tools;
using CereBro.OpenAI.Utilities;
using OpenAI.Chat;

namespace CereBro.OpenAI;

public class OpenAIChatProvider : IChatProvider
{
    private readonly ChatClient _client;
    
    private readonly ChatCompletionOptions _options;

    private readonly List<ChatMessage> _messages = new List<ChatMessage>();

    private AsyncCollectionResult<StreamingChatCompletionUpdate> _completion;

    public Func<Message, Task> Completed { get; set; }

    public ITool[] Tools { get; }

    public Queue<ToolCall> CallQueue { get; private set; } = new Queue<ToolCall>();
    
    private Queue<ChatToolCall> _chatToolCalls = new Queue<ChatToolCall>();

    public IChatDispatcher ChatDispatcher { get; private set; }

    public OpenAIChatProvider(IChatDispatcher chatDispatcher, IEnumerable<ITool> tools)
    {
        Tools = tools.ToArray();
        
        ChatDispatcher = chatDispatcher;
        
        _client = new ChatClient(model: "gpt-4o", Environment.GetEnvironmentVariable("OPEN_AI_API_KEY"));

        _options = new ChatCompletionOptions();
        
        foreach (var tool in Tools)
        {
            _options.Tools.Add(tool.ChatTool());
        }
        
        Completed += message =>
        {
            Role role = message.Role;
            
            switch (role)
            {
                case Role.User:
                    _messages.Add(message.Value);
                    break;
                case Role.Tool:
                    _messages.Add(new ToolChatMessage(_chatToolCalls.Dequeue().Id, message.Value));
                    break;
                case Role.Brain:
                    if (!string.IsNullOrEmpty(message.Value))
                    {
                        _messages.Add(new AssistantChatMessage(message.Value));
                    }
                    if (_chatToolCalls.Count != 0)
                    {
                        _messages.Add(new AssistantChatMessage(_chatToolCalls.ToList()));
                    }
                    break;
            }

            return Task.CompletedTask;
        };
    }

    public async Task FetchResponse(Func<string, Task> callback)
    {
        _completion = _client.CompleteChatStreamingAsync(_messages, _options);
        
        List<ChatToolCall> chatToolCalls = new List<ChatToolCall>();

        await foreach (var completionUpdate in _completion)
        {
            if (completionUpdate.ContentUpdate.Count > 0)
            {
                string part = completionUpdate.ContentUpdate[0].Text;
             
                await callback(part);
            }

            if (completionUpdate.ToolCallUpdates.Count > 0)
            {
                completionUpdate.ToolCallUpdates.ChatToolCalls(ref chatToolCalls);
            }
        }

        _chatToolCalls = new Queue<ChatToolCall>(chatToolCalls);
        
        CallQueue = new Queue<ToolCall>(chatToolCalls.Select(c => c.ToolCall()));
    }
}