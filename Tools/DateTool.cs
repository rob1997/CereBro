using Newtonsoft.Json.Linq;

namespace Brain.Tools;

public class DateTool : OpenAiTool
{
    public override string Name => "GetCurrentDate";
    
    public override string Description => "Get the current date";
    
    public override ToolParameters? Parameters => null;

    protected override string Execute(JToken arguments)
    {
        return "January 1, 2025";
    }
}