using System.Threading.Tasks;
using CereBro.Core.Tools;

namespace CereBro.Core;

public interface IChatDispatcher
{
    Task<string> PromptUser();
    
    Task ShowResponse(string message, int index);
    
    Task<bool> PromptToolCall(ITool tool);
}