using System;
using PCRE.Wrapper;

namespace PCRE
{
    [Flags]
    public enum PcreOptions : long
    {
        None = 0,

        // Normal options
        IgnoreCase = PcrePatternOptions.CaseLess,
        MultiLine = PcrePatternOptions.MultiLine,
        Singleline = PcrePatternOptions.DotAll,
        ExplicitCapture = PcrePatternOptions.NoAutoCapture,
        IgnorePatternWhitespace = PcrePatternOptions.Extended,
        JavaScript = PcrePatternOptions.JavaScriptCompat | ASCII,

        Anchored = PcrePatternOptions.Anchored,
        Ungreedy = PcrePatternOptions.Ungreedy,
        NotBeginningOfLine = PcrePatternOptions.NotBol,
        NotEndOfLine = PcrePatternOptions.NotEol,
        NotEmpty = PcrePatternOptions.NotEmpty,
        NotEmptyAtStart = PcrePatternOptions.NotEmptyAtStart,
        FirstLineOnly = PcrePatternOptions.FirstLine,
        DuplicateNames = PcrePatternOptions.DupNames,

        NewLineCr = PcrePatternOptions.NewLineCr,
        NewLineLf = PcrePatternOptions.NewLineLf,
        NewLineCrLf = PcrePatternOptions.NewLineCrLf,
        NewLineAny = PcrePatternOptions.NewLineAny,
        NewLineAnyCrLf = PcrePatternOptions.NewLineAnyCrLf,

        BackslashRAnyCrLf = PcrePatternOptions.BsrAnyCrLf,
        BackslashRUnicode = PcrePatternOptions.BsrUnicode,

        NoStartOptimize = PcrePatternOptions.NoStartOptimize,
        NoAutoPossess = PcrePatternOptions.NoAutoPossess,
        DollarEndOnly = PcrePatternOptions.DollarEndOnly,
        ExtraPcreFunctionality = PcrePatternOptions.Extra,

        // Flipped options
        ASCII = PcrePatternOptions.Ucp,
        CheckUnicodeValidity = PcrePatternOptions.NoUtf16Check,

        // Extra options
        Studied = 1L << 32,
        Compiled = 1L << 33
    }
}
