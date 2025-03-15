using System.Threading.Tasks;

namespace Brain
{
    public static class Program
    {   
        public static Task Main(string[] args)
        {
            var chatProvider = new OpenAiChatProvider();
            
            return (chatProvider as IChatProvider).StartConversation();
        }
    }
}