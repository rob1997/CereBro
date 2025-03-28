using System.ComponentModel;
using ModelContextProtocol.Server;

namespace CereBro.Server.Tools.Examples;

[McpToolType]
public static class LocationTool
{
    [McpTool("getLocation"), Description("Gets the current location of the client.")]
    public static string Location() => $"Austin, TX";
}