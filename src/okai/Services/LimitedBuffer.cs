using System.Text;

namespace okai;

public sealed class LimitedBuffer
{
    private const string TruncatedSuffix = "[output truncated]";
    private readonly int _maxBytes;
    private readonly StringBuilder _builder = new();
    private int _bytes;
    private bool _truncated;

    public LimitedBuffer(int maxBytes)
    {
        _maxBytes = maxBytes;
    }

    public void AppendLine(string line)
    {
        var bytes = Encoding.UTF8.GetByteCount(line + Environment.NewLine);
        if (_bytes + bytes > _maxBytes)
        {
            _truncated = true;
            return;
        }

        _builder.AppendLine(line);
        _bytes += bytes;
    }

    public override string ToString()
    {
        if (_truncated)
        {
            return _builder.ToString() + TruncatedSuffix;
        }

        return _builder.ToString();
    }
}
