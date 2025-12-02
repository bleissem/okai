using okai;
using okai.Requests;
using Xunit;

public class WebSearchHandlerTests
{
    [Fact]
    public async Task ReturnsError_WhenClientFails()
    {
        var handler = new WebSearchHandler(new FakeClient(WebSearchResult.Failure("missing key")));
        var result = await handler.Handle(new WebSearchQuery("test"), default);

        Assert.Contains("missing key", result.PayloadForModel);
    }

    [Fact]
    public async Task ReturnsResults_WhenClientSucceeds()
    {
        var hit = new WebSearchHit("title", "https://example.com", "snippet");
        var handler = new WebSearchHandler(new FakeClient(new WebSearchResult(true, new List<WebSearchHit> { hit })));

        var result = await handler.Handle(new WebSearchQuery("test"), default);

        Assert.Contains("title", result.PayloadForModel);
        Assert.Contains("example.com", result.PayloadForModel);
    }

    private sealed class FakeClient : IWebSearchClient
    {
        private readonly WebSearchResult _result;
        public FakeClient(WebSearchResult result) => _result = result;
        public Task<WebSearchResult> SearchAsync(string query, CancellationToken cancellationToken) => Task.FromResult(_result);
    }
}
