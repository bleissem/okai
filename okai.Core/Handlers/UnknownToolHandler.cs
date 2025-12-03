using System.Text.Json;
using MediatR;
using okai.Requests;

namespace okai;

public class UnknownToolHandler : IRequestHandler<UnknownToolQuery, ToolResult>
{
    public Task<ToolResult> Handle(UnknownToolQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new ToolResult(JsonSerializer.Serialize(new { error = $"unknown tool: {request.Name}", rawArgs = request.RawArgs }), $"error: unknown tool {request.Name}"));
    }
}
