using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using okai;

namespace okai.Agents;

public class AgentFactory : IAgentFactory
{
    private readonly AppOptions _options;
    private readonly IAgentTooling _tooling;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IServiceProvider _serviceProvider;

    public AgentFactory(AppOptions options, IAgentTooling tooling, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
    {
        _options = options;
        _tooling = tooling;
        _loggerFactory = loggerFactory;
        _serviceProvider = serviceProvider;
    }

    public AIAgent CreateAgent(IChatClient chatClient, string model)
    {
        var tools = _tooling.BuildTools().ToArray();
        var name = $"okai-cli-{Guid.NewGuid():N}";
        var instructions = $"You are an interactive CLI agent. Use the provided function tools to inspect and modify files under the configured root ({_options.Root}), run shell commands responsibly, and keep outputs concise.";
        var agentOptions = new ChatClientAgentOptions
        {
            Id = name,
            Name = "okai",
            Instructions = instructions,
            Description = $"CLI agent targeting model '{model}' with file and shell tools.",
            ChatOptions = new ChatOptions { Tools = tools.ToList() }
        };

        return chatClient.CreateAIAgent(agentOptions, _loggerFactory, _serviceProvider);
    }
}
