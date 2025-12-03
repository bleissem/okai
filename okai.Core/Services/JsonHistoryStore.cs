using Microsoft.Extensions.Logging;
using System.Text.Json;
using OpenAI.Chat;

namespace okai;

public class JsonHistoryStore : IHistoryStore
{
    private readonly AppOptions _options;
    private readonly ILogger<JsonHistoryStore> _logger;

    public JsonHistoryStore(AppOptions options, ILogger<JsonHistoryStore> logger)
    {
        _options = options;
        _logger = logger;
    }

    public List<ChatMessage> Load()
    {
        var messages = new List<ChatMessage>();
        try
        {
            if (!File.Exists(_options.HistoryPath))
            {
                return messages;
            }

            var json = File.ReadAllText(_options.HistoryPath);
            var entries = JsonSerializer.Deserialize<List<HistoryEntry>>(json) ?? new List<HistoryEntry>();
            foreach (var entry in entries)
            {
                if (string.IsNullOrWhiteSpace(entry.Content))
                {
                    continue;
                }

                switch (entry.Role)
                {
                    case "user":
                        messages.Add(new UserChatMessage(entry.Content));
                        break;
                    case "assistant":
                        messages.Add(new AssistantChatMessage(entry.Content));
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "could not load history '{HistoryPath}'", _options.HistoryPath);
        }

        return messages;
    }

    public void Save(List<ChatMessage> messages)
    {
        try
        {
            var entries = new List<HistoryEntry>();
            foreach (var message in messages)
            {
                switch (message)
                {
                    case UserChatMessage user:
                        {
                            var content = ExtractText(user.Content);
                            if (!string.IsNullOrWhiteSpace(content))
                            {
                                entries.Add(new HistoryEntry("user", content));
                            }
                            break;
                        }
                    case AssistantChatMessage assistant:
                        {
                            var content = ExtractText(assistant.Content);
                            if (!string.IsNullOrWhiteSpace(content))
                            {
                                entries.Add(new HistoryEntry("assistant", content));
                            }
                            break;
                        }
                }
            }

            var json = JsonSerializer.Serialize(entries, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_options.HistoryPath, json);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "could not save history '{HistoryPath}'", _options.HistoryPath);
        }
    }

    private static string ExtractText(ChatMessageContent? content)
    {
        if (content is null)
        {
            return string.Empty;
        }

        var parts = content.Where(p => p.Kind == ChatMessageContentPartKind.Text).Select(p => p.Text);
        return string.Join(string.Empty, parts);
    }
}
