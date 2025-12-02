namespace okai;

public class ToolContext : IToolContext
{
    public string Root { get; }

    public ToolContext(string root)
    {
        Root = root;
    }
}
