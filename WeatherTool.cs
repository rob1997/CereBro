using System;
using System.Text.Json;
using OpenAI.Chat;

namespace Brain;

public class WeatherTool : OpenAiTool
{
    public override string Name => "GetWeather";
    
    public override ChatTool ChatTool => ChatTool.CreateFunctionTool(
        functionName: Name, functionDescription: "Get the weather in a given location",
        functionParameters: BinaryData.FromBytes("""
                                                 {
                                                     "type": "object",
                                                     "properties": {
                                                         "location": {
                                                             "type": "string",
                                                             "description": "The city and state, e.g. Boston, MA"
                                                         },
                                                         "date": {
                                                            "type": "string",
                                                            "description": "The current date, e.g. March 1, 2019"
                                                        },
                                                         "unit": {
                                                             "type": "string",
                                                             "enum": [ "celsius", "fahrenheit" ],
                                                             "description": "The temperature unit to use. Infer this from the specified location."
                                                         }
                                                     },
                                                     "required": [ "location", "date" ]
                                                 }
                                                 """u8.ToArray()));

    public override string Execute(BinaryData arguments)
    {
        using JsonDocument argumentsJson = JsonDocument.Parse(arguments);
        bool hasLocation =
            argumentsJson.RootElement.TryGetProperty("location", out JsonElement location);
                                    
        bool hasDate = argumentsJson.RootElement.TryGetProperty("date", out JsonElement date);
        
        bool hasUnit = argumentsJson.RootElement.TryGetProperty("unit", out JsonElement unit);

        //Note that the model may hallucinate arguments too. Consequently, it is important to do the
        // appropriate parsing and validation before calling the function.
        if (!hasLocation)
        {
            throw new ArgumentNullException(nameof(location),
                "The location argument is required");
        }
        
        if (!hasDate)
        {
            throw new ArgumentNullException(nameof(date),
                "The date argument is required");
        }
                                    
        string toolResult = hasUnit
            ? Execute(location.GetString(), date.GetString(), unit.GetString())
            : Execute(location.GetString(), date.GetString());

        return toolResult;
    }

    private static string Execute(string location, string date, string unit = "celsius")
    {
        // Call the weather API here.
        return $"31 {unit}";
    }
}