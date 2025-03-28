using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace CereBro.OpenAI;

// Just a Placeholder Type to allow multiple IChatClient implementations to be registered with the DI container
public class OpenAIFunctionInvokingChatClient : FunctionInvokingChatClient
{
    public OpenAIFunctionInvokingChatClient(IChatClient innerClient, ILogger logger = null) : base(innerClient, logger)
    {
    }
}