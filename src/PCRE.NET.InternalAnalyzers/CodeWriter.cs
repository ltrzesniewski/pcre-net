using System;
using System.Text;

namespace PCRE.NET.InternalAnalyzers;

internal class CodeWriter
{
    private const int _indentWidth = 4;

    private readonly StringBuilder _sb = new();
    private bool _isAtStartOfLine = true;

    public int Indent { get; set; }

    public CodeWriter Append<T>(T? value)
    {
        WriteIndent();
        Append(value?.ToString() ?? "");
        return this;
    }

    public CodeWriter Append(string? value)
    {
        WriteIndent();
        _sb.Append(value);
        return this;
    }

    public CodeWriter AppendLine<T>(T? value)
    {
        AppendLine(value?.ToString());
        return this;
    }

    public CodeWriter AppendLine(string? value = null)
    {
        WriteIndent();
        _sb.Append(value).Append('\n');
        _isAtStartOfLine = true;
        return this;
    }

    public override string ToString()
        => _sb.ToString();

    public BlockScope WriteBlock()
    {
        EnsureIsOnNewLine();
        Append("{");
        Indent++;
        return new BlockScope(this);
    }

    private void EnsureIsOnNewLine()
    {
        if (!_isAtStartOfLine)
            AppendLine();
    }

    private void WriteIndent()
    {
        if (_isAtStartOfLine)
            _sb.Append(' ', Indent * _indentWidth);

        _isAtStartOfLine = false;
    }

    public readonly struct BlockScope(CodeWriter writer) : IDisposable
    {
        public void Dispose()
        {
            writer.EnsureIsOnNewLine();
            writer.Indent--;
            writer.AppendLine("}");
        }
    }
}
