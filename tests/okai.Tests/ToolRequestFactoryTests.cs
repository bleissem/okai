using MediatR;
using okai;
using okai.Requests;
using Xunit;

public class ToolRequestFactoryTests
{
    [Fact]
    public void MapsToKnownRequests()
    {
        var factory = new ToolRequestFactory();
        Assert.IsType<ListDirQuery>(factory.Create("list_dir", new Dictionary<string, string>(), "{}"));
        Assert.IsType<ReadFileQuery>(factory.Create("read_file", new Dictionary<string, string> { { "path", "a.txt" } }, "{}"));
        Assert.IsType<WriteFileCommand>(factory.Create("write_file", new Dictionary<string, string> { { "path", "a.txt" }, { "content", "hi" } }, "{}"));
        Assert.IsType<RunShellCommand>(factory.Create("run_shell", new Dictionary<string, string> { { "command", "echo ok" } }, "{}"));
        Assert.IsType<WebSearchQuery>(factory.Create("web_search", new Dictionary<string, string> { { "query", "test" } }, "{}"));
    }

    [Fact]
    public void UnknownToolMapsToUnknownQuery()
    {
        var factory = new ToolRequestFactory();
        var req = factory.Create("unknown", new Dictionary<string, string>(), "raw");
        Assert.IsType<UnknownToolQuery>(req);
    }

    [Fact]
    public void MissingRequiredArg_Throws()
    {
        var factory = new ToolRequestFactory();
        Assert.Throws<InvalidOperationException>(() => factory.Create("read_file", new Dictionary<string, string>(), "{}"));
    }
}
