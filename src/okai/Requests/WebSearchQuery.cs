using MediatR;

namespace okai.Requests;

public record WebSearchQuery(string Query) : IRequest<ToolResult>;
