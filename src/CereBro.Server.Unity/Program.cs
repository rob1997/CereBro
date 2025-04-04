using System.CommandLine;
using System.Threading.Tasks;

namespace CereBro.Server.Unity;

public static class Program
{
    public static Task Main(string[] args)
    {
        var portOption = new Option<int?>(name: "--port", description: "The port for sending MCP requests to Unity.",
            isDefault: true, parseArgument: result =>
            {
                if (result.Tokens.Count == 0)
                {
                    return 5000;
                }

                string input = result.Tokens[0].Value;

                if (int.TryParse(input, out int port))
                {
                    if (port <= 0 || port > 65535)
                    {
                        result.ErrorMessage = "Port must be between 0 and 65535.";
                    }

                    else
                    {
                        return port;
                    }
                }

                return null;
            });

        var rootCommand = new RootCommand("CereBro Unity Server");

        rootCommand.AddOption(portOption);

        rootCommand.SetHandler(Server.RunAsync, portOption);

        return rootCommand.InvokeAsync(args);
    }
}