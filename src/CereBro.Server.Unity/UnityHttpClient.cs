using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ModelContextProtocol.Protocol.Types;

namespace CereBro.Server.Unity;

public class UnityHttpClient
{
    private readonly ICereBroServerUnityConfig _config;
    
    public UnityHttpClient(ICereBroServerUnityConfig config)
    {
        _config = config;
    }

    private async Task<TResponse> SendRequestToUnityAsync<TResponse>(HttpMethod method, string endpoint, object data,
        CancellationToken cancellationToken)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpContent content = null;

            if (data != null)
            {
                string json = JsonSerializer.Serialize(data);

                content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            var request = new HttpRequestMessage(method, Combine(_config.Url, endpoint)) { Content = content };

            var response = await client.SendAsync(request, cancellationToken);

            return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken);
        }
    }

    public async Task<ListToolsResult> GetToolListAsync(CancellationToken cancellationToken)
    {
        return await SendRequestToUnityAsync<ListToolsResult>(HttpMethod.Get, "tools/list", null,
            cancellationToken);
    }

    public async Task<CallToolResponse> CallToolAsync(CallToolRequestParams requestParams,
        CancellationToken cancellationToken)
    {
        return await SendRequestToUnityAsync<CallToolResponse>(HttpMethod.Post, "tools/call", requestParams,
            cancellationToken);
    }

    private static Uri Combine(Uri url, string path)
    {
        return new Uri(url, path);
    }
}