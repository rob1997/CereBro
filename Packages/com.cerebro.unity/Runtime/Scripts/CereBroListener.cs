using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CereBro.Unity
{
    public delegate Task<byte[]> RequestHandlerDelegate(string body = null, CancellationToken cancellationToken = default);
    
    public static class CereBroListener
    {
        private static readonly Dictionary<string, RequestHandlerDelegate> RequestHandlers = new Dictionary<string, RequestHandlerDelegate>
        {
            {"tools/list", ToolListHandler.HandleToolList},
            {"tools/call", ToolCallHandler.HandleToolCall},
        };
        
        private static HttpListener _listener;

#if UNITY_EDITOR
        public static async void RunForEditor(int port = 5000)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            
            AssemblyReloadEvents.beforeAssemblyReload += () =>
            {
                cancellationTokenSource.Cancel();
                
                Close();
            };
            
            await Run(port, cancellationTokenSource.Token);
        }
#endif

        public static async Task Run(int port = 5000, CancellationToken cancellationToken = default)
        {
            using (_listener = new HttpListener())
            {
                Uri url = new Uri($"http://localhost:{port}");
                
                try
                {
                    _listener.Prefixes.Add($"{url}/");
                    
                    foreach (string path in RequestHandlers.Keys)
                    {
                        _listener.Prefixes.Add($"{url.Combine(path)}/");
                    }

                    _listener.Start();
                   
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var getContext = _listener.GetContextAsync();

                        if (getContext !=
                            // This is a workaround for the fact that GetContextAsync() doesn't support cancellation tokens
                            await Task.WhenAny(getContext, Task.Delay(Timeout.Infinite, cancellationToken)))
                        {
                            break;
                        }
                        
                        var context = getContext.Result;

                        var request = context.Request;

                        string path = request.Url.AbsolutePath.TrimStart('/');
                        
                        var response = context.Response;
                        
                        if (RequestHandlers.TryGetValue(path, out RequestHandlerDelegate handler) && handler != null)
                        {
                            try
                            {
                                using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
                                {
                                    string body = await reader.ReadToEndAsync();
                                    
                                    byte[] result = await handler.Invoke(body, cancellationToken);

                                    response.ContentLength64 = result.Length;

                                    await using (Stream output = response.OutputStream)
                                    {
                                        await output.WriteAsync(result, 0, result.Length, cancellationToken);
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                response.ErrorResponse();
                            }
                        }

                        else
                        {
                            response.ErrorResponse((int) HttpStatusCode.NotFound);
                        }
                    }
                }
                finally
                {
                    Close();
                }
            }
        }

        private static Uri Combine(this Uri url, string path)
        {
            return new Uri(url, path);
        }
        
        private static void ErrorResponse(this HttpListenerResponse response, int statusCode = (int) HttpStatusCode.InternalServerError)
        {
            response.StatusCode = statusCode;
            
            response.Close();
        }

        private static void Close()
        {
            if (_listener != null)
            {
                _listener.Stop();
                
                _listener.Close();
                
                _listener = null;
            }
        }
    }
}