using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CereBro
{
    public static class CereBro
    {
        public static IHost BuildCereBro<TChatDispatcher>(this HostApplicationBuilder builder, ICereBroConfig config)
            where TChatDispatcher : class, IChatDispatcher
        {
            builder.Services
                .AddHostedService<ChatProvider>()
                .AddSingleton<IChatDispatcher, TChatDispatcher>()
                .AddSingleton(config);

            return builder.Build();
        }
        
        public static IHost BuildCereBro(this HostApplicationBuilder builder, ICereBroConfig config)
        {
            return BuildCereBro<ConsoleChatDispatcher>(builder, config);
        }
    }
}