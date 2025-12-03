using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using okai.Agents;

namespace okai.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOkaiCore(this IServiceCollection services, AppOptions options)
    {
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
        services.AddTransient<IPathGuard, PathGuard>();
        services.AddTransient<IToolRequestFactory, ToolRequestFactory>();
        services.AddTransient<IApprovalService, ApprovalService>();
        services.AddTransient<IShellRunner, ShellRunner>();
        services.AddSingleton<IShellPolicy, ShellPolicy>();
        services.AddSingleton<IWebSearchClient, WebSearchClient>();
        return services;
    }

    public static IServiceCollection AddOkaiAgents(this IServiceCollection services)
    {
        services.AddTransient<IAgentTooling, AgentTooling>();
        services.AddSingleton<IAgentFactory, AgentFactory>();
        services.AddSingleton<IChatLoop, ChatLoop>();
        return services;
    }

    public static IServiceCollection AddOkaiFoundry(this IServiceCollection services, AppOptions options)
    {
        services.AddSingleton(provider => new AIProjectClient(new Uri(options.Endpoint), new DefaultAzureCredential()));
        return services;
    }
}
