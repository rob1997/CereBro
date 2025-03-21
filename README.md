# CereBro
[![GitHub release](https://img.shields.io/github/v/release/rob1997/CereBro?include_prereleases)](https://github.com/rob1997/CereBro/releases)
[![NuGet stable version](https://img.shields.io/nuget/v/Rob1997.CereBro)](https://www.nuget.org/packages/Rob1997.CereBro/)
[![GitHub license](https://img.shields.io/github/license/rob1997/TrackGenerator)](https://opensource.org/licenses/MIT)

CereBro is a model-agnostic AI Agent Wrapper for .Net. You can write Tools that can be used with different AI models without changing the code.

## Models

Below is a list of supported and planned models for CereBro.

**Supported:**

- [OpenAI](https://github.com/rob1997/CereBro/tree/main/src/CereBro.Open-AI/#cerebroopen-ai)

**Planned:**

- [Claude](https://claude.ai/)
- [Grok](https://x.ai/)
- [DeepSeek](https://www.deepseek.com/)
- [Gemini](https://gemini.google.com/)

## Installation

You can install the package from NuGet using the following command:

```bash
dotnet add package Rob1997.CereBro
```

## Usage

To use CereBro, you just need to call `Runner.StartConversation(ITool[] tools)` with an array of Tools.

```csharp
// You'll have to replace AIChatProvider with an implementation of IChatProvider
// See supported Models section above
Task conversation = Runner.StartConversation<AIChatProvider, ConsoleChatDispatcher>(new ITool[]
        {
            // Examples
            new LocationTool(),
            new DateTool(),
            new WeatherTool()
        });
// Awaitable Task for the conversation
```
`AIChatProvider` is the Chat Provider for the AI model you want to use, and `ConsoleChatDispatcher` is the Dispatcher that will handle the conversation. You'll have to replace `AIChatProvider` with an implementation of the `IChatProvider`, you can use once the supported section under [Models](#models) providers, or you can (implement)[] your own.

To create a new Tool, you need to create a new class that implements the `ITool` interface. You can then implement the `Execute(JToken arguments)` method to run/execute your Tool. You can find examples of Tools [here](https://github.com/rob1997/CereBro/tree/main/src/CereBro/Tools/Examples).

## How it Works

If you would like to know how it works, I've a dev-log entry on it [here](https://rob1997.github.io/devlog/log-5.html).

## Contributing

If you'd like to contribute to the project, you can fork the repository and create a pull request. You can also create an issue if you find any bugs or have any feature requests.

---