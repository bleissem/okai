using OpenAI.Chat;

namespace okai;

public interface IHistoryStore
{
    List<ChatMessage> Load();
    void Save(List<ChatMessage> messages);
}
