using System.Text;

namespace okai;

public record ToolCallBuffer(string Id)
{
    public int Index { get; set; }
    public string? FunctionName { get; set; }
    public StringBuilder Arguments { get; } = new();
}
