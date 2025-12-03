using System.Text.Json;
using MediatR;
using okai.Requests;

namespace okai;

public class ListDirHandler : IRequestHandler<ListDirQuery, ToolResult>
{
    private const int MaxEntries = 200;
    private readonly IToolContext _context;
    private readonly IPathGuard _guard;

    public ListDirHandler(IToolContext context, IPathGuard guard)
    {
        _context = context;
        _guard = guard;
    }

    public Task<ToolResult> Handle(ListDirQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var relative = request.Path ?? string.Empty;
            var target = _guard.Resolve(_context.Root, relative);
            if (!Directory.Exists(target))
            {
                return Task.FromResult(new ToolResult(JsonSerializer.Serialize(new { error = "path not found", path = relative }), $"path not found: {relative}"));
            }

            var entries = Directory.EnumerateFileSystemEntries(target)
                .Select(p => Path.GetFileName(p) + (Directory.Exists(p) ? "/" : string.Empty))
                .Take(MaxEntries)
                .ToArray();

            var payload = JsonSerializer.Serialize(new { path = relative, entries });
            return Task.FromResult(new ToolResult(payload, $"listed {entries.Length} entries in {relative}"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(new ToolResult(JsonSerializer.Serialize(new { error = ex.Message }), $"error: {ex.Message}"));
        }
    }
}
