using System.Threading.Tasks;
using Brain.Core;

namespace Brain.OpenAI;

public static class Program
{
    public static Task Main(string[] args)
    {
        return Runner.StartConversation<OpenAIChatProvider, ConsoleChatDispatcher>();
    }
}