using System.Text.Json;
using MediatR;
using okai.Requests;

namespace okai;

public class ReadFileHandler : IRequestHandler<ReadFileQuery, ToolResult>
{
    private readonly IToolContext _context;
    private readonly IPathGuard _guard;

    public ReadFileHandler(IToolContext context, IPathGuard guard)
    {
        _context = context;
        _guard = guard;
    }

    public Task<ToolResult> Handle(ReadFileQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var target = _guard.Resolve(_context.Root, request.Path);
            if (!File.Exists(target))
            {
                return Task.FromResult(new ToolResult(JsonSerializer.Serialize(new { error = "file not found", path = request.Path }), $"file not found: {request.Path}"));
            }

            var content = File.ReadAllText(target);
            var payload = JsonSerializer.Serialize(new { path = request.Path, content });
            return Task.FromResult(new ToolResult(payload, $"read {request.Path} ({content.Length} chars)"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(new ToolResult(JsonSerializer.Serialize(new { error = ex.Message }), $"error: {ex.Message}"));
        }
    }
}
