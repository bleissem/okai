using Microsoft.Extensions.Logging.Abstractions;
using okai;
using Xunit;

namespace okai.Tests;

public class ShellRunnerTests
{
    [Fact]
    public async Task EchoCommand_ReturnsExitZero_Cmd()
    {
        using var temp = new TempFolder();
        var opts = new AppOptions("https://example.invalid", "gpt-4o-mini", temp.Path, "hist", false, "policy", "default", "themes", "search", "key", "cmd", "test", "cfg");
        var runner = new ShellRunner(opts, NullLogger<ShellRunner>.Instance);
        var result = await runner.RunAsync("echo okai-shell-test", temp.Path, default);

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("okai-shell-test", result.Stdout);
    }

    [Fact]
    public async Task EchoCommand_ReturnsExitZero_PowerShell()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        using var temp = new TempFolder();
        var opts = new AppOptions("https://example.invalid", "gpt-4o-mini", temp.Path, "hist", false, "policy", "default", "themes", "search", "key", "powershell", "test", "cfg");
        var runner = new ShellRunner(opts, NullLogger<ShellRunner>.Instance);
        var result = await runner.RunAsync("Write-Output okai-shell-ps", temp.Path, default);

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("okai-shell-ps", result.Stdout);
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
