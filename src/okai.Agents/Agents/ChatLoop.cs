using Azure.AI.Projects;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using okai;

namespace okai.Agents;

public class ChatLoop : IChatLoop
{
    private readonly AppOptions _options;
    private readonly IConsoleTheme _console;
    private readonly IAgentFactory _agentFactory;
    private readonly AIProjectClient _projectClient;

    public ChatLoop(AppOptions options, IConsoleTheme console, IAgentFactory agentFactory, AIProjectClient projectClient)
    {
        _options = options;
        _console = console;
        _agentFactory = agentFactory;
        _projectClient = projectClient;
    }

    public async Task RunAsync()
    {
        _console.PrintHeader(_options.Endpoint, _options.Model, _options.Root);
        _console.RenderStatusBar(_options.Model, _options.Root);
        if (string.IsNullOrWhiteSpace(_options.SearchKey))
        {
            _console.PrintWarning("web_search is disabled (set OKAI_SEARCH_KEY to enable)");
        }

        var chatClient = _projectClient.OpenAI.GetChatClient(_options.Model).AsIChatClient();
        var agent = _agentFactory.CreateAgent(chatClient, _options.Model);
        var thread = agent.GetNewThread();

        while (true)
        {
            var input = await _console.ReadInputAsync();
            if (input is null)
            {
                break;
            }

            if (TryHandleCommand(input, ref agent, ref thread))
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            _console.AppendUserMessage(input);
            var response = await agent.RunAsync(input, thread);
            var message = response.ToString();
            _console.PrintAssistantPrefix();
            _console.PrintTrace("assistant", message);
            _console.PrintNewLine();
            _console.RenderStatusBar(_options.Model, _options.Root);
        }
    }

    private bool TryHandleCommand(string input, ref AIAgent agent, ref AgentThread thread)
    {
        if (input.Equals("/exit", StringComparison.OrdinalIgnoreCase))
        {
            Environment.Exit(0);
        }

        if (input.StartsWith("/model", StringComparison.OrdinalIgnoreCase))
        {
            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length >= 2)
            {
                var newModel = parts[1];
                var chatClient = _projectClient.OpenAI.GetChatClient(newModel).AsIChatClient();
                agent = _agentFactory.CreateAgent(chatClient, newModel);
                thread = agent.GetNewThread();
                _console.RenderStatusBar(newModel, _options.Root);
            }
            return true;
        }

        if (input.Equals("/help", StringComparison.OrdinalIgnoreCase))
        {
            _console.PrintStatus("Commands: /exit, /model <name>, /help");
            return true;
        }

        return false;
    }
}
