namespace okai;

public interface IShellPolicy
{
    bool IsAllowed(string command);
}

public class ShellPolicy : IShellPolicy
{
    private readonly HashSet<string> _allowed;

    public ShellPolicy()
    {
        var env = Environment.GetEnvironmentVariable("OKAI_SHELL_ALLOW");
        if (!string.IsNullOrWhiteSpace(env))
        {
            _allowed = new HashSet<string>(env.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries), StringComparer.OrdinalIgnoreCase);
        }
        else
        {
            _allowed = new HashSet<string>(new[]
            {
                "ls", "dir", "pwd", "echo", "git", "rg", "cat", "type"
            }, StringComparer.OrdinalIgnoreCase);
        }
    }

    public bool IsAllowed(string command)
    {
        var first = GetFirstToken(command);
        if (string.IsNullOrWhiteSpace(first))
        {
            return false;
        }

        // Allow git subcommands broadly if "git" is whitelisted.
        if (_allowed.Contains(first))
        {
            return true;
        }

        return false;
    }

    private static string GetFirstToken(string command)
    {
        var parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts[0] : string.Empty;
    }
}
