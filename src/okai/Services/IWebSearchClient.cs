namespace okai;

public record WebSearchHit(string Title, string Url, string Snippet);

public record WebSearchResult(bool Success, List<WebSearchHit> Results, string? Error = null)
{
    public static WebSearchResult Failure(string message) => new(false, new List<WebSearchHit>(), message);
}

public interface IWebSearchClient
{
    Task<WebSearchResult> SearchAsync(string query, CancellationToken cancellationToken);
}
