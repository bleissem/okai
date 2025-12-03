namespace okai;

public static class PaletteCatalog
{
    private static readonly ConsolePalette Default = new(
        AssistantPrefix: "bold yellow",
        UserPrefix: "bold green",
        Tool: "blue",
        Warning: "yellow",
        Error: "red",
        Trace: "dim",
        HeaderTitle: "bold cyan",
        HeaderLabel: "gray",
        HeaderValue: "white",
        StatusLabel: "gray",
        StatusValue: "white",
        ShellIdle: "gray");

    private static readonly ConsolePalette HighContrast = new(
        AssistantPrefix: "bold blue",
        UserPrefix: "bold green",
        Tool: "bold yellow",
        Warning: "bold yellow",
        Error: "bold red",
        Trace: "gray",
        HeaderTitle: "bold white",
        HeaderLabel: "white",
        HeaderValue: "aqua",
        StatusLabel: "white",
        StatusValue: "aqua",
        ShellIdle: "gray");

    private static readonly ConsolePalette Solarized = new(
        AssistantPrefix: "rgb(181,137,0)",
        UserPrefix: "rgb(38,139,210)",
        Tool: "rgb(42,161,152)",
        Warning: "rgb(203,75,22)",
        Error: "rgb(220,50,47)",
        Trace: "rgb(147,161,161)",
        HeaderTitle: "rgb(108,113,196)",
        HeaderLabel: "rgb(88,110,117)",
        HeaderValue: "rgb(131,148,150)",
        StatusLabel: "rgb(88,110,117)",
        StatusValue: "rgb(131,148,150)",
        ShellIdle: "rgb(147,161,161)");

    private static readonly ConsolePalette VsDark = new(
        AssistantPrefix: "rgb(255,203,107)", // orange-ish
        UserPrefix: "rgb(86,199,255)",       // cyan
        Tool: "rgb(141,200,255)",
        Warning: "rgb(255,199,109)",
        Error: "rgb(255,140,140)",
        Trace: "rgb(145,145,145)",
        HeaderTitle: "rgb(127,196,255)",
        HeaderLabel: "rgb(150,150,150)",
        HeaderValue: "rgb(220,220,220)",
        StatusLabel: "rgb(150,150,150)",
        StatusValue: "rgb(220,220,220)",
        ShellIdle: "rgb(145,145,145)");

    private static readonly ConsolePalette VsLight = new(
        AssistantPrefix: "rgb(218,112,44)",  // orange
        UserPrefix: "rgb(0,102,204)",        // blue
        Tool: "rgb(0,153,204)",
        Warning: "rgb(204,102,0)",
        Error: "rgb(204,0,0)",
        Trace: "rgb(110,110,110)",
        HeaderTitle: "rgb(0,122,204)",
        HeaderLabel: "rgb(90,90,90)",
        HeaderValue: "rgb(40,40,40)",
        StatusLabel: "rgb(90,90,90)",
        StatusValue: "rgb(40,40,40)",
        ShellIdle: "rgb(110,110,110)");

    private static readonly ConsolePalette VsHighContrast = new(
        AssistantPrefix: "bold white on rgb(0,0,0)",
        UserPrefix: "bold black on rgb(255,255,0)",
        Tool: "bold white on rgb(0,120,215)",
        Warning: "bold black on rgb(255,255,0)",
        Error: "bold white on rgb(255,0,0)",
        Trace: "bold white on rgb(0,0,0)",
        HeaderTitle: "bold white on rgb(0,0,0)",
        HeaderLabel: "bold white on rgb(0,0,0)",
        HeaderValue: "bold yellow on rgb(0,0,0)",
        StatusLabel: "bold white on rgb(0,0,0)",
        StatusValue: "bold yellow on rgb(0,0,0)",
        ShellIdle: "bold white on rgb(0,0,0)");

    private static readonly ConsolePalette TomorrowNightBlue = new(
        AssistantPrefix: "rgb(153,214,255)",
        UserPrefix: "rgb(209,241,169)",
        Tool: "rgb(153,221,255)",
        Warning: "rgb(255,197,143)",
        Error: "rgb(255,157,164)",
        Trace: "rgb(114,133,183)",
        HeaderTitle: "rgb(153,214,255)",
        HeaderLabel: "rgb(114,133,183)",
        HeaderValue: "rgb(255,255,255)",
        StatusLabel: "rgb(114,133,183)",
        StatusValue: "rgb(209,241,169)",
        ShellIdle: "rgb(114,133,183)");

    public static ConsolePalette Resolve(string name, IReadOnlyDictionary<string, ConsolePalette>? custom = null)
    {
        if (custom is not null && custom.TryGetValue(name, out var userPalette))
        {
            return userPalette;
        }

        return name.ToLowerInvariant() switch
        {
            "highcontrast" or "contrast" => HighContrast,
            "solarized" => Solarized,
            "tomorrownightblue" or "tnb" => TomorrowNightBlue,
            "vsdark" or "dark+" or "darkplus" => VsDark,
            "vslight" or "light+" or "lightplus" => VsLight,
            "vshighcontrast" or "vshc" or "hc" => VsHighContrast,
            _ => Default
        };
    }
}
