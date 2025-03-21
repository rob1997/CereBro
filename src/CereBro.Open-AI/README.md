# CereBro.Open-AI

[![NuGet stable version](https://img.shields.io/nuget/v/Rob1997.CereBro.Open-AI)](https://www.nuget.org/packages/Rob1997.CereBro.Open-AI)
[![GitHub license](https://img.shields.io/github/license/rob1997/TrackGenerator)](https://opensource.org/licenses/MIT)

CereBro.Open-AI is OpenAI's implementation for CereBro.

## Installation

You can install the package from NuGet using the following command:

```bash
dotnet add package Rob1997.CereBro.Open-AI
```

## Usage

Once you've installed the package, you'll need to add OpenAI's API Key to your environment variables.

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

Once you've set the API Key, you can use the `OpenAIChatProvider` in your `Runner.StartConversation` method.

```csharp
// Awaitable Task for the conversation
Task conversation = Runner.StartConversation<OpenAIChatProvider, ConsoleChatDispatcher>(new ITool[]
        {
            // Examples
            new LocationTool(),
            new DateTool(),
            new WeatherTool()
        });
```