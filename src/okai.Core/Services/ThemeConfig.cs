using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace okai;

public record ThemeConfig(Dictionary<string, ThemeDefinition> Themes)
{
    public static ThemeConfig Empty => new(new Dictionary<string, ThemeDefinition>(StringComparer.OrdinalIgnoreCase));
}

public record ThemeDefinition(
    string AssistantPrefix,
    string UserPrefix,
    string Tool,
    string Warning,
    string Error,
    string Trace,
    string HeaderTitle,
    string HeaderLabel,
    string HeaderValue,
    string StatusLabel,
    string StatusValue,
    string ShellIdle);

public static class ThemeLoader
{
    public static ThemeConfig Load(string path, ILogger? logger = null)
    {
        try
        {
            if (!File.Exists(path))
            {
                return ThemeConfig.Empty;
            }

            var json = File.ReadAllText(path);
            var cfg = JsonSerializer.Deserialize<ThemeConfig>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (cfg?.Themes is null)
            {
                return ThemeConfig.Empty;
            }

            return new ThemeConfig(new Dictionary<string, ThemeDefinition>(cfg.Themes, StringComparer.OrdinalIgnoreCase));
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "failed to load theme config from {ThemePath}", path);
            return ThemeConfig.Empty;
        }
    }
}
