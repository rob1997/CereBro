using System;
using System.Threading.Tasks;

namespace CereBro;

public class ConsoleChatDispatcher : IChatDispatcher
{
    public Task<string> PromptUser()
    {
        Console.Write("\n[YOU]: ");
        
        return Task.FromResult(Console.ReadLine());
    }

    public Task ShowStreamingResponse(string message, int index)
    {
        Console.Write(index == 0 ? $"[BRAIN]: {message}" : message);

        return Task.CompletedTask;
    }

    public Task ShowResponse(string message)
    {
        Console.Write($"[BRAIN]: {message}");

        return Task.CompletedTask;
    }

    public Task<bool> PromptToolCall(string name)
    {
        Console.WriteLine($"[TOOL]: Use `{name}` tool? (Y/N)");
        
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