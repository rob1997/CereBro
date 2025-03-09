using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Brain.Tools;

public class WeatherTool : OpenAiTool
{
    public override string Name => "GetWeather";
    
    public override string Description => "Get the weather in a given location";

    public override ToolParameters? Parameters => new ToolParameters(
      properties: new Dictionary<string, ToolProperty>
    {
        {"location", new ToolProperty("string", "The city and state, e.g. Boston, MA")},
        {"date", new ToolProperty("string", "The current date, e.g. March 1, 2019")},
        {"unit", new ToolProperty("string", "The temperature unit to use. Infer this from the specified location.", new[] { "celsius", "fahrenheit" })}
    },
        required: new[] { "location", "date" }
    );

    public override Task<string> Execute(JToken arguments)
    {
        string location = arguments["location"].ToString();
        
        string date = arguments["date"].ToString();
        
        string unit = arguments["unit"] != null ? arguments["unit"].ToString() : "celcius";
        
        // TODO use some weather API to get the weather for the specified location and date
        return Task.FromResult($"31 {unit}");
    }
}