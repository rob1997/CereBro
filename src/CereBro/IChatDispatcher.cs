using System.Threading.Tasks;

namespace CereBro;

public interface IChatDispatcher
{
    Task<string> PromptUser();
    
    Task ShowStreamingResponse(string message, int index);
    
    Task ShowResponse(string message);
    
    Task<bool> PromptToolCall(string name);
}