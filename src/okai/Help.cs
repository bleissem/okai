namespace okai;

public static class Help
{
    private const string HelpFlag = "--help";
    private const string ShortHelpFlag = "-h";

    public static bool ShouldShowHelp(string[] args) =>
        args.Any(arg => string.Equals(arg, HelpFlag, StringComparison.OrdinalIgnoreCase)
                        || string.Equals(arg, ShortHelpFlag, StringComparison.OrdinalIgnoreCase));

    public static string Build() =>
        string.Join(Environment.NewLine, new[]
        {
            "okai - CLI-Agent für KI-Projekte",
            string.Empty,
            "Usage:",
            "  dotnet run --project src/okai/okai.csproj [options]",
            string.Empty,
            "Options:",
            "  -h|--help    Show this help",
            string.Empty,
            "Environment:",
            "  AZURE_AI_PROJECT_ENDPOINT  Required (Foundry project endpoint)",
            "  AZURE_AI_MODEL             Optional (default: gpt-4o-mini)",
            "  AZURE_AI_ROOT              Optional (default: current directory)",
            "  OKAI_THEME                 Optional (e.g., default, vsdark, solarized)",
            "  OKAI_SEARCH_KEY            Optional (required for web_search tool)",
        });
}
