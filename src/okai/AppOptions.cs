using Microsoft.Extensions.Logging;

namespace okai;

public record AppOptions(
    string Endpoint,
    string Model,
    string Root,
    string HistoryPath,
    bool ApprovalsEnabled,
    string ApprovalsPolicyPath,
    string Theme,
    string ThemePath,
    string SearchEndpoint,
    string SearchKey,
    string Shell,
    string ProfileName,
    string ConfigPath)
{
    public static AppOptions? FromEnvironment(ILogger logger)
    {
        var configPath = Environment.GetEnvironmentVariable("OKAI_CONFIG_PATH") ?? Path.Combine(Directory.GetCurrentDirectory(), "okai.config.json");
        var profileName = Environment.GetEnvironmentVariable("OKAI_PROFILE") ?? "default";
        var config = ConfigLoader.Load(configPath, logger);
        var profile = config.Profiles.FirstOrDefault(p => string.Equals(p.Name, profileName, StringComparison.OrdinalIgnoreCase));

        var endpoint = Environment.GetEnvironmentVariable("AZURE_AI_PROJECT_ENDPOINT") ?? profile?.Endpoint;
        if (string.IsNullOrWhiteSpace(endpoint))
        {
            PrintUsage(logger, configPath);
            return null;
        }

        var model = Environment.GetEnvironmentVariable("AZURE_AI_MODEL") ?? profile?.Model ?? "gpt-4o-mini";
        var root = Path.GetFullPath(Environment.GetEnvironmentVariable("AZURE_AI_ROOT") ?? profile?.Root ?? Directory.GetCurrentDirectory());
        var historyPath = Environment.GetEnvironmentVariable("AZURE_AI_HISTORY_PATH") ?? profile?.HistoryPath ?? Path.Combine(root, ".okai_history.json");
        var approvalsEnabled = GetBoolEnv("OKAI_APPROVALS") ?? profile?.ApprovalsEnabled ?? false;
        var approvalsPolicyPath = Environment.GetEnvironmentVariable("OKAI_APPROVALS_POLICY") ?? profile?.ApprovalsPolicyPath ?? Path.Combine(root, ".okai_approvals.json");
        var theme = Environment.GetEnvironmentVariable("OKAI_THEME") ?? profile?.Theme ?? "default";
        var themePath = Environment.GetEnvironmentVariable("OKAI_THEMES_PATH")
                         ?? profile?.ThemesPath
                         ?? GetDefaultThemePath();
        var searchEndpoint = Environment.GetEnvironmentVariable("OKAI_SEARCH_ENDPOINT") ?? profile?.SearchEndpoint ?? "https://api.bing.microsoft.com/v7.0/search";
        var searchKey = Environment.GetEnvironmentVariable("OKAI_SEARCH_KEY") ?? profile?.SearchKey ?? string.Empty;
        var shell = Environment.GetEnvironmentVariable("OKAI_SHELL") ?? profile?.Shell ?? "cmd";

        return new AppOptions(endpoint, model, root, historyPath, approvalsEnabled, approvalsPolicyPath, theme, themePath, searchEndpoint, searchKey, shell, profileName, configPath);
    }

    private static bool? GetBoolEnv(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);
        if (value is null) return null;
        return value.Equals("true", StringComparison.OrdinalIgnoreCase);
    }

    private static void PrintUsage(ILogger logger, string configPath)
    {
        logger.LogError("Required config:");
        logger.LogInformation("  AZURE_AI_PROJECT_ENDPOINT (env) or okai.config.json (profiles[].endpoint)");
        logger.LogInformation("Optional env vars:");
        logger.LogInformation("  AZURE_AI_MODEL (default: gpt-4o-mini)");
        logger.LogInformation("  AZURE_AI_ROOT (default: current directory)");
        logger.LogInformation("  AZURE_AI_HISTORY_PATH (default: <root>/.okai_history.json)");
        logger.LogInformation("  OKAI_APPROVALS (default: false)");
        logger.LogInformation("  OKAI_APPROVALS_POLICY (default: <root>/.okai_approvals.json)");
        logger.LogInformation("  OKAI_CONFIG_PATH (default: ./okai.config.json)");
        logger.LogInformation("  OKAI_PROFILE (default: default)");
        logger.LogInformation("  OKAI_THEME (default: default)");
        logger.LogInformation("  OKAI_THEMES_PATH (default: ~/okai.themes.json)");
        logger.LogInformation("  OKAI_SEARCH_ENDPOINT (default: https://api.bing.microsoft.com/v7.0/search)");
        logger.LogInformation("  OKAI_SEARCH_KEY (required for web_search tool)");
        logger.LogInformation("  OKAI_SHELL (default: cmd; options: cmd, powershell, sh)");
        logger.LogInformation($"Config file path: {configPath}");
    }

    private static string GetDefaultThemePath()
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        return Path.Combine(home, DefaultHomeDirName, DefaultThemesFileName);
    }

    private const string DefaultHomeDirName = ".okai";
    private const string DefaultThemesFileName = "okai.themes.json";
}
