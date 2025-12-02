using System.IO;
using okai;
using Xunit;

public class ConsoleThemeTests
{
    [Fact]
    public void SafeWindowHeight_UsesProviderValue()
    {
        var height = ConsoleTheme.SafeWindowHeight(() => 25);

        Assert.Equal(25, height);
    }

    [Fact]
    public void SafeWindowHeight_FallsBackOnException()
    {
        var height = ConsoleTheme.SafeWindowHeight(() => throw new IOException("no console"));

        Assert.Equal(40, height);
    }
}
