using OpenAI.Chat;

namespace okai;

public static class ToolBuilder
{
    public static List<ChatTool> BuildTools()
    {
        return new List<ChatTool>
        {
            BuildFunction(
                "list_dir",
                "List files and directories under a path relative to the root.",
                """
                { "type": "object", "properties": { "path": { "type": "string", "description": "Relative path to list" } }, "required": ["path"] }
                """),
            BuildFunction(
                "read_file",
                "Read a UTF-8 text file relative to the root.",
                """
                { "type": "object", "properties": { "path": { "type": "string", "description": "Relative file path" } }, "required": ["path"] }
                """),
            BuildFunction(
                "write_file",
                "Write UTF-8 text content to a file relative to the root (overwrites existing content).",
                """
                { "type": "object", "properties": { "path": { "type": "string", "description": "Relative file path" }, "content": { "type": "string", "description": "Full file content to write" } }, "required": ["path", "content"] }
                """),
            BuildFunction(
                "run_shell",
                "Run a shell command inside the configured root directory.",
                """
                { "type": "object", "properties": { "command": { "type": "string", "description": "Shell command to execute" } }, "required": ["command"] }
                """),
            BuildFunction(
                "web_search",
                "Search the web for information (Bing Web Search API).",
                """
                { "type": "object", "properties": { "query": { "type": "string", "description": "Search query" } }, "required": ["query"] }
                """)
        };
    }

    private static ChatTool BuildFunction(string name, string description, string jsonSchema)
    {
        return ChatTool.CreateFunctionTool(name, description, BinaryData.FromString(jsonSchema), functionSchemaIsStrict: true);
    }
}
