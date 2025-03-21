using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace CereBro
{
    public static class Runner
    {
        public static ServiceProvider ServiceProvider { get; private set; }
        
        public static Task StartConversation<TChatProvider, TChatDispatcher>()
            where TChatProvider : class, IChatProvider
            where TChatDispatcher : class, IChatDispatcher
        {
            ServiceCollection services = new ServiceCollection();

            services.AddSingleton<IChatProvider, TChatProvider>();
            
            services.AddSingleton<IChatDispatcher, TChatDispatcher>();

            ServiceProvider = services.BuildServiceProvider();

            IChatProvider chatProvider = ServiceProvider.GetService<IChatProvider>();
            
            return chatProvider.StartConversation();
        }
    }
}