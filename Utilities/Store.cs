using System;

namespace Brain.Utilities;

public struct Store
{
    public static Store Instance
    {
        get
        {
            if (!_initialized)
            {
                _instance = new Store();

                _initialized = true;
            }
            
            return _instance;
        }
    }

    private static Store _instance;
    
    private static bool _initialized;
    
    public string ApiKey { get; private set; }

    public Store()
    {
        ApiKey = Environment.GetEnvironmentVariable("OPEN_AI_API_KEY");
    }
}