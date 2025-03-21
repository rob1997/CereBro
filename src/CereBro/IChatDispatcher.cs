using System.Threading.Tasks;
using CereBro.Tools;

namespace CereBro;

public interface IChatDispatcher
{
    Task<string> PromptUser();
    
    Task ShowResponse(string message, int index);
    
    Task<bool> PromptToolCall(ITool tool);
}