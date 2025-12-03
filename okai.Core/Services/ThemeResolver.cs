namespace okai;

public interface IThemeResolver
{
    ConsolePalette Resolve(string name);
    IReadOnlyCollection<string> Names { get; }
}

public class ThemeResolver : IThemeResolver
{
    private static readonly string[] BuiltInNames = new[]
    {
        "default", "highcontrast", "solarized", "tomorrownightblue",
        "vsdark", "dark+", "darkplus",
        "vslight", "light+", "lightplus",
        "vshighcontrast", "vshc", "hc"
    };

    private readonly Dictionary<string, ConsolePalette> _custom;
    private readonly HashSet<string> _names;

    public ThemeResolver(Dictionary<string, ConsolePalette> customPalettes)
    {
        _custom = customPalettes;
        _names = new HashSet<string>(BuiltInNames, StringComparer.OrdinalIgnoreCase);
        foreach (var name in _custom.Keys)
        {
            _names.Add(name);
        }
    }

    public IReadOnlyCollection<string> Names => _names;

    public ConsolePalette Resolve(string name)
    {
        return PaletteCatalog.Resolve(name, _custom);
    }
}
