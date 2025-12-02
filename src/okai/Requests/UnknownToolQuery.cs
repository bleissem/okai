using MediatR;

namespace okai.Requests;

public record UnknownToolQuery(string Name, string RawArgs) : IRequest<ToolResult>;
