using System;

namespace PCRE.Tests.Pcre;

public sealed class TestPattern : IEquatable<TestPattern>
{
    public string FullString { get; }

    public string Pattern { get; set; }
    public string OptionsString { get; set; }
    public PcreOptions PatternOptions { get; set; }
    public PcreOptions ResetOptionBits { get; set; }

    public int LineNumber { get; set; }

    public bool AllMatches { get; set; }
    public bool GetRemainingString { get; set; }
    public bool ExtractMarks { get; set; }
    public bool HexEncoding { get; set; }
    public bool IncludeInfo { get; set; }

    public string? ReplaceWith { get; set; }
    public bool SubjectLiteral { get; set; }

    public bool NotSupported { get; set; }

    public uint JitStack { get; set; }

    public TestPattern(string fullString, string pattern, string optionsString, int lineNumber)
    {
        FullString = fullString;
        Pattern = pattern;
        OptionsString = optionsString;
        LineNumber = lineNumber;
    }

    public override bool Equals(object? obj)
        => Equals(obj as TestPattern);

    public bool Equals(TestPattern? other)
        => other != null && string.Equals(FullString, other.FullString);

    public override int GetHashCode()
        => FullString.GetHashCode();

    public static bool operator ==(TestPattern? left, TestPattern? right) => Equals(left, right);
    public static bool operator !=(TestPattern? left, TestPattern? right) => !Equals(left, right);

    public override string ToString() => FullString;
}
