using okai;
using Xunit;

namespace okai.Tests;

public class ConfigLoaderTests
{
    [Fact]
    public void Load_ReturnsEmpty_WhenMissing()
    {
        using var temp = new TempFolder();
        var cfg = ConfigLoader.Load(Path.Combine(temp.Path, "none.json"));
        Assert.Empty(cfg.Profiles);
    }

    [Fact]
    public void Load_ParsesProfiles()
    {
        using var temp = new TempFolder();
        var configPath = Path.Combine(temp.Path, "okai.config.json");
        File.WriteAllText(configPath,
            """
            { "profiles": [ { "name": "default", "endpoint": "https://ex", "model": "gpt", "root": "/tmp" } ] }
            """);

        var cfg = ConfigLoader.Load(configPath);
        Assert.Single(cfg.Profiles);
        Assert.Equal("default", cfg.Profiles[0].Name);
        Assert.Equal("https://ex", cfg.Profiles[0].Endpoint);
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
