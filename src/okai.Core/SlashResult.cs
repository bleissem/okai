using OpenAI.Chat;

namespace okai;

public record SlashResult(bool Handled, bool ExitRequested, string Model, ChatClient ChatClient)
{
    public static SlashResult CreateHandled(string model, ChatClient chatClient) => new(true, false, model, chatClient);
    public static SlashResult CreateExit(string model, ChatClient chatClient) => new(true, true, model, chatClient);
    public static SlashResult CreateNotHandled(string model, ChatClient chatClient) => new(false, false, model, chatClient);
}
