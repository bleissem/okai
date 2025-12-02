using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace okai;

public class OkaiConfig
{
    public List<Profile> Profiles { get; set; } = new();
}

public class Profile
{
    public string Name { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string? Model { get; set; }
    public string? Root { get; set; }
    public string? HistoryPath { get; set; }
    public bool? ApprovalsEnabled { get; set; }
    public string? ApprovalsPolicyPath { get; set; }
    public string? Theme { get; set; }
    public string? ThemesPath { get; set; }
    public string? SearchEndpoint { get; set; }
    public string? SearchKey { get; set; }
    public string? Shell { get; set; }
}

public static class ConfigLoader
{
    public static OkaiConfig Load(string path, ILogger? logger = null)
    {
        try
        {
            if (!File.Exists(path))
            {
                return new OkaiConfig();
            }

            var json = File.ReadAllText(path);
            var cfg = JsonSerializer.Deserialize<OkaiConfig>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var profiles = cfg?.Profiles ?? new List<Profile>();
            return new OkaiConfig { Profiles = profiles };
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "failed to load config from {ConfigPath}", path);
            return new OkaiConfig();
        }
    }
}
