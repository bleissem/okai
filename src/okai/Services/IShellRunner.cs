namespace okai;

public interface IShellRunner
{
    Task<ShellResult> RunAsync(string command, string workingDirectory, CancellationToken cancellationToken);
}
