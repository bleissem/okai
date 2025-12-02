using Microsoft.Extensions.Logging.Abstractions;
using okai;
using Xunit;

public class ApprovalServiceTests
{
    [Fact]
    public void ApprovalsDisabled_AllowsCommand()
    {
        var opts = new AppOptions("https://example.invalid", "gpt-4o-mini", Environment.CurrentDirectory, "hist", false, "policy.json", "default", "themes.json", "search", "key", "cmd", "test", "cfg");
        var service = new ApprovalService(opts, NullLogger<ApprovalService>.Instance, new NullConsoleTheme(), new FixedInput("y"));

        Assert.True(service.Approve("echo ok"));
    }

    [Fact]
    public void ApprovalsEnabled_UsesPolicyFile()
    {
        using var temp = new TempFolder();
        var policyPath = Path.Combine(temp.Path, "policy.json");
        File.WriteAllText(policyPath, """["echo preapproved"]""");

        var opts = new AppOptions("https://example.invalid", "gpt-4o-mini", Environment.CurrentDirectory, "hist", true, policyPath, "default", "themes.json", "search", "key", "cmd", "test", "cfg");
        var service = new ApprovalService(opts, NullLogger<ApprovalService>.Instance, new NullConsoleTheme(), new FixedInput("n"));

        Assert.True(service.Approve("echo preapproved"));
        Assert.False(service.Approve("echo needs-approval"));
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
