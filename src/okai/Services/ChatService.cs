using System.Text;
using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;

namespace okai;

public class ChatService : IChatService
{
    private readonly IMediator _mediator;
    private readonly IToolRequestFactory _toolFactory;
    private readonly IConsoleTheme _console;
    private readonly ILogger<ChatService> _logger;
    public (string? Command, int? ExitCode) LastShellCommand { get; private set; } = (null, null);

    public ChatService(IMediator mediator, IToolRequestFactory toolFactory, IConsoleTheme console, ILogger<ChatService> logger)
    {
        _mediator = mediator;
        _toolFactory = toolFactory;
        _console = console;
        _logger = logger;
    }

    public async Task RunAgentTurnAsync(ChatClient chatClient, ChatCompletionOptions options, List<ChatMessage> messages)
    {
        while (true)
        {
            var streamResult = await StreamChatAsync(chatClient, messages, options);

            if (streamResult.ToolCalls.Count > 0)
            {
                messages.Add(new AssistantChatMessage(streamResult.ToolCalls));
                foreach (var call in streamResult.ToolCalls)
                {
                    var args = ParseArgs(call.FunctionArguments);
                    var request = _toolFactory.Create(call.FunctionName, args.Args, args.Raw);
                    ToolResult toolResponse = await _console.WithSpinnerAsync(
                        $"running {call.FunctionName}...",
                        () => _mediator.Send(request));

                    _console.PrintToolLog(call.FunctionName, toolResponse.Log);
                    if (call.FunctionName == "run_shell")
                    {
                        UpdateShellStatus(args.Args, toolResponse);
                    }
                    messages.Add(new ToolChatMessage(call.Id, toolResponse.PayloadForModel));
                }

                continue;
            }

            if (!string.IsNullOrWhiteSpace(streamResult.Text))
            {
                messages.Add(new AssistantChatMessage(streamResult.Text));
            }

            break;
        }
    }

    private void UpdateShellStatus(Dictionary<string, string> args, ToolResult result)
    {
        args.TryGetValue("command", out var command);
        int? exitCode = null;
        try
        {
            using var doc = JsonDocument.Parse(result.PayloadForModel);
            if (doc.RootElement.TryGetProperty("exitCode", out var exit))
            {
                exitCode = exit.GetInt32();
            }
        }
        catch (Exception ex)
        {
            _logger.LogTrace(ex, "failed to parse shell exit code from tool result payload");
        }
        LastShellCommand = (command, exitCode);
    }

    private async Task<StreamResult> StreamChatAsync(ChatClient chatClient, List<ChatMessage> messages, ChatCompletionOptions options)
    {
        var stream = chatClient.CompleteChatStreamingAsync(messages, options);
        var textBuilder = new StringBuilder();
        var toolBuffers = new Dictionary<string, ToolCallBuffer>();

        _console.PrintAssistantPrefix();
        await foreach (var update in stream)
        {
            if (update.ContentUpdate is { Count: > 0 })
            {
                foreach (var part in update.ContentUpdate.Where(p => p.Kind == ChatMessageContentPartKind.Text))
                {
                    textBuilder.Append(part.Text);
                    _console.PrintTrace("stream", part.Text);
                }
            }

            if (update.ToolCallUpdates is { Count: > 0 })
            {
                foreach (var toolUpdate in update.ToolCallUpdates)
                {
                    if (!toolBuffers.TryGetValue(toolUpdate.ToolCallId, out var buffer))
                    {
                        buffer = new ToolCallBuffer(toolUpdate.ToolCallId);
                        toolBuffers[toolUpdate.ToolCallId] = buffer;
                    }

                    buffer.FunctionName ??= toolUpdate.FunctionName;
                    buffer.Index = toolUpdate.Index;
                    if (toolUpdate.FunctionArgumentsUpdate is not null)
                    {
                        buffer.Arguments.Append(toolUpdate.FunctionArgumentsUpdate.ToString());
                    }
                }
            }
        }

        _console.PrintNewLine();

        var toolCalls = toolBuffers.Values
            .OrderBy(b => b.Index)
            .Select(b => ChatToolCall.CreateFunctionToolCall(
                b.Id,
                b.FunctionName ?? "unknown_function",
                BinaryData.FromString(b.Arguments.ToString())))
            .ToList();

        return new StreamResult(textBuilder.ToString(), toolCalls);
    }

    private (Dictionary<string, string> Args, string Raw) ParseArgs(BinaryData functionArguments)
    {
        var raw = functionArguments.ToString();
        try
        {
            var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(raw) ?? new Dictionary<string, string>();
            return (dict, raw);
        }
        catch (Exception ex)
        {
            _logger.LogTrace(ex, "failed to parse tool arguments: {RawArgs}", raw);
            return (new Dictionary<string, string>(), raw);
        }
    }
}
