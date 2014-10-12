using System;
using System.Text.RegularExpressions;

namespace PCRE
{
    [Flags]
    public enum PcreOptions
    {
        None = 0,

        // Use the same values as in RegexOptions where possible
        IgnoreCase = RegexOptions.IgnoreCase,
        MultiLine = RegexOptions.Multiline,
        ExplicitCapture = RegexOptions.ExplicitCapture,
        Compiled = RegexOptions.Compiled,
        Singleline = RegexOptions.Singleline,
        IgnorePatternWhitespace = RegexOptions.IgnorePatternWhitespace,
        ECMAScript = RegexOptions.ECMAScript,

        // PCRE-specific
        Study = 4096
    }
}
