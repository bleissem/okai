using MediatR;
using okai.Requests;

namespace okai;

public class ToolRequestFactory : IToolRequestFactory
{
    public IRequest<ToolResult> Create(string name, Dictionary<string, string> args, string rawArgs)
    {
        return name switch
        {
            "list_dir" => new ListDirQuery(GetArgOrDefault(args, "path", string.Empty)),
            "read_file" => new ReadFileQuery(GetArg(args, "path")),
            "write_file" => new WriteFileCommand(GetArg(args, "path"), GetArg(args, "content")),
            "run_shell" => new RunShellCommand(GetArg(args, "command")),
            "web_search" => new WebSearchQuery(GetArg(args, "query")),
            _ => new UnknownToolQuery(name, rawArgs)
        };
    }

    private string GetArg(Dictionary<string, string> args, string key)
    {
        if (args.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        throw new InvalidOperationException($"missing '{key}'");
    }

    private string GetArgOrDefault(Dictionary<string, string> args, string key, string defaultValue)
    {
        if (args.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        return defaultValue;
    }
}
