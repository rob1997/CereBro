using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Client;
using ModelContextProtocol.Configuration;
using ModelContextProtocol.Protocol.Types;

namespace CereBro;

public class ChatProvider : BackgroundService, IAsyncDisposable
{
    private IList<IMcpClient> _mcpClients = [];

    private readonly IChatClient _chatClient;

    private readonly IChatDispatcher _chatDispatcher;
    
    private readonly ICereBroConfig _config;

    private List<AIFunction> _tools = [];

    private readonly List<ChatMessage> _messages = [];

    public ChatProvider(IChatClient chatClient, IChatDispatcher chatDispatcher, ICereBroConfig config)
    {
        _chatClient = chatClient;
        _chatDispatcher = chatDispatcher;
        _config = config;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        FileStream stream = File.OpenRead(_config.ServersFilePath);

        var servers =
            await JsonSerializer.DeserializeAsync<McpServerConfig[]>(stream, cancellationToken: cancellationToken);

        foreach (var server in servers)
        {
            var mcpClient = await McpClientFactory.CreateAsync(server,
                new McpClientOptions { ClientInfo = new Implementation { Name = "CereBro", Version = "1.0.0", } },
                cancellationToken: cancellationToken);
            
            _tools.AddRange(await mcpClient.GetAIFunctionsAsync(cancellationToken: cancellationToken));
            
            _mcpClients.Add(mcpClient);
        }

        // Start ExecuteAsync()
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stopToken)
    {
        while (!stopToken.IsCancellationRequested)
        {
            await PromptUser();

            await GetStreamingResponseAsync(stopToken);
        }
    }

    private async Task PromptUser()
    {
        // If the last message isn't from the Assistant then don't prompt the User
        // Sometimes it could be a tool call
        if (_messages.Count >= 2 && _messages[^1]?.Role != ChatRole.Assistant)
        {
            return;
        }

        string message = await _chatDispatcher.PromptUser();

        _messages.Add(new(ChatRole.User, message));
    }

    private async Task GetStreamingResponseAsync(CancellationToken cancellationToken)
    {
        List<ChatResponseUpdate> updates = [];

        int index = 0;

        await foreach (var update in _chatClient.GetStreamingResponseAsync(_messages, new() { Tools = [.. _tools] },
                           cancellationToken: cancellationToken))
        {
            updates.Add(update);

            if (update.Role == ChatRole.Assistant)
            {
                FunctionCallContent[] calls = update.Contents.OfType<FunctionCallContent>().ToArray();

                if (calls.Length != 0)
                {
                    _messages.AddMessages(updates);

                    foreach (var call in calls)
                    {
                        CallToolResponse response = default;

                        if (await _chatDispatcher.PromptToolCall(call.Name))
                        {
                            if (GetTool(call.Name, out AIFunction tool))
                            {
                                var result = await tool.InvokeAsync((Dictionary<string, object>)call.Arguments ?? [],
                                    cancellationToken: cancellationToken);

                                string json = result?.ToString();
                                
                                if (json != null)
                                {
                                    using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
                                
                                    response = await JsonSerializer.DeserializeAsync<CallToolResponse>(stream, cancellationToken: cancellationToken);
                                }
                                
                                else
                                {
                                    response = ErrorResponse($"Tool '{call.Name}' returned null.");
                                }
                            }

                            else
                            {
                                response = ErrorResponse($"Tool '{call.Name}' not found.");
                            }
                        }

                        else
                        {
                            response = new CallToolResponse
                            {
                                Content =
                                [
                                    new Content { Text = "Tool Invocation Rejected by User", Type = "text" }
                                ],
                                IsError = true,
                            };
                        }

                        _messages.Add(new ChatMessage(ChatRole.Tool,
                            [new FunctionResultContent(call.CallId, JsonSerializer.Serialize(response))]));
                    }

                    // GetStreamingResponseAsync() goes on to execute tools ¯\_(ツ)_/¯
                    return;
                }

                if (!string.IsNullOrEmpty(update.Text))
                {
                    await _chatDispatcher.ShowStreamingResponse(update.Text, index++);
                }
            }
        }

        _messages.AddMessages(updates);
    }

    private bool GetTool(string name, out AIFunction tool)
    {
        tool = _tools.FirstOrDefault(t => t.Name == name);

        return tool != null;
    }
    
    private static CallToolResponse ErrorResponse(string message)
    {
        return new CallToolResponse
        {
            Content =
            [
                new Content { Text = message, Type = "text" }
            ],
            IsError = true,
        };
    }
    
    public override void Dispose()
    {
        base.Dispose();

        _chatClient.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var mcpClient in _mcpClients)
        {
            await mcpClient.DisposeAsync();
        }
    }
}