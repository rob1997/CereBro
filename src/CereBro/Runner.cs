using System.Threading.Tasks;
using CereBro.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace CereBro
{
    public static class Runner
    {
        public static ServiceProvider ServiceProvider { get; private set; }
        
        public static Task StartConversation<TChatProvider, TChatDispatcher>(ITool[] tools)
            where TChatProvider : class, IChatProvider
            where TChatDispatcher : class, IChatDispatcher
        {
            ServiceCollection services = new ServiceCollection();

            ServiceProvider = services
                .AddSingleton<IChatProvider, TChatProvider>()
                .AddSingleton<IChatDispatcher, TChatDispatcher>()
                .AddTools(tools)
                .BuildServiceProvider();
            
            IChatProvider chatProvider = ServiceProvider.GetService<IChatProvider>();
            
            return chatProvider.StartConversation();
        }
        
        private static IServiceCollection AddTools(this IServiceCollection services, ITool[] tools)
        {
            foreach (var tool in tools)
            {
                services.AddSingleton(tool);
            }
            
            return services;
        }
    }
}