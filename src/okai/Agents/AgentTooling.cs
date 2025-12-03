using System.ComponentModel;
using System.Text.Json;
using MediatR;
using Microsoft.Extensions.AI;
using okai;

namespace okai.Agents;

public class AgentTooling : IAgentTooling
{
    private readonly IMediator _mediator;
    private readonly IToolRequestFactory _factory;

    public AgentTooling(IMediator mediator, IToolRequestFactory factory)
    {
        _mediator = mediator;
        _factory = factory;
    }

    public IEnumerable<AITool> BuildTools() =>
        [
            AIFunctionFactory.Create(ListDirAsync),
            AIFunctionFactory.Create(ReadFileAsync),
            AIFunctionFactory.Create(WriteFileAsync),
            AIFunctionFactory.Create(RunShellAsync),
            AIFunctionFactory.Create(WebSearchAsync)
        ];

    [Description("List directory entries relative to the configured root.")]
    public async Task<string> ListDirAsync([Description("Path to list; defaults to root when omitted.")] string? path = null, CancellationToken cancellationToken = default)
    {
        var args = new Dictionary<string, string> { { "path", path ?? string.Empty } };
        return await RunToolAsync("list_dir", args, cancellationToken);
    }

    [Description("Read a text file relative to the configured root.")]
    public async Task<string> ReadFileAsync([Description("Path to the file to read.")] string path, CancellationToken cancellationToken = default)
    {
        var args = new Dictionary<string, string> { { "path", path } };
        return await RunToolAsync("read_file", args, cancellationToken);
    }

    [Description("Write text content to a file relative to the configured root.")]
    public async Task<string> WriteFileAsync(
        [Description("Path to the file to write.")] string path,
        [Description("Content to write.")] string content,
        CancellationToken cancellationToken = default)
    {
        var args = new Dictionary<string, string>
        {
            { "path", path },
            { "content", content }
        };
        return await RunToolAsync("write_file", args, cancellationToken);
    }

    [Description("Run a shell command within the configured root.")]
    public async Task<string> RunShellAsync(
        [Description("Command line to execute.")] string command,
        CancellationToken cancellationToken = default)
    {
        var args = new Dictionary<string, string> { { "command", command } };
        return await RunToolAsync("run_shell", args, cancellationToken);
    }

    [Description("Perform a web search using the configured search provider.")]
    public async Task<string> WebSearchAsync(
        [Description("Query string to search.")] string query,
        CancellationToken cancellationToken = default)
    {
        var args = new Dictionary<string, string> { { "query", query } };
        return await RunToolAsync("web_search", args, cancellationToken);
    }

    private async Task<string> RunToolAsync(string name, Dictionary<string, string> args, CancellationToken cancellationToken)
    {
        var raw = JsonSerializer.Serialize(args);
        var request = _factory.Create(name, args, raw);
        var result = await _mediator.Send(request, cancellationToken);
        return result.PayloadForModel;
    }
}
