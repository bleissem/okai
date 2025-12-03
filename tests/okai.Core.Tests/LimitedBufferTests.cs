using okai;
using Xunit;

namespace okai.Tests;

public class LimitedBufferTests
{
    [Fact]
    public void Truncates_WhenOverLimit()
    {
        var buffer = new LimitedBuffer(10);
        buffer.AppendLine("12345");
        buffer.AppendLine("67890");

        var text = buffer.ToString();
        Assert.Contains("12345", text);
        Assert.Contains("[output truncated]", text);
    }
}
