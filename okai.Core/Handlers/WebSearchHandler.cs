using System.Text.Json;
using MediatR;
using okai.Requests;

namespace okai;

public class WebSearchHandler : IRequestHandler<WebSearchQuery, ToolResult>
{
    private readonly IWebSearchClient _client;

    public WebSearchHandler(IWebSearchClient client)
    {
        _client = client;
    }

    public async Task<ToolResult> Handle(WebSearchQuery request, CancellationToken cancellationToken)
    {
        var result = await _client.SearchAsync(request.Query, cancellationToken);
        if (!result.Success)
        {
            return new ToolResult(JsonSerializer.Serialize(new { error = result.Error }), result.Error ?? "search failed");
        }

        var payload = JsonSerializer.Serialize(new { query = request.Query, results = result.Results });
        return new ToolResult(payload, $"found {result.Results.Count} results");
    }
}
