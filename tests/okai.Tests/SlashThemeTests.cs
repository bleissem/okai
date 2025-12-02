using Azure.AI.Projects;
using Moq;
using okai;
using OpenAI.Chat;
using Xunit;

public class SlashThemeTests
{
    [Fact]
    public async Task ThemeCommand_AppliesPalette()
    {
        var console = new FakeConsoleTheme();
        var palette = new ConsolePalette("a", "u", "t", "w", "e", "tr", "ht", "hl", "hv", "sl", "sv", "si");
        var resolver = new FakeThemeResolver(palette, "default");
        var options = new AppOptions("https://example.invalid", "model", Environment.CurrentDirectory, "hist", false, "policy", "default", "themes", "search", "key", "cmd", "profile", "cfg");
        var handler = new SlashCommandHandler(console, resolver, options);

        var result = await handler.HandleAsync("/theme alt", Mock.Of<AIProjectClient>(), Mock.Of<ChatClient>(), options, new List<ChatMessage>());

        Assert.True(result.Handled);
        Assert.Equal(palette, console.LastPalette);
        Assert.True(console.HeaderRendered);
    }

    [Fact]
    public async Task ThemeList_ShowsNames()
    {
        var console = new FakeConsoleTheme();
        var resolver = new FakeThemeResolver(new ConsolePalette("a", "u", "t", "w", "e", "tr", "ht", "hl", "hv", "sl", "sv", "si"), "one", "two");
        var options = new AppOptions("https://example.invalid", "model", Environment.CurrentDirectory, "hist", false, "policy", "default", "themes", "search", "key", "cmd", "profile", "cfg");
        var handler = new SlashCommandHandler(console, resolver, options);

        var result = await handler.HandleAsync("/theme list", Mock.Of<AIProjectClient>(), Mock.Of<ChatClient>(), options, new List<ChatMessage>());

        Assert.True(result.Handled);
        Assert.Contains(console.StatusMessages, m => m.Contains("one"));
        Assert.Contains(console.StatusMessages, m => m.Contains("two"));
    }

    private sealed class FakeThemeResolver : IThemeResolver
    {
        private readonly ConsolePalette _palette;
        private readonly List<string> _names;

        public FakeThemeResolver(ConsolePalette palette, params string[] names)
        {
            _palette = palette;
            _names = names.ToList();
        }

        public IReadOnlyCollection<string> Names => _names;

        public ConsolePalette Resolve(string name) => _palette;
    }

    private sealed class FakeConsoleTheme : IConsoleTheme
    {
        public ConsolePalette? LastPalette { get; private set; }
        public bool HeaderRendered { get; private set; }
        public List<string> StatusMessages { get; } = new();

        public void ApplyPalette(ConsolePalette palette) => LastPalette = palette;
        public void PrintHeader(string endpoint, string model, string root) => HeaderRendered = true;
        public void PrintStatus(string message) => StatusMessages.Add(message);
        public void PrintAssistantPrefix() { }
        public Task<string?> ReadInputAsync() => Task.FromResult<string?>(null);
        public void AppendUserMessage(string message) { }
        public void PrintNewLine() { }
        public void PrintToolLog(string tool, string log) { }
        public void PrintWarning(string message) { }
        public void PrintError(string message) { }
        public void PrintTrace(string label, string message) { }
        public Task<T> WithSpinnerAsync<T>(string message, Func<Task<T>> action) => action();
        public void RenderStatusBar(string model, string root) { }
        public void RenderStatusBar(string model, string root, string lastCommand) { }
        public void RenderStatusBar(string model, string root, string lastCommand, int? lastExitCode) { }
    }
}
