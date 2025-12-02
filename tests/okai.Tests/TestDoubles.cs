using Microsoft.Extensions.Logging.Abstractions;
using okai;

internal sealed class NullConsoleTheme : IConsoleTheme
{
    public void PrintHeader(string endpoint, string model, string root) { }
    public void PrintStatus(string message) { }
    public void PrintAssistantPrefix() { }
    public Task<string?> ReadInputAsync() => Task.FromResult<string?>(string.Empty);
    public void AppendUserMessage(string message) { }
    public void PrintNewLine() { }
    public void PrintToolLog(string tool, string log) { }
    public void PrintWarning(string message) { }
    public void PrintError(string message) { }
    public void PrintTrace(string label, string message) { }
    public void RenderStatusBar(string model, string root) { }
    public void RenderStatusBar(string model, string root, string lastCommand) { }
    public void RenderStatusBar(string model, string root, string lastCommand, int? lastExitCode) { }
    public Task<T> WithSpinnerAsync<T>(string message, Func<Task<T>> action) => action();
    public void ApplyPalette(ConsolePalette palette) { }
}

internal sealed class FixedInput : IUserInput
{
    private readonly Queue<string?> _responses;

    public FixedInput(params string?[] responses)
    {
        _responses = new Queue<string?>(responses);
    }

    public string? ReadLine()
    {
        if (_responses.Count == 0)
        {
            return null;
        }

        return _responses.Dequeue();
    }
}
