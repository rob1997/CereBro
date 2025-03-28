using System;
using System.ComponentModel;
using System.Globalization;
using ModelContextProtocol.Server;

namespace CereBro.Server.Tools.Examples;

[McpToolType]
public static class DateTool
{
    [McpTool("getDate"), Description("Gets the current date.")]
    public static string GetDate(
        [Description("format for the returned date.")]
        string format = "dd/M/yyyy")
    {
        return DateTime.Now.ToString(format, CultureInfo.InvariantCulture);
    }
}