using ModelContextProtocol;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace CereBro.Server
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddMcpServer()
                .WithStdioServerTransport()
                .WithTools();

            IHost app = builder.Build();

            await app.RunAsync();
        }
    }
}