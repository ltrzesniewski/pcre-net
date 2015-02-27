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
        JavaScript = PatternOptions.AltBsUX | PatternOptions.MatchUnsetBackref,
        Unicode = PatternOptions.Ucp,

        Anchored = PatternOptions.Anchored,
        Ungreedy = PatternOptions.Ungreedy,
        NotBeginningOfLine = PatternOptions.NotBol,
        NotEndOfLine = PatternOptions.NotEol,
        NotEmpty = PatternOptions.NotEmpty,
        NotEmptyAtStart = PatternOptions.NotEmptyAtStart,
        FirstLineOnly = PatternOptions.FirstLine,
        DuplicateNames = PatternOptions.DupNames,
        AutoCallout = PatternOptions.AutoCallout,

        NoStartOptimize = PatternOptions.NoStartOptimize,
        NoAutoPossess = PatternOptions.NoAutoPossess,
        DollarEndOnly = PatternOptions.DollarEndOnly,

        AltBsUX =  PatternOptions.AltBsUX,
        AllowEmptyClass = PatternOptions.AllowEmptyClass,
        MatchUnsetBackref = PatternOptions.MatchUnsetBackref,
        NoDotStarAnchor = PatternOptions.NoDotStarAnchor,

        // Flipped options
        CheckUnicodeValidity = PatternOptions.NoUtfCheck,

        // Extra options
        Compiled = 1L << 32,
        CompiledPartial = 1L << 33,
    }
}
