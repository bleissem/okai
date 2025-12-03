using MediatR;

namespace okai.Requests;

public record ReadFileQuery(string Path) : IRequest<ToolResult>;
