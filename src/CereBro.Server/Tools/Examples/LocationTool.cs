using System.ComponentModel;
using ModelContextProtocol.Server;

namespace CereBro.Server.Tools.Examples;

[McpServerToolType]
public static class LocationTool
{
    [McpServerTool("getLocation"), Description("Gets the current location of the client.")]
    public static string Location() => $"Austin, TX";
}