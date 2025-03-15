using System.Threading.Tasks;
using Brain.Core.Tools;

namespace Brain.Core;

public interface IChatDispatcher
{
    Task<string> PromptUser();
    
    Task ShowResponse(string message, int index);
    
    Task<bool> PromptToolCall(ITool tool);
}