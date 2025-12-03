using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace okai.Agents;

public interface IAgentFactory
{
    AIAgent CreateAgent(IChatClient chatClient, string model);
}
