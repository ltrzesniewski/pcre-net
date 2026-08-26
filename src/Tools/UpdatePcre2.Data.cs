using System.Globalization;
using System.Text.RegularExpressions;

internal static class UpdateData
{
    public static readonly string[] CutPositions =
    [
        "QUERY_BAR|JX_NEST_TOO_DEEP",
        "E|CLASS_NEST_TOO_DEEP",
        "E|CLASS_INVALID_OPERATOR",
        "E|CLASS_UNEXPECTED_OPERATOR",
        "E|CLASS_EXPECTED_OPERAND",
        "E|CLASS_MIXED_OPERATORS",
        "E|CLASS_HINT_SQUARE_BRACKET",
        "PERL_E|CLASS_UNEXPECTED_EXPR",
        "PERL_E|CLASS_EMPTY_EXPR",
        "PERL_E|CLASS_MISSING_CLOSE",
        "PERL_E|CLASS_UNEXPECTED_CHAR",
        "NULL_ERROR|OFFSET",
        "NO|MATCH",
        "BAD|DATA",
        "MIXED|TABLES",
        "BAD|MAGIC",
        "BAD|MODE",
        "BAD|OFFSET",
        "BAD|OPTION",
        "BAD|REPLACEMENT",
        "BAD|UTF|OFFSET",
        "DFA_BAD|RESTART",
        "DFA_RECURSE",
        "DFA_U|COND",
        "DFA_U|FUNC",
        "DFA_U|ITEM",
        "DFA_WS|SIZE",
        "JIT_BAD|OPTION",
        "JIT_STACK|LIMIT",
        "MATCH|LIMIT",
        "NO|MEMORY",
        "NO|SUBSTRING",
        "NO|UNIQUE|SUBSTRING",
        "RECURSE|LOOP",
        "DEPTH|LIMIT",
        "BAD|OFFSET|LIMIT",
        "BAD|REP|ESCAPE",
        "REP|MISSING|BRACE",
        "BAD|SUBSTITUTION",
        "BAD|SUBS|PATTERN",
        "TOO|MANY|REPLACE",
        "BAD|SERIALIZED|DATA",
        "HEAP|LIMIT",
        "CONVERT|SYNTAX",
        "INTERNAL_DUP|MATCH",
        "DFA_U|INVALID_UTF",
        "INVALID|OFFSET",
        "JIT|UNSUPPORTED",
        "REPLACE|CASE",
        "TOO|LARGE|REPLACE",
        "DIFF|SUBS|PATTERN",
        "DIFF|SUBS|SUBJECT",
        "DIFF|SUBS|OFFSET",
        "DIFF|SUBS|OPTIONS",
        "BAD|BACKSLASH|K",
        "PARTIAL|SUBS",
    ];
}

internal readonly record struct PcreConstant(string Type, string Name, string Value)
{
    public static IEnumerable<PcreConstant> ParsePcre2Header(string headerPath)
    {
        var commentRe = new Regex(
            @"/\* .*? \*/",
            RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace
        );

        var defineRe = new Regex(
            """
        ^ \s* \# \s* define \s+
        (?<name>PCRE2_\w+) \s+
        \(? \s*
        (?<value> -? (?:0x)? [0-9]+ )
        [uU]?
        \s* \)?
        \s* $
        """,
            RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline
        );

        var toIgnore = new HashSet<string>
        {
            "PCRE2_MAJOR",
            "PCRE2_MINOR",
            "PCRE2_LOCAL_WIDTH"
        };

        foreach (var fileLine in File.ReadLines(headerPath))
        {
            if (fileLine.IndexOf("Obsolete", StringComparison.OrdinalIgnoreCase) >= 0)
                continue;

            var line = commentRe.Replace(fileLine, string.Empty);
            var m = defineRe.Match(line);

            if (!m.Success)
                continue;

            var (name, value) = (m.Groups["name"].Value, m.Groups["value"].Value);

            if (toIgnore.Contains(name))
                continue;

            var type = name.StartsWith("PCRE2_ERROR_") ? "int" : "uint";

            yield return new PcreConstant(type, name, value);
        }
    }
}

internal readonly record struct PcreErrorMessage(int Value, string Message)
{
    public static IEnumerable<PcreErrorMessage> ParseErrorFile(string filePath)
    {
        // That's not the cleanest method, but the native library is not compiled at this point...

        using var enumerator = File.ReadAllLines(filePath).AsEnumerable().GetEnumerator();

        enumerator.AdvanceToMatch("compile_error_texts[] =");
        const int compileErrorBase = 100;
        var value = compileErrorBase;

        while (true)
        {
            var line = enumerator.AdvanceToNextLine().Trim();

            if (ShouldStop(line))
                break;

            if (ShouldSkipLine(line, value - compileErrorBase))
                continue;

            if (line == "#ifndef EBCDIC")
            {
                yield return new PcreErrorMessage(value++, GetMessage(enumerator.AdvanceToNextLine()));
                enumerator.AdvanceToMatch("#endif");
                continue;
            }

            yield return new PcreErrorMessage(value++, GetMessage(line));
        }

        enumerator.AdvanceToMatch("match_error_texts[] =");
        value = 0;

        while (true)
        {
            var line = enumerator.AdvanceToNextLine().Trim();

            if (ShouldStop(line))
                break;

            if (ShouldSkipLine(line, -value))
                continue;

            yield return new PcreErrorMessage(value--, GetMessage(line));
        }
    }

    private static bool ShouldStop(string line)
        => line == ";";

    private static bool ShouldSkipLine(string line, int expectedIndex)
    {
        if (line.StartsWith("/*", StringComparison.Ordinal) && line.EndsWith("*/", StringComparison.Ordinal))
        {
            if (int.TryParse(line[2..^2].Trim(), out var index) && index != expectedIndex)
                throw new InvalidOperationException($"Unexpected position: got {index}, expected: {expectedIndex}");

            return true;
        }

        return false;
    }

    private static string GetMessage(string line)
    {
        var match = Regex.Match(
            line,
            """
            ^ \s* " (?<message> .*? ) \\0 " \s*
            (?:
                /\* .* \*/ \s*
            )? $
            """,
            RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace
        );

        if (!match.Success)
            throw new InvalidOperationException($"Unexpected line format: {line}");

        var message = match.Groups["message"].Value;

        message = message.Replace("""\\""", """\""")
                         .Replace("""in UTF-" XSTRING(PCRE2_CODE_UNIT_WIDTH) " mode""", "in the used UTF mode")
                         .Replace(""" (maximum " XSTRING(MAX_NAME_SIZE) " code units)""", string.Empty)
                         .Replace(""" (maximum " XSTRING(MAX_NAME_COUNT) ")""", string.Empty);

        if (message.Contains("XSTRING"))
            throw new InvalidOperationException($"Unexpected XSTRING in message: {message}");

        return message;
    }
}

file static class Extensions
{
    extension(IEnumerator<string> enumerator)
    {
        public string AdvanceToNextLine()
            => enumerator.MoveNext() ? enumerator.Current : throw new InvalidOperationException("Could not advance to the next line.");

        public void AdvanceToMatch(string content)
        {
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Contains(content))
                    return;
            }

            throw new InvalidOperationException($"Could not find '{content}'.");
        }
    }
}
