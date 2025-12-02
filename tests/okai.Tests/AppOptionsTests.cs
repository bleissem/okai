using Microsoft.Extensions.Logging.Abstractions;
using okai;
using Xunit;

public class AppOptionsTests
{
    [Fact]
    public void FromEnvironment_ReturnsNull_WhenEndpointMissing()
    {
        var original = Environment.GetEnvironmentVariable("AZURE_AI_PROJECT_ENDPOINT");
        Environment.SetEnvironmentVariable("AZURE_AI_PROJECT_ENDPOINT", null);

        try
        {
            var options = AppOptions.FromEnvironment(NullLogger<AppOptions>.Instance);
            Assert.Null(options);
        }
        finally
        {
            Environment.SetEnvironmentVariable("AZURE_AI_PROJECT_ENDPOINT", original);
        }
    }

    [Fact]
    public void FromEnvironment_ReturnsOptions_WhenEndpointSet()
    {
        var original = Environment.GetEnvironmentVariable("AZURE_AI_PROJECT_ENDPOINT");
        Environment.SetEnvironmentVariable("AZURE_AI_PROJECT_ENDPOINT", "https://example.invalid");

        try
        {
            var options = AppOptions.FromEnvironment(NullLogger<AppOptions>.Instance);
            Assert.NotNull(options);
            Assert.Equal("https://example.invalid", options!.Endpoint);
            Assert.Equal("gpt-4o-mini", options.Model);
            Assert.False(options.ApprovalsEnabled);
            Assert.Equal("default", options.Theme);
        }
        finally
        {
            Environment.SetEnvironmentVariable("AZURE_AI_PROJECT_ENDPOINT", original);
        }
    }
}
