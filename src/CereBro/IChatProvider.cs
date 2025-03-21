using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CereBro.Utilities;
using CereBro.Tools;

namespace CereBro;

public interface IChatProvider
{
    public Func<Message, Task> Completed { get; set; }
    
    public ITool[] Tools { get; }
    
    public Queue<ToolCall> CallQueue { get; }

    public IChatDispatcher ChatDispatcher { get; }

    public Task StartConversation()
    {
        Completed += OnCompleted;

        return PromptUser();
    }

    private Task OnCompleted(Message message)
    {
        Role role = message.Role;
                    
        switch (role)
        {
            case Role.User:
                return FetchResponse();
            case Role.Tool:
                return CallQueue.Count != 0 ?
                    // Next Tool or Fetch Response from Brain based on Tool Response
                    UseTool() : FetchResponse();

            case Role.Brain:
                return CallQueue.Count != 0 ? 
                    // Check for Queued Tool Calls before Prompting User
                    UseTool() : PromptUser();
            default:
                throw new ArgumentOutOfRangeException(nameof(role), role, null);
        }
    }
    
    public async Task PromptUser()
    {
        string message = await ChatDispatcher.PromptUser();
        
        await InvokeCompleted(new Message(Role.User, message));
    }

    Task FetchResponse(Func<string, Task> callback);
    
    public async Task FetchResponse()
    {
        string message = string.Empty;
        
        int index = 0;
        
        await FetchResponse(response =>
        {
            message += response;
            
            return ChatDispatcher.ShowResponse(response, index++);
        });
        
        await InvokeCompleted(new Message(Role.Brain, message));
    }
    
    public async Task UseTool()
    {
        ToolCall call = CallQueue.Dequeue();

        await InvokeCompleted(new Message(Role.Tool, await UseTool(call)));
    }

    private async Task<string> UseTool(ToolCall call)
    {
        if (GetTool(call.Name, out ITool tool))
        {
            if (await ChatDispatcher.PromptToolCall(tool))
            {
                if (tool.Parameters != null)
                {
                    if (!call.Arguments.Validate(tool.Parameters.Value, out string message))
                    {
                        return $"Failed to execute {tool.Name}: {message}";
                    }
                }
                
                return await tool.Execute(call.Arguments);
            }

            return $"{tool.Name} tool usage rejected by user.";
        }

        return $"{call.Name} tool not registered.";
    }

    private bool GetTool(string name, out ITool tool)
    {
        tool = null;
        
        try
        {
            tool = Tools.Single(t => t.Name == name);
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }
    
    private Task InvokeCompleted(Message message)
    {
        return Completed?.Invoke(message);
    }
}