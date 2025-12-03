using Moq;
using okai;
using okai.Requests;
using Xunit;

namespace okai.Tests;

public class RunShellHandlerTests
{
    [Fact]
    public async Task Denies_WhenApprovalFails()
    {
        var ctx = new ToolContext(Environment.CurrentDirectory);
        var approvals = Mock.Of<IApprovalService>(_ => _.Approve(It.IsAny<string>()) == true);
        var shell = Mock.Of<IShellRunner>();
        var policy = new FakePolicy(false);
        var handler = new RunShellHandler(ctx, approvals, shell, policy);

        var result = await handler.Handle(new RunShellCommand("echo hi"), default);

        Assert.Contains("not allowed", result.PayloadForModel);
    }

    [Fact]
    public async Task ExecutesShell_WhenApproved()
    {
        var ctx = new ToolContext(Environment.CurrentDirectory);

        var approvalsMock = new Mock<IApprovalService>();
        approvalsMock.Setup(a => a.Approve(It.IsAny<string>())).Returns(true);

        var shellMock = new Mock<IShellRunner>();
        shellMock.Setup(s => s.RunAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ShellResult(0, "ok", ""));

        var policy = new FakePolicy(true);
        var handler = new RunShellHandler(ctx, approvalsMock.Object, shellMock.Object, policy);

        var result = await handler.Handle(new RunShellCommand("echo hi"), default);

        shellMock.Verify(s => s.RunAsync("echo hi", ctx.Root, It.IsAny<CancellationToken>()), Times.Once);
        Assert.Contains("\"exitCode\":0", result.PayloadForModel);
    }

    private sealed class FakePolicy : IShellPolicy
    {
        private readonly bool _allow;
        public FakePolicy(bool allow) => _allow = allow;
        public bool IsAllowed(string command) => _allow;
    }
}
