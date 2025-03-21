using System.Linq;
using CereBro.Tools;
using Newtonsoft.Json.Linq;

namespace CereBro.Utilities;

public static class Utilities
{
    
    // Validates arguments against parameters (if required values exist)
    public static bool Validate(this JToken arguments, ToolParameters parameters, out string message)
    {
        message = string.Empty;
        
        var properties = parameters.Properties;
            
        string[] required = parameters.Required;
            
        for (int i = 0; i < properties.Count; i++)
        {
            var property = properties.ElementAt(i);
                
            string name = property.Key;
                
            var value = arguments[name];
                
            if (required.Contains(name) && value == null)
            {
                message = $"The {name} argument is missing";

                return false;
            }
        }

        return true;
    }
}