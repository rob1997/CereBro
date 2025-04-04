using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ModelContextProtocol.Protocol.Types;
using ModelContextProtocol.Server;

namespace CereBro.Unity
{
    public static class ToolCallHandler
    {
        public static async Task<byte[]> HandleToolCall(string body, CancellationToken cancellationToken)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(body)))
            {
                var arguments = await JsonSerializer.DeserializeAsync<CallToolRequestParams>(stream, cancellationToken: cancellationToken);
                
                CallToolResponse response = await HandleToolCall(arguments, cancellationToken);
                
                string json = JsonSerializer.Serialize(response);
                                
                return Encoding.UTF8.GetBytes(json);
            }
        }
        
        private static Task<CallToolResponse> HandleToolCall(CallToolRequestParams callToolParams, CancellationToken cancellationToken)
        {
            McpServerTool tool = ToolListHandler.Tools.FirstOrDefault(t => t.ProtocolTool.Name == callToolParams.Name);

            if (tool != null)
            {
                return tool.InvokeAsync(new RequestContext<CallToolRequestParams>(null!, callToolParams), cancellationToken);
            }

            return Task.FromResult(new CallToolResponse
            {
                Content = new List<Content>
                {
                    new Content
                    {
                        Text = $"{callToolParams?.Name} Tool not registered",
                        Type = "text",
                    }
                },
                IsError = true,
            });
        }
    }
}