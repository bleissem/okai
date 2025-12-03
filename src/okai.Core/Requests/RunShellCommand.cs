using MediatR;

namespace okai.Requests;

public record RunShellCommand(string Command) : IRequest<ToolResult>;
