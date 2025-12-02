using System.Net.Http;
using System.Text.Json;

namespace okai;

public class WebSearchClient : IWebSearchClient
{
    private const int ResultCount = 5;
    private readonly AppOptions _options;
    private readonly HttpClient _http;

    public WebSearchClient(AppOptions options)
    {
        _options = options;
        _http = new HttpClient();
    }

    public async Task<WebSearchResult> SearchAsync(string query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return WebSearchResult.Failure("query is required");
        }

        if (string.IsNullOrWhiteSpace(_options.SearchKey))
        {
            return WebSearchResult.Failure("search key not configured (set OKAI_SEARCH_KEY)");
        }

        try
        {
            var uri = $"{_options.SearchEndpoint}?q={Uri.EscapeDataString(query)}&count={ResultCount}";
            using var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Add("Ocp-Apim-Subscription-Key", _options.SearchKey);
            using var response = await _http.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return WebSearchResult.Failure($"search failed: {(int)response.StatusCode}");
            }

            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
            if (!doc.RootElement.TryGetProperty("webPages", out var webPages) ||
                !webPages.TryGetProperty("value", out var value) ||
                value.ValueKind != JsonValueKind.Array)
            {
                return new WebSearchResult(true, new List<WebSearchHit>());
            }

            var hits = new List<WebSearchHit>();
            foreach (var item in value.EnumerateArray())
            {
                var name = item.TryGetProperty("name", out var n) ? n.GetString() ?? string.Empty : string.Empty;
                var url = item.TryGetProperty("url", out var u) ? u.GetString() ?? string.Empty : string.Empty;
                var snippet = item.TryGetProperty("snippet", out var s) ? s.GetString() ?? string.Empty : string.Empty;
                if (!string.IsNullOrWhiteSpace(url))
                {
                    hits.Add(new WebSearchHit(name, url, snippet));
                }
            }

            return new WebSearchResult(true, hits);
        }
        catch (Exception ex)
        {
            return WebSearchResult.Failure($"search error: {ex.Message}");
        }
    }
}
