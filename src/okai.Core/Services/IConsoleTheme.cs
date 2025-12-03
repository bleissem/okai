using OpenAI.Chat;

namespace okai;

public interface IConsoleTheme
{
    void PrintHeader(string endpoint, string model, string root);
    void PrintStatus(string message);
    void PrintAssistantPrefix();
    Task<string?> ReadInputAsync();
    void AppendUserMessage(string message);
    void PrintNewLine();
    void PrintToolLog(string tool, string log);
    void PrintWarning(string message);
    void PrintError(string message);
    void PrintTrace(string label, string message);
    Task<T> WithSpinnerAsync<T>(string message, Func<Task<T>> action);
    void RenderStatusBar(string model, string root);
    void RenderStatusBar(string model, string root, string lastCommand);
    void RenderStatusBar(string model, string root, string lastCommand, int? lastExitCode);
    void ApplyPalette(ConsolePalette palette);
}
