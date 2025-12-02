using MediatR;

namespace okai.Requests;

public record ListDirQuery(string? Path) : IRequest<ToolResult>;
