using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;

namespace CereBro.OpenAI
{
    public static class Extensions
    {
        public static IServiceCollection UseOpenAI(this IServiceCollection services, string apiKey, string modelId)
        {
            return services.UseChatClient<OpenAIFunctionInvokingChatClient>(new OpenAIClient(apiKey)
                    .AsChatClient(modelId));
        }
    }
}