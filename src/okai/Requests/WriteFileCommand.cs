using MediatR;

namespace okai.Requests;

public record WriteFileCommand(string Path, string Content) : IRequest<ToolResult>;
