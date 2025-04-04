using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ModelContextProtocol.Protocol.Types;
using ModelContextProtocol.Server;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace CereBro.Unity
{
    public static class ToolListHandler
    {
        public static List<McpServerTool> Tools
        {
            get
            {
                if (_tools != null)
                {
                    return _tools;
                }
                
                _tools = new List<McpServerTool>();
                
                // Get all types with [McpServerToolType] Attribute
                IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(type => type.GetCustomAttribute<McpServerToolTypeAttribute>() != null);

                foreach (Type type in types)
                {
                    foreach (MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        if (method.GetCustomAttribute<McpServerToolAttribute>() != null)
                        {
                            _tools.Add(method.IsStatic ? 
                                McpServerTool.Create(method) : McpServerTool.Create(method, type));
                        }
                    }
                }
                
                return _tools;
            }
        }

        private static List<McpServerTool> _tools;
        
        public static Task<byte[]> HandleToolList(string body, CancellationToken cancellationToken)
        {
            string json = JsonSerializer.Serialize(new ListToolsResult
            {
                Tools = Tools.Select(t => t.ProtocolTool).ToList()
            });
            
            return Task.FromResult(Encoding.UTF8.GetBytes(json));
        }
    }
}