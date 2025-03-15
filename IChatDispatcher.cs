using System.Threading.Tasks;
using Brain.Tools;

namespace Brain;

public interface IChatDispatcher
{
    Task<string> PromptUser();
    
    Task ShowResponse(string message, int index);
    
    Task<bool> PromptToolCall(ITool tool);
}