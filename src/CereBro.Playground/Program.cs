using System.Threading.Tasks;
using CereBro.OpenAI;
using CereBro.Tools;
using CereBro.Tools.Examples;

namespace CereBro.Playground
{
    public static class Program
    {
        public static Task Main(string[] args)
        {
            return Runner.StartConversation<OpenAIChatProvider, ConsoleChatDispatcher>(new ITool[]
            {
                new LocationTool(),
                new DateTool(),
                new WeatherTool()
            });
        }
    }
}