using MediatR;

namespace okai;

public interface IToolRequestFactory
{
    IRequest<ToolResult> Create(string name, Dictionary<string, string> args, string rawArgs);
}
