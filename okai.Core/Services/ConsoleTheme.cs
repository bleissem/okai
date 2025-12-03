using System.IO;
using OpenAI.Chat;
using Spectre.Console;

namespace okai;

public class ConsoleTheme : IConsoleTheme
{
    private const int DefaultWindowHeight = 40;
    private ConsolePalette _palette;
    private readonly List<string> _logs = new();
    private readonly object _lock = new();
    private string _statusModel = string.Empty;
    private string _statusRoot = string.Empty;
    private string? _statusCommand;
    private int? _statusExitCode;
    private List<string> _headerLines = new();

    public ConsoleTheme(ConsolePalette palette)
    {
        _palette = palette;
    }

    public void PrintHeader(string endpoint, string model, string root)
    {
        _headerLines = new List<string>
        {
            $"[{_palette.HeaderTitle}]okai[/]  [{_palette.HeaderLabel}]endpoint[/]: [{_palette.HeaderValue}]{endpoint}[/]",
            $"[{_palette.HeaderLabel}]model[/]: [{_palette.HeaderValue}]{model}[/]  [{_palette.HeaderLabel}]root[/]: [{_palette.HeaderValue}]{root}[/]",
            "[gray]slash commands: /help, /exit, /model <name>[/]"
        };
        _statusModel = model;
        _statusRoot = root;
        Render(null);
    }

    public void PrintStatus(string message) =>
        AddLog($"[{_palette.Trace}]{Escape(message)}[/]");

    public void PrintAssistantPrefix() =>
        AddLog($"[{_palette.AssistantPrefix}]assistant>[/] ");

    public void PrintNewLine() =>
        AddLog(string.Empty);

    public void PrintToolLog(string tool, string log) =>
        AddLog($"[{_palette.Tool}]tool[{tool}][/]: {Escape(log)}");

    public void PrintWarning(string message) =>
        AddLog($"[{_palette.Warning}]warning[/]: {Escape(message)}");

    public void PrintError(string message) =>
        AddLog($"[{_palette.Error}]error[/]: {Escape(message)}");

    public void RenderStatusBar(string model, string root)
    {
        UpdateStatus(model, root, null, null);
    }

    public void RenderStatusBar(string model, string root, string lastCommand)
    {
        UpdateStatus(model, root, lastCommand, null);
    }

    public void RenderStatusBar(string model, string root, string lastCommand, int? lastExitCode)
    {
        UpdateStatus(model, root, lastCommand, lastExitCode);
    }

    public void PrintTrace(string label, string message) =>
        AddLog($"[{_palette.Trace}]{Escape(label)}[/]: {Escape(message)}");

    public async Task<T> WithSpinnerAsync<T>(string message, Func<Task<T>> action)
    {
        var result = await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .StartAsync(message, async _ => await action());
        Render(null);
        return result;
    }

    public async Task<string?> ReadInputAsync()
    {
        var prompt = $"[{_palette.UserPrefix}]user>[/] ";
        Render(prompt);
        return await Task.Run(() => Console.ReadLine());
    }

    public void AppendUserMessage(string message)
    {
        AddLog($"[{_palette.UserPrefix}]user>[/] {Escape(message)}");
    }

    private void AddLog(string line)
    {
        lock (_lock)
        {
            _logs.Add(line);
        }
        Render(null);
    }

    private void UpdateStatus(string model, string root, string? lastCommand, int? lastExitCode)
    {
        _statusModel = model;
        _statusRoot = root;
        _statusCommand = lastCommand;
        _statusExitCode = lastExitCode;
        Render(null);
    }

    public void ApplyPalette(ConsolePalette palette)
    {
        _palette = palette;
        Render(null);
    }

    private void Render(string? prompt)
    {
        lock (_lock)
        {
            var windowHeight = SafeWindowHeight(() => Console.WindowHeight);
            var maxLog = Math.Max(5, windowHeight - 6);
            if (_logs.Count > maxLog)
            {
                _logs.RemoveRange(0, _logs.Count - maxLog);
            }

            AnsiConsole.Clear();
            foreach (var line in _headerLines)
            {
                AnsiConsole.MarkupLine(line);
            }
            AnsiConsole.WriteLine();
            foreach (var line in _logs)
            {
                AnsiConsole.MarkupLine(line);
            }

            var shell = _statusCommand is null
                ? $"[{_palette.ShellIdle}]shell[/]: idle"
                : $"[{_palette.StatusLabel}]shell[/]: [{_palette.StatusValue}]{Escape(_statusCommand)}[/]{(_statusExitCode.HasValue ? $" [{_palette.StatusLabel}](exit {_statusExitCode})[/]" : string.Empty)}";

            var bar = new Columns(
                new Text($"[{_palette.StatusLabel}]model[/]: [{_palette.StatusValue}]{_statusModel}[/]"),
                new Text($"[{_palette.StatusLabel}]root[/]: [{_palette.StatusValue}]{_statusRoot}[/]"),
                new Text(shell)
            ).Collapse();

            AnsiConsole.Write(bar);
            AnsiConsole.WriteLine();

            if (prompt is not null)
            {
                AnsiConsole.Markup(prompt);
            }
        }
    }

    private static string Escape(string input) => input.Replace("[", "[[").Replace("]", "]]");

    internal static int SafeWindowHeight(Func<int> windowHeightProvider)
    {
        try
        {
            return windowHeightProvider();
        }
        catch (IOException)
        {
            return DefaultWindowHeight;
        }
        catch (PlatformNotSupportedException)
        {
            return DefaultWindowHeight;
        }
    }
}
