using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Brain.Tools;

namespace Brain;

public interface IChatProvider
{
    public Func<Role, Task> Completed { get; }
    
    
    public ITool[] Tools { get; }
    
    public Queue<ToolCall> CallQueue { get; }

    public async Task UseTools()
    {
        while (CallQueue.Count != 0)
        {
            ToolCall call = CallQueue.Dequeue();

            call.Response?.Invoke(await UseTool(call));
        }

        await InvokeCompleted(Role.Tool);
    }

    private async Task<string> UseTool(ToolCall call)
    {
        if (GetTool(call.Name, out ITool tool))
        {
            Console.WriteLine($"[TOOL]: Use {tool.Name} tool? (Y/N)");

            while (true)
            {
                switch (Console.ReadLine()?.ToUpper())
                {
                    case "Y":
                        return await tool.Execute(call.Arguments);
                    case "N":
                        return $"{tool.Name} tool usage rejected by user.";
                    default:
                        Console.WriteLine("Invalid input.");
                        continue;
                }
            }
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
    
    private Task InvokeCompleted(Role role)
    {
        return Completed?.Invoke(role);
    }
}