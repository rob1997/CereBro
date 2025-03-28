using System;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenAI;

namespace CereBro.OpenAI
{
    public static class Extensions
    {
        internal static ILogger GetLogger(this IServiceProvider services)
        {
            ILoggerFactory factory = services.GetService<ILoggerFactory>();
            
            return factory?.CreateLogger(typeof (FunctionInvokingChatClient));
        }
        
        internal static ChatClientBuilder UseOpenAIFunctionInvokingChatClient(this ChatClientBuilder builder)
        {
            return builder.Use((innerClient, services) => new OpenAIFunctionInvokingChatClient(innerClient, services.GetLogger()));
        }
        
        public static IServiceCollection UseOpenAI(this IServiceCollection services, string apiKey, string modelId)
        {
            return services
                .AddSingleton<IChatClient, OpenAIFunctionInvokingChatClient>(_ => 
                    (OpenAIFunctionInvokingChatClient) new OpenAIClient(apiKey)
                    .AsChatClient(modelId)
                    .AsBuilder()
                    .UseOpenAIFunctionInvokingChatClient()
                    .Build());
        }
    }
}