using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MediatR;
using Microsoft.Agents.AI;
using okai;
using okai.Agents;
using okai.Extensions;

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
services.AddOkaiCore(options);
services.AddOkaiAgents();
services.AddMediatR(typeof(ListDirHandler).Assembly);
services.AddOkaiFoundry(options);

var provider = services.BuildServiceProvider();

var loop = provider.GetRequiredService<IChatLoop>();

await loop.RunAsync();

