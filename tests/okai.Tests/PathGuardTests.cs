using System;
using System.IO;
using okai;
using Xunit;

public class PathGuardTests
{
    [Fact]
    public void RejectsPathsOutsideRoot_WithSharedPrefix()
    {
        var guard = new PathGuard();
        using var root = new TempFolder();
        using var sibling = new TempFolder();

        var relative = Path.GetRelativePath(root.Path, Path.Combine(sibling.Path, "file.txt"));

        Assert.Throws<InvalidOperationException>(() => guard.Resolve(root.Path, relative));
    }

    [Fact]
    public void RejectsWindowsStyleTraversal_OnUnix()
    {
        if (OperatingSystem.IsWindows())
        {
            return;
        }

        var guard = new PathGuard();
        using var root = new TempFolder();

        Assert.Throws<InvalidOperationException>(() => guard.Resolve(root.Path, "..\\secret.txt"));
    }

    [Fact]
    public void AllowsPathInsideRoot()
    {
        var guard = new PathGuard();
        using var root = new TempFolder();

        var resolved = guard.Resolve(root.Path, "file.txt");

        Assert.StartsWith(Path.GetFullPath(root.Path), resolved, StringComparison.OrdinalIgnoreCase);
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
            try { Directory.Delete(Path, recursive: true); } catch { /* ignore cleanup failures */ }
        }
    }
}
