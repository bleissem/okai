using okai;
using Xunit;

public class ThemeResolverTests
{
    [Fact]
    public void ResolvesCustomPalette()
    {
        var palette = new ConsolePalette("a", "u", "t", "w", "e", "tr", "ht", "hl", "hv", "sl", "sv", "si");
        var resolver = new ThemeResolver(new Dictionary<string, ConsolePalette>(StringComparer.OrdinalIgnoreCase)
        {
            { "mytheme", palette }
        });

        var resolved = resolver.Resolve("mytheme");

        Assert.Equal(palette, resolved);
        Assert.Contains("mytheme", resolver.Names);
    }

    [Fact]
    public void ResolvesBuiltin_WhenUnknownCustom()
    {
        var resolver = new ThemeResolver(new Dictionary<string, ConsolePalette>());

        var resolved = resolver.Resolve("vsdark");

        Assert.NotNull(resolved);
    }
}
