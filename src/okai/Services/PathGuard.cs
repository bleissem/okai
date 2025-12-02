namespace okai;

public interface IPathGuard
{
    string Resolve(string root, string relative);
}

public class PathGuard : IPathGuard
{
    private const string OutsideRootMessage = "path is outside of the allowed root";

    public string Resolve(string root, string relative)
    {
        var normalizedRoot = EnsureTrailingSeparator(Path.GetFullPath(root));
        var combined = Path.GetFullPath(Path.Combine(normalizedRoot, relative));
        var relativeToRoot = Path.GetRelativePath(normalizedRoot, combined);
        if (IsOutsideRoot(relativeToRoot))
        {
            throw new InvalidOperationException(OutsideRootMessage);
        }

        return combined;
    }

    private static string EnsureTrailingSeparator(string path) =>
        path.EndsWith(Path.DirectorySeparatorChar) ? path : path + Path.DirectorySeparatorChar;

    private static bool IsOutsideRoot(string relativeToRoot) =>
        string.Equals(relativeToRoot, "..", StringComparison.Ordinal)
        || relativeToRoot.StartsWith(".." + Path.DirectorySeparatorChar, StringComparison.Ordinal)
        || relativeToRoot.StartsWith(".." + Path.AltDirectorySeparatorChar, StringComparison.Ordinal);
}
