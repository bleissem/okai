using Azure.AI.Projects;
using MediatR;
using OpenAI.Chat;

namespace okai;

public class ChatLoop
{
    private readonly AppOptions _options;
    private readonly IHistoryStore _historyStore;
    private readonly IChatService _chatService;
    private readonly ISlashCommandHandler _slashHandler;
    private readonly IConsoleTheme _console;

    public ChatLoop(AppOptions options, IHistoryStore historyStore, IChatService chatService, ISlashCommandHandler slashHandler, IConsoleTheme console)
    {
        _options = options;
        _historyStore = historyStore;
        _chatService = chatService;
        _slashHandler = slashHandler;
        _console = console;
    }

    public async Task RunAsync(AIProjectClient projectClient, ChatClient chatClient)
    {
        var messages = _historyStore.Load();
        _console.PrintHeader(_options.Endpoint, _options.Model, _options.Root);
        _console.RenderStatusBar(_options.Model, _options.Root);
        if (string.IsNullOrWhiteSpace(_options.SearchKey))
        {
            _console.PrintWarning("web_search is disabled (set OKAI_SEARCH_KEY to enable)");
        }

        var tools = ToolBuilder.BuildTools();
        var completionOptions = new ChatCompletionOptions
        {
            ToolChoice = ChatToolChoice.CreateAutoChoice(),
            AllowParallelToolCalls = true
        };
        foreach (var tool in tools)
        {
            completionOptions.Tools.Add(tool);
        }

        while (true)
        {
            var input = await _console.ReadInputAsync();
            if (input is null)
            {
                break;
            }

            var slash = await _slashHandler.HandleAsync(input, projectClient, chatClient, _options, messages);
            if (slash.Handled)
            {
                chatClient = slash.ChatClient;
                _console.RenderStatusBar(slash.Model, _options.Root);
                _historyStore.Save(messages);
                if (slash.ExitRequested)
                {
                    return;
                }
                continue;
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            _console.AppendUserMessage(input);
            messages.Add(new UserChatMessage(input));
            await _chatService.RunAgentTurnAsync(chatClient, completionOptions, messages);
            _historyStore.Save(messages);
            var lastShell = _chatService.LastShellCommand;
            if (lastShell.Command is null)
            {
                _console.RenderStatusBar(_options.Model, _options.Root);
            }
            else if (lastShell.ExitCode.HasValue)
            {
                _console.RenderStatusBar(_options.Model, _options.Root, lastShell.Command, lastShell.ExitCode);
            }
            else
            {
                _console.RenderStatusBar(_options.Model, _options.Root, lastShell.Command);
            }
        }
    }
}
