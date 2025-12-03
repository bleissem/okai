using Azure.AI.Projects;
using OpenAI.Chat;

namespace okai;

public class SlashCommandHandler : ISlashCommandHandler
{
    private readonly IConsoleTheme _console;
    private readonly IThemeResolver _resolver;
    private readonly AppOptions _options;

    public SlashCommandHandler(IConsoleTheme console, IThemeResolver resolver, AppOptions options)
    {
        _console = console;
        _resolver = resolver;
        _options = options;
    }

    public Task<SlashResult> HandleAsync(string input, AIProjectClient projectClient, ChatClient chatClient, AppOptions options, List<ChatMessage> messages)
    {
        if (!input.StartsWith("/"))
        {
            return Task.FromResult(SlashResult.CreateNotHandled(options.Model, chatClient));
        }

        if (input.Equals("/exit", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(SlashResult.CreateExit(options.Model, chatClient));
        }

        if (input.StartsWith("/theme", StringComparison.OrdinalIgnoreCase))
        {
            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length == 1 || parts[1].Equals("list", StringComparison.OrdinalIgnoreCase))
            {
                var names = string.Join(", ", _resolver.Names);
                _console.PrintStatus($"themes: {names}");
                return Task.FromResult(SlashResult.CreateHandled(options.Model, chatClient));
            }

            var name = parts[1];
            var palette = _resolver.Resolve(name);
            _console.ApplyPalette(palette);
            _console.PrintHeader(_options.Endpoint, options.Model, _options.Root);
            return Task.FromResult(SlashResult.CreateHandled(options.Model, chatClient));
        }

        return Task.FromResult(SlashResult.CreateNotHandled(options.Model, chatClient));
    }
}
