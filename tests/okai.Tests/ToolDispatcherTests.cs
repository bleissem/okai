using okai;
using okai.Requests;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

public class ToolDispatcherTests
{
    [Fact]
    public async Task ListDir_ReturnsEntries()
    {
        using var temp = new TempFolder();
        var file = Path.Combine(temp.Path, "file.txt");
        File.WriteAllText(file, "hello");

        var mediator = BuildMediator(temp.Path);
        var result = await mediator.Send(new ListDirQuery("."));

        Assert.Contains("listed", result.Log);
        Assert.Contains("file.txt", result.PayloadForModel);
    }

    [Fact]
    public async Task WriteAndRead_File_RoundTrips()
    {
        using var temp = new TempFolder();
        var mediator = BuildMediator(temp.Path);

        var write = await mediator.Send(new WriteFileCommand("a.txt", "hi there"));
        Assert.Contains("wrote a.txt", write.Log);

        var read = await mediator.Send(new ReadFileQuery("a.txt"));
        Assert.Contains("hi there", read.PayloadForModel);
    }

    [Fact]
    public async Task OutsideRoot_IsRejected()
    {
        using var temp = new TempFolder();
        var mediator = BuildMediator(temp.Path);

        var result = await mediator.Send(new ReadFileQuery("..\\secret.txt"));

        Assert.Contains("outside of the allowed root", result.Log);
    }

    private static IMediator BuildMediator(string root)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IPathGuard, PathGuard>();
        services.AddSingleton<IToolContext>(new ToolContext(root));
        services.AddMediatR(typeof(ListDirHandler).Assembly);
        var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<IMediator>();
    }

    private sealed class TempFolder : IDisposable
    {
        public string Path { get; }

        public TempFolder()
        {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName());
            Directory.CreateDirectory(Path);
        }

        public void Dispose()
        {
            try { Directory.Delete(Path, recursive: true); } catch { /* ignore */ }
        }
    }
}
