using System.ComponentModel;
using ModelContextProtocol.Server;

namespace CereBro.Server.Tools.Examples;

[McpToolType]
public static class WeatherTool
{
    [McpTool("getWeather"), Description("Gets the weather.")]
    public static string Weather(
        [Description("date for the weather")] string date,
        [Description("location for the weather.")] string location,
        [Description("tempature unit for the weather.")] string unit = "celsius")
    {
        return $"31 {unit}";
    }
}