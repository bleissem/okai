using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace okai;

public class ApprovalService : IApprovalService
{
    private readonly AppOptions _options;
    private readonly ILogger<ApprovalService> _logger;
    private readonly IConsoleTheme _console;
    private readonly IUserInput _input;
    private readonly HashSet<string> _cachedApprovals;

    public ApprovalService(AppOptions options, ILogger<ApprovalService> logger, IConsoleTheme console, IUserInput input)
    {
        _options = options;
        _logger = logger;
        _console = console;
        _input = input;
        _cachedApprovals = LoadPolicy(options.ApprovalsPolicyPath);
    }

    public bool Approve(string command)
    {
        if (!_options.ApprovalsEnabled)
        {
            _logger.LogTrace("approvals disabled; allowing '{Command}'", command);
            return true;
        }

        if (_cachedApprovals.Contains(command))
        {
            _logger.LogTrace("command pre-approved: '{Command}'", command);
            return true;
        }

        _console.PrintStatus($"approve shell command? \"{command}\" [y/N]");
        var input = _input.ReadLine();
        if (string.Equals(input, "y", StringComparison.OrdinalIgnoreCase))
        {
            _cachedApprovals.Add(command);
            PersistPolicy(_options.ApprovalsPolicyPath, _cachedApprovals);
            _logger.LogInformation("approved command: '{Command}'", command);
            return true;
        }

        _logger.LogWarning("command denied: '{Command}'", command);
        return false;
    }

    private HashSet<string> LoadPolicy(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }

            var json = File.ReadAllText(path);
            var list = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
            return new HashSet<string>(list, StringComparer.OrdinalIgnoreCase);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "failed to load approvals policy from {ApprovalsPolicyPath}", path);
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }
    }

    private void PersistPolicy(string path, HashSet<string> approvals)
    {
        try
        {
            var json = JsonSerializer.Serialize(approvals);
            File.WriteAllText(path, json);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "failed to persist approvals policy to {ApprovalsPolicyPath}", path);
        }
    }
}
