using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace PCRE.NET.Analyzers.Support;

internal class CodeWriter
{
    private const int _indentWidth = 4;

    private readonly StringBuilder _sb = new();
    private bool _isAtStartOfLine = true;

    public int Indent { get; set; }

    public CodeWriter Append<T>(T? value)
        => Append(value?.ToString());

    public CodeWriter AppendLine<T>(T? value)
        => AppendLine(value?.ToString());

    public CodeWriter Append([StringSyntax("csharp")] string? value)
    {
        if (value is null or "")
            return this;

        var position = 0;

        while (true)
        {
            AppendPendingIndent();

            var nextNewLine = value.IndexOf('\n', position);
            if (nextNewLine >= 0)
            {
                _sb.Append(value, position, nextNewLine + 1 - position);
                _isAtStartOfLine = true;

                position = nextNewLine + 1;
                continue;
            }

            _sb.Append(value, position, value.Length - position);
            break;
        }

        return this;
    }

    public CodeWriter AppendRaw([StringSyntax("csharp")] string? value)
    {
        _sb.Append(value);
        return this;
    }

    public CodeWriter AppendLine([StringSyntax("csharp")] string? value = null)
    {
        Append(value);

        _sb.AppendLine();
        _isAtStartOfLine = true;

        return this;
    }

    public CodeWriter TrimComma()
    {
        while (_sb.Length >= 1 && _sb[_sb.Length - 1] == ' ')
            _sb.Remove(_sb.Length - 1, 1);

        if (_sb.Length >= 1 && _sb[_sb.Length - 1] == ',')
            _sb.Remove(_sb.Length - 1, 1);

        return this;
    }

    public override string ToString()
        => _sb.ToString();

    public BlockScope WriteBlock([StringSyntax("csharp")] string? header = null)
    {
        Append(header);
        EnsureIsOnNewLine();
        AppendLine("{");
        Indent++;
        return new BlockScope(this);
    }

    private void EnsureIsOnNewLine()
    {
        if (!_isAtStartOfLine)
            AppendLine();
    }

    public CodeWriter AppendPendingIndent()
    {
        if (_isAtStartOfLine)
            _sb.Append(' ', Indent * _indentWidth);

        _isAtStartOfLine = false;
        return this;
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
