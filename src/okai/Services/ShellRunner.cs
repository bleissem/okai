using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace okai;

public record ShellResult(int ExitCode, string Stdout, string Stderr);

public class ShellRunner : IShellRunner
{
    private const int DefaultTimeoutMs = 30_000;
    private const int MaxOutputBytes = 100_000;
    private readonly AppOptions _options;
    private readonly ILogger<ShellRunner> _logger;

    public ShellRunner(AppOptions options, ILogger<ShellRunner> logger)
    {
        _options = options;
        _logger = logger;
    }

    public async Task<ShellResult> RunAsync(string command, string workingDirectory, CancellationToken cancellationToken)
    {
        var shell = GetShell();
        var psi = new ProcessStartInfo
        {
            FileName = shell.FileName,
            Arguments = shell.Arguments(command),
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = false,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = psi };
        var stdout = new LimitedBuffer(MaxOutputBytes);
        var stderr = new LimitedBuffer(MaxOutputBytes);

        process.OutputDataReceived += (_, e) => { if (e.Data is not null) stdout.AppendLine(e.Data); };
        process.ErrorDataReceived += (_, e) => { if (e.Data is not null) stderr.AppendLine(e.Data); };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        var timeoutTask = Task.Delay(DefaultTimeoutMs, cancellationToken);
        var waitTask = process.WaitForExitAsync(cancellationToken);
        var completed = await Task.WhenAny(waitTask, timeoutTask);
        if (completed == timeoutTask)
        {
            try
            {
                process.Kill(true);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "failed to kill timed-out process");
            }
            _logger.LogWarning("shell command timed out");
            return new ShellResult(-1, stdout.ToString(), "timeout");
        }

        return new ShellResult(process.ExitCode, stdout.ToString(), stderr.ToString());
    }

    private (string FileName, Func<string, string> Arguments) GetShell()
    {
        var shell = _options.Shell.ToLowerInvariant();
        if (OperatingSystem.IsWindows())
        {
            if (shell == "powershell" || shell == "pwsh")
            {
                return ("powershell.exe", cmd => $"-NoLogo -NoProfile -Command \"{cmd}\"");
            }

            return ("cmd.exe", cmd => $"/C {cmd}");
        }

        if (shell == "powershell" || shell == "pwsh")
        {
            return ("pwsh", cmd => $"-NoLogo -NoProfile -Command \"{cmd}\"");
        }

        return ("/bin/sh", cmd => $"-c \"{cmd}\"");
    }
}
