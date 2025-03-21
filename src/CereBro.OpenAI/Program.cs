using System.Threading.Tasks;
using CereBro;

namespace CereBro.OpenAI;

public static class Program
{
    public static Task Main(string[] args)
    {
        return Runner.StartConversation<OpenAIChatProvider, ConsoleChatDispatcher>();
    }
}