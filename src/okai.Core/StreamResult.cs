using OpenAI.Chat;

namespace okai;

public record StreamResult(string Text, IReadOnlyList<ChatToolCall> ToolCalls);
