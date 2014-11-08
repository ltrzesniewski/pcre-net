using System;
using PCRE.Wrapper;

namespace PCRE
{
    [Flags]
    public enum PcreOptions : long
    {
        None = 0,

        // Normal options
        IgnoreCase = PatternOptions.CaseLess,
        MultiLine = PatternOptions.MultiLine,
        Singleline = PatternOptions.DotAll,
        ExplicitCapture = PatternOptions.NoAutoCapture,
        IgnorePatternWhitespace = PatternOptions.Extended,
        JavaScript = PatternOptions.JavaScriptCompat,
        Unicode = PatternOptions.Ucp,

        Anchored = PatternOptions.Anchored,
        Ungreedy = PatternOptions.Ungreedy,
        NotBeginningOfLine = PatternOptions.NotBol,
        NotEndOfLine = PatternOptions.NotEol,
        NotEmpty = PatternOptions.NotEmpty,
        NotEmptyAtStart = PatternOptions.NotEmptyAtStart,
        FirstLineOnly = PatternOptions.FirstLine,
        DuplicateNames = PatternOptions.DupNames,

        NewLineCr = PatternOptions.NewLineCr,
        NewLineLf = PatternOptions.NewLineLf,
        NewLineCrLf = PatternOptions.NewLineCrLf,
        NewLineAny = PatternOptions.NewLineAny,
        NewLineAnyCrLf = PatternOptions.NewLineAnyCrLf,

        BackslashRAnyCrLf = PatternOptions.BsrAnyCrLf,
        BackslashRUnicode = PatternOptions.BsrUnicode,

        NoStartOptimize = PatternOptions.NoStartOptimize,
        NoAutoPossess = PatternOptions.NoAutoPossess,
        DollarEndOnly = PatternOptions.DollarEndOnly,
        ExtraPcreFunctionality = PatternOptions.Extra,

        // Flipped options
        CheckUnicodeValidity = PatternOptions.NoUtf16Check,

        // Extra options
        Studied = 1L << 32,
        Compiled = 1L << 33
    }
}
