using Microsoft.Extensions.AI;

namespace okai.Agents;

public interface IAgentTooling
{
    IEnumerable<AITool> BuildTools();
}
