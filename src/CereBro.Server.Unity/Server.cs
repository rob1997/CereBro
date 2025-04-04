using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol;
using ModelContextProtocol.Protocol.Types;
using ModelContextProtocol.Server;

namespace CereBro.Server.Unity
{
    public static class Server
    {
        public static Task RunAsync(int? port)
        {
            var builder = Host.CreateApplicationBuilder();

            builder.Services.AddSingleton<ICereBroServerUnityConfig>(new CereBroServerUnityConfig
            {
                Url = new Uri($"http://localhost:{port}"),
            });

            builder.Services.AddSingleton<UnityHttpClient>();

            builder.Services.AddMcpServer(configureOptions: options =>
                {
                    AssemblyName assembly = Assembly.GetExecutingAssembly().GetName();

                    options.ServerInfo = new Implementation
                    {
                        Name = assembly.FullName, Version = assembly.Version?.ToString() ?? "1.0.0",
                    };
                })
                .WithStdioServerTransport()
                .WithListToolsHandler(ListToolsHandler)
                .WithCallToolHandler(CallToolHandler);

            IHost app = builder.Build();

            return app.RunAsync();
        }

        private static UnityHttpClient GetUnityHttpClient(IMcpServer server)
        {
            if (server.Services != null)
            {
                return server.Services.GetRequiredService<UnityHttpClient>();
            }

            throw new McpServerException($"{nameof(UnityHttpClient)} not configured.");
        }

        private static async Task<ListToolsResult> ListToolsHandler(RequestContext<ListToolsRequestParams> request,
            CancellationToken cancellationToken)
        {
            UnityHttpClient client = GetUnityHttpClient(request.Server);

            return await client.GetToolListAsync(cancellationToken);
        }

        private static async Task<CallToolResponse> CallToolHandler(RequestContext<CallToolRequestParams> request,
            CancellationToken cancellationToken)
        {
            UnityHttpClient client = GetUnityHttpClient(request.Server);

            return await client.CallToolAsync(request.Params, cancellationToken);
        }
    }
}