using System;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CereBro
{
    public static class CereBro
    {
        public static IHost BuildCereBro<TChatDispatcher>(this HostApplicationBuilder builder, ICereBroConfig config)
            where TChatDispatcher : class, IChatDispatcher
        {
            builder.Services.AddHostedService<ChatProvider>()
                .AddSingleton<IChatDispatcher, TChatDispatcher>()
                .AddSingleton(config);

            return builder.Build();
        }

        public static IHost BuildCereBro(this HostApplicationBuilder builder, ICereBroConfig config)
        {
            return BuildCereBro<ConsoleChatDispatcher>(builder, config);
        }

        public static IServiceCollection UseChatClient<T>(this IServiceCollection services, IChatClient chatClient)
            where T : FunctionInvokingChatClient
        {
            return services.AddSingleton<IChatClient, T>(_ =>
                (T)chatClient.AsBuilder().UseFunctionInvocation<T>().Build());
        }

        public static ChatClientBuilder UseFunctionInvocation<T>(this ChatClientBuilder builder)
            where T : FunctionInvokingChatClient
        {
            return builder.Use((innerClient, services) =>
                (T)Activator.CreateInstance(typeof(T), innerClient, services.GetLogger()));
        }

        internal static ILogger GetLogger(this IServiceProvider services)
        {
            ILoggerFactory factory = services.GetService<ILoggerFactory>();

            return factory?.CreateLogger(typeof(FunctionInvokingChatClient));
        }
    }
}