# CereBro.Unity

CereBro.Unity is a model-agnostic AI Agent Wrapper for Unity based on [CereBro](https://github.com/rob1997/CereBro/). Now with [Model Context Protocol](https://modelcontextprotocol.io/), based on the [Official C# SDK](https://github.com/modelcontextprotocol/csharp-sdk), you can write Tools that can be used with different AI models without changing the code.

## Installation

### [Git URL](https://docs.unity3d.com/Manual/upm-ui-giturl.html)

You can Install CereBro.Unity Package from Git URL [https://github.com/rob1997/CereBro.git?path=/Packages/com.cerebro.unity](https://github.com/rob1997/CereBro.git?path=/Packages/com.cerebro.unity)

### Open UPM

Not yet.

## Usage

### MCP Server

#### Step 1: Import Server

From the Menu Bar select `CereBro > Import Server`

This will import `CereBro.Server.Unity` in the root directory of your Unity Project. This will serve as an intermediary MCP Server that'll relay data to and from Unity.

#### Step 2: Configure Client

You need to configure your specific chat client to use `CereBro.Server.Unity`. You can do this by adding this to your servers config file.

```json
{
  "unity-server": {
    "command": "dotnet \"<Path To Unity Project>/CereBro.Server.Unity/CereBro.Server.Unity.dll\"",
    "args": [
      "--port <Port Number>"
    ]
  }
}
```

If you're using `CereBro` as a client you can add this to your `servers.json` file.

```json
[
  {
    "Id": "unity-server",
    "Name": "CereBro.Server.Unity",
    "TransportType": "stdio",
    "TransportOptions": {
      "command": "dotnet \"<Path To Unity Project>/CereBro.Server.Unity/CereBro.Server.Unity.dll\"",
      "arguments": "--port <Port Number>"
    }
  }
]
```

#### Step 3: Run CereBro in Unity

Next you need to run CereBro in Unity. This will create a listener that'll listen for requests from `CereBro.Server.Unity`.

**Editor:**

```csharp
[InitializeOnLoad]
public static class CereBroEditorRunner
{
    static CereBroEditorRunner()
    {
        CereBroListener.RunForEditor(5050);
    }
}
```

**Runtime:**

To run CereBro in runtime, you need to call `CereBroListener.Run(int port = 5000, CancellationToken cancellationToken = default)` method, which will return an Awaitable Task.

Now you can start your Client and use CereBro in Unity.

### MCP Client

Not yet.

## CereBro.Unity Sample

You can import CereBro.Unity's Sample from the Package Manager. It contains Editor tools that can be called from an MCP Client.

- **CreateShapeTool:** Creates a primitive shape such as a cube, sphere, cylinder, etc... in the active scene.
- **GetCurrentPositionTool:** Gets the current position of a GameObject in the active scene.
- **MoveGameObjectTool:** Moves a GameObject in the active scene by a specified amount in the x, y, and z directions.
- **RePositionTool:** Repositions a GameObject in the active scene to a specified position.
- **RotateGameObjectTool:** Rotates a GameObject in the active scene by a specified amount along the x, y, and z axis.

## Contributing

If you'd like to contribute to the project, you can fork the repository and create a pull request. You can also create an issue if you find any bugs or have any feature requests.

---