using Azure.AI.Projects;
using OpenAI.Chat;

namespace okai;

public interface ISlashCommandHandler
{
    Task<SlashResult> HandleAsync(string input, AIProjectClient projectClient, ChatClient chatClient, AppOptions options, List<ChatMessage> messages);
}
