using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MediatR;
using OpenAI.Chat;
using okai;

static void ConfigureLogging(ILoggingBuilder builder)
{
    builder.SetMinimumLevel(LogLevel.Information);
    builder.AddSimpleConsole(options =>
    {
        options.SingleLine = true;
        options.TimestampFormat = "HH:mm:ss ";
    });
}

if (Help.ShouldShowHelp(args))
{
    Console.WriteLine(Help.Build());
    return;
}

var services = new ServiceCollection();
services.AddLogging(ConfigureLogging);

var bootstrapFactory = LoggerFactory.Create(ConfigureLogging);
var bootstrapLogger = bootstrapFactory.CreateLogger("bootstrap");

var options = AppOptions.FromEnvironment(bootstrapLogger);
if (options is null)
{
    return;
}
services.AddSingleton(options);
services.AddSingleton<IThemeResolver>(provider =>
{
    var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger("ThemeLoader");
    var themeConfig = ThemeLoader.Load(options.ThemePath, logger);
    var custom = themeConfig.Themes.ToDictionary(
        kvp => kvp.Key,
        kvp => new ConsolePalette(
            kvp.Value.AssistantPrefix,
            kvp.Value.UserPrefix,
            kvp.Value.Tool,
            kvp.Value.Warning,
            kvp.Value.Error,
            kvp.Value.Trace,
            kvp.Value.HeaderTitle,
            kvp.Value.HeaderLabel,
            kvp.Value.HeaderValue,
            kvp.Value.StatusLabel,
            kvp.Value.StatusValue,
            kvp.Value.ShellIdle),
        StringComparer.OrdinalIgnoreCase);
    return new ThemeResolver(custom);
});
services.AddSingleton(provider =>
{
    var resolver = provider.GetRequiredService<IThemeResolver>();
    return resolver.Resolve(options.Theme);
});
services.AddSingleton<IConsoleTheme, ConsoleTheme>();
services.AddSingleton<IUserInput, ConsoleUserInput>();
services.AddSingleton<IToolContext>(_ => new ToolContext(options.Root));
services.AddSingleton<IPathGuard, PathGuard>();
services.AddSingleton<IHistoryStore, JsonHistoryStore>();
services.AddSingleton<IToolRequestFactory, ToolRequestFactory>();
services.AddSingleton<ISlashCommandHandler, SlashCommandHandler>();
services.AddSingleton<IChatService, ChatService>();
services.AddSingleton<ChatLoop>();
services.AddSingleton<IApprovalService, ApprovalService>();
services.AddSingleton<IShellRunner, ShellRunner>();
services.AddSingleton<IShellPolicy, ShellPolicy>();
services.AddSingleton<IWebSearchClient, WebSearchClient>();
services.AddMediatR(typeof(ListDirHandler).Assembly);

var provider = services.BuildServiceProvider();

var projectClient = new AIProjectClient(new Uri(options.Endpoint), new DefaultAzureCredential());
var chatClient = projectClient.OpenAI.GetChatClient(options.Model);
var loop = provider.GetRequiredService<ChatLoop>();

await loop.RunAsync(projectClient, chatClient);

