using Azure.AI.Projects;
using OpenAI.Chat;

namespace okai;

public class SlashCommandHandler : ISlashCommandHandler
{
    private readonly IConsoleTheme _console;
    private readonly IThemeResolver _themes;
    private readonly AppOptions _options;

    public SlashCommandHandler(IConsoleTheme console, IThemeResolver themes, AppOptions options)
    {
        _console = console;
        _themes = themes;
        _options = options;
    }

    public async Task<SlashResult> HandleAsync(
        string input,
        AIProjectClient projectClient,
        ChatClient chatClient,
        AppOptions options,
        List<ChatMessage> messages)
    {
        var trimmed = input.Trim();
        if (!trimmed.StartsWith("/"))
        {
            return SlashResult.CreateNotHandled(options.Model, chatClient);
        }

        var parts = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var command = parts[0].ToLowerInvariant();

        switch (command)
        {
            case "/exit":
                return SlashResult.CreateExit(options.Model, chatClient);
            case "/help":
                _console.PrintStatus("commands: /help, /exit, /model <name>, /theme list, /theme <name>");
                return SlashResult.CreateHandled(options.Model, chatClient);
            case "/model" when parts.Length >= 2:
                {
                    var model = parts[1];
                    var newClient = projectClient.OpenAI.GetChatClient(model);
                    _console.PrintStatus($"switched model to {model}");
                    return SlashResult.CreateHandled(model, newClient);
                }
            case "/theme" when parts.Length == 2 && parts[1].Equals("list", StringComparison.OrdinalIgnoreCase):
                _console.PrintStatus($"themes: {string.Join(", ", _themes.Names)}");
                return SlashResult.CreateHandled(options.Model, chatClient);
            case "/theme" when parts.Length == 2:
                {
                    var name = parts[1];
                    var palette = _themes.Resolve(name);
                    _console.ApplyPalette(palette);
                    _console.PrintHeader(_options.Endpoint, options.Model, _options.Root);
                    _console.RenderStatusBar(options.Model, _options.Root);
                    _console.PrintStatus($"applied theme '{name}'");
                    return SlashResult.CreateHandled(options.Model, chatClient);
                }
            default:
                return SlashResult.CreateNotHandled(options.Model, chatClient);
        }
    }
}
