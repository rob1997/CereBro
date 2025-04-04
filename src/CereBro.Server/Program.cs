using System.Reflection;
using ModelContextProtocol;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using ModelContextProtocol.Protocol.Types;

namespace CereBro.Server
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
                
            builder.Services.AddMcpServer(configureOptions: options =>
                {
                    AssemblyName assembly = Assembly.GetExecutingAssembly().GetName();

                    options.ServerInfo = new Implementation
                    {
                        Name = assembly.FullName, Version = assembly.Version?.ToString() ?? "1.0.0",
                    };
                })
                .WithStdioServerTransport()
                .WithToolsFromAssembly();

            IHost app = builder.Build();

            await app.RunAsync();
        }
    }
}