using System;
using System.Threading.Tasks;
using CereBro.Tools;

namespace CereBro;

public class ConsoleChatDispatcher : IChatDispatcher
{
    public Task<string> PromptUser()
    {
        Console.Write("\n[YOU]: ");
        
        return Task.FromResult(Console.ReadLine());
    }

    public Task ShowResponse(string message, int index)
    {
        Console.Write(index == 0 ? $"[BRAIN]: {message}" : message);

        return Task.CompletedTask;
    }

    public Task<bool> PromptToolCall(ITool tool)
    {
        Console.WriteLine($"[TOOL]: Use `{tool.DisplayName}` tool? (Y/N)");
        
        while (true)
        {
            switch (Console.ReadLine()?.ToUpper())
            {
                case "Y":
                    return Task.FromResult(true);
                case "N":
                    return Task.FromResult(false);
                default:
                    Console.WriteLine("Invalid input, please try again.");
                    continue;
            }
        }
    }
}