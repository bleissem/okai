using System.Text.Json;
using MediatR;
using okai.Requests;

namespace okai;

public class WriteFileHandler : IRequestHandler<WriteFileCommand, ToolResult>
{
    private readonly IToolContext _context;
    private readonly IPathGuard _guard;

    public WriteFileHandler(IToolContext context, IPathGuard guard)
    {
        _context = context;
        _guard = guard;
    }

    public Task<ToolResult> Handle(WriteFileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var target = _guard.Resolve(_context.Root, request.Path);
            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            File.WriteAllText(target, request.Content);
            var payload = JsonSerializer.Serialize(new { path = request.Path, writtenBytes = request.Content.Length });
            return Task.FromResult(new ToolResult(payload, $"wrote {request.Path} ({request.Content.Length} chars)"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(new ToolResult(JsonSerializer.Serialize(new { error = ex.Message }), $"error: {ex.Message}"));
        }
    }
}
