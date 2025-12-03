using OpenAI.Chat;

namespace okai;

public interface IChatService
{
    (string? Command, int? ExitCode) LastShellCommand { get; }
    Task RunAgentTurnAsync(ChatClient chatClient, ChatCompletionOptions options, List<ChatMessage> messages);
}
