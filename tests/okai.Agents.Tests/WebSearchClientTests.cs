using okai;
using Xunit;

public class WebSearchClientTests
{
    [Fact]
    public async Task ReturnsFailure_WhenKeyMissing()
    {
        var opts = new AppOptions(
            "https://example.invalid",
            "model",
            Environment.CurrentDirectory,
            "hist",
            false,
            "policy",
            "default",
            "themes",
            "https://api.bing.microsoft.com/v7.0/search",
            string.Empty,
            "cmd",
            "profile",
            "cfg");

        var client = new WebSearchClient(opts);
        var result = await client.SearchAsync("test", default);

        Assert.False(result.Success);
        Assert.Contains("search key", result.Error ?? string.Empty);
    }
}
