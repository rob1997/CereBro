# CereBro
[![GitHub release](https://img.shields.io/github/v/release/rob1997/CereBro?include_prereleases)](https://github.com/rob1997/CereBro/releases)
[![NuGet stable version](https://img.shields.io/nuget/v/Rob1997.CereBro)](https://www.nuget.org/packages/Rob1997.CereBro/)
[![GitHub license](https://img.shields.io/github/license/rob1997/CereBro)](https://opensource.org/licenses/MIT)

CereBro is a model-agnostic AI Agent Wrapper for .Net. Now with 🔥 **[Model Context Protocol](https://modelcontextprotocol.io/)** 🔥 , based on the [Official C# SDK](https://github.com/modelcontextprotocol/csharp-sdk), you can write Tools that can be used with different AI models without changing the code.

## Models

Below is a list of supported and planned models for CereBro.

**Supported:**

- [OpenAI](https://github.com/rob1997/CereBro/tree/main/src/CereBro.Open-AI/#cerebroopen-ai)

**Planned:**

- [Claude](https://claude.ai/)
- [Grok](https://x.ai/)
- [DeepSeek](https://www.deepseek.com/)
- [Gemini](https://gemini.google.com/)
- [Ollama](https://ollama.com/)

## Installation

You can install the package from NuGet using the following command:

```bash
dotnet add package Rob1997.CereBro

dotnet add package Rob1997.CereBro.Open-AI
```

## Usage

### Step 1: Create a `servers.json` file

This file will contain the configuration for the MCP servers you want to use. Below is an example of the `servers.json` file.

```json
[
  {
    "Id": "everything-server",
    "Name": "Everything",
    "TransportType": "stdio",
    "TransportOptions": {
      "command": "npx",
      "arguments": "-y @modelcontextprotocol/server-everything"
    }
  }
]
```
You can check out more servers [here](https://github.com/modelcontextprotocol/servers/).

### Step 2: Add your OpenAI API Key to your environment variables

```bash
export OPEN_AI_API_KEY="your-api-key"
```

```powershell
$env:OPEN_AI_API_KEY="your-api-key"
```

If you want this to be permanent, you can add it to your `.bashrc` or `.bash_profile` file in linux or use the following command in PowerShell.

```powershell
[Environment]::SetEnvironmentVariable("OPEN_AI_API_KEY", "your-api-key", "User")
```

### Step 3: Add the following code to your `Program.cs` (Entry Point)

```csharp
public static async Task Main(string[] args)
{
    var builder = Host.CreateApplicationBuilder(args);
    
    builder.Services.UseOpenAI(Environment.GetEnvironmentVariable("OPEN_AI_API_KEY"), "gpt-4o-mini");
            
    IHost cereBro = builder.BuildCereBro(new CereBroConfig{ ServersFilePath = "./servers.json" });

    await cereBro.RunAsync();
}
```

CereBro uses the Console as a chat dispatcher. You can create your own dispatcher by implementing the [`IChatDispatcher`](https://github.com/rob1997/CereBro/blob/main/src/CereBro/IChatDispatcher.cs) interface and use `builder.BuildCereBro<IChatDispatcher>(config)` to build CereBro's host.

### Step 4: Run your application

```bash
dotnet run
```

## Adding a new Model

Currently, CereBro only supports [OpenAI's models](https://platform.openai.com/docs/models). To add a new model you'll need to Implement `Microsoft.Extensions.AI.IChatClient`, unless it already exists, Microsoft already has implementations for some models like [OpenAI](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.ai.openaichatclient?view=net-9.0-pp) and [Ollama](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.ai.ollamachatclient?view=net-9.0-pp).

Once you've done that you can create a Placeholder Type that implements `Microsoft.Extensions.AI.FunctionInvokingChatClient` something like [this](https://github.com/rob1997/CereBro/blob/main/src/CereBro.Open-AI/OpenAIFunctionInvokingChatClient.cs).

Finally, you can use the `UseChatClient<T>(this IServiceCollection services, IChatClient chatClient)
where T : FunctionInvokingChatClient` extension method to add your model to the service collection.

⚠️ **Note** ⚠️

At the moment CereBro doesn't support multiple models at the same time, so you'll have to remove the `UseOpenAI` method from the `Program.cs` file to use another model.

## Contributing

If you'd like to contribute to the project, you can fork the repository and create a pull request. You can also create an issue if you find any bugs or have any feature requests.

---