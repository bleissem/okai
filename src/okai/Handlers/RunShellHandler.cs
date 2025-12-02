using System.Text.Json;
using MediatR;
using okai.Requests;

namespace okai;

public class RunShellHandler : IRequestHandler<RunShellCommand, ToolResult>
{
    private readonly IToolContext _context;
    private readonly IApprovalService _approvals;
    private readonly IShellRunner _shellRunner;
    private readonly IShellPolicy _policy;

    public RunShellHandler(IToolContext context, IApprovalService approvals, IShellRunner shellRunner, IShellPolicy policy)
    {
        _context = context;
        _approvals = approvals;
        _shellRunner = shellRunner;
        _policy = policy;
    }

    public async Task<ToolResult> Handle(RunShellCommand request, CancellationToken cancellationToken)
    {
        if (!_policy.IsAllowed(request.Command))
        {
            return new ToolResult(JsonSerializer.Serialize(new { error = "command not allowed by policy" }), "command not allowed");
        }

        if (!_approvals.Approve(request.Command))
        {
            return new ToolResult(JsonSerializer.Serialize(new { error = "command not approved" }), "command not approved");
        }

        var result = await _shellRunner.RunAsync(request.Command, _context.Root, cancellationToken);
        return new ToolResult(
            JsonSerializer.Serialize(new { exitCode = result.ExitCode, stdout = result.Stdout, stderr = result.Stderr }),
            $"shell exit {result.ExitCode}");
    }
}
