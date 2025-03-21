using System.Threading.Tasks;
using CereBro.Tools;
using CereBro.Tools.Examples;

namespace CereBro.OpenAI;

public static class Program
{
    public static Task Main(string[] args)
    {
        return Runner.StartConversation<OpenAIChatProvider, ConsoleChatDispatcher>(new ITool[]
        {
            // Examples
            new LocationTool(),
            new DateTool(),
            new WeatherTool()
        });
    }
}