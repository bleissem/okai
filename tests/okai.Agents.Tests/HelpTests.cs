using System;
using okai;
using Xunit;

public class HelpTests
{
    [Theory]
    [InlineData("--help")]
    [InlineData("-h")]
    public void ShouldShowHelp_DetectsFlags(string flag)
    {
        Assert.True(Help.ShouldShowHelp(new[] { flag }));
    }

    [Fact]
    public void ShouldShowHelp_DetectsFlagAmongOtherArgs()
    {
        Assert.True(Help.ShouldShowHelp(new[] { "foo", "--help" }));
    }

    [Fact]
    public void ShouldShowHelp_IgnoresWhenNoFlags()
    {
        Assert.False(Help.ShouldShowHelp(Array.Empty<string>()));
    }

    [Fact]
    public void Build_IncludesKeySections()
    {
        var text = Help.Build();

        Assert.Contains("okai - CLI-Agent", text, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("AZURE_AI_PROJECT_ENDPOINT", text, StringComparison.Ordinal);
        Assert.Contains("dotnet run --project", text, StringComparison.OrdinalIgnoreCase);
    }
}
