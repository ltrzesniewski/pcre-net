using System;
using PCRE.Wrapper;

namespace PCRE
{
    [Flags]
    public enum PcreOptions : long
    {
        None = 0,

        // Normal options
        IgnoreCase = PatternOptions.Caseless,
        Caseless = PatternOptions.Caseless,

        MultiLine = PatternOptions.MultiLine,

        Singleline = PatternOptions.DotAll,
        DotAll = PatternOptions.DotAll,

        ExplicitCapture = PatternOptions.NoAutoCapture,
        NoAutoCapture = PatternOptions.NoAutoCapture,

        IgnorePatternWhitespace = PatternOptions.Extended,
        Extended = PatternOptions.Extended,

        JavaScript = PatternOptions.AltBsUX | PatternOptions.MatchUnsetBackref,
        AltBsUX = PatternOptions.AltBsUX,
        MatchUnsetBackref = PatternOptions.MatchUnsetBackref,

        Unicode = PatternOptions.Ucp,
        Ucp = PatternOptions.Ucp,

        Anchored = PatternOptions.Anchored,
        Ungreedy = PatternOptions.Ungreedy,
        NotBol = PatternOptions.NotBol,
        NotEol = PatternOptions.NotEol,
        NotEmpty = PatternOptions.NotEmpty,
        NotEmptyAtStart = PatternOptions.NotEmptyAtStart,
        FirstLine = PatternOptions.FirstLine,
        DupNames = PatternOptions.DupNames,
        AutoCallout = PatternOptions.AutoCallout,

        NoStartOptimize = PatternOptions.NoStartOptimize,
        NoAutoPossess = PatternOptions.NoAutoPossess,
        DollarEndOnly = PatternOptions.DollarEndOnly,

        AltCircumflex = PatternOptions.AltCircumflex,
        AllowEmptyClass = PatternOptions.AllowEmptyClass,
        NoDotStarAnchor = PatternOptions.NoDotStarAnchor,

        NoUtfCheck = PatternOptions.NoUtfCheck,
        NeverUcp = PatternOptions.NeverUcp,
        NeverBackslashC = PatternOptions.NeverBackslashC,       

        // Extra options
        Compiled = 1L << 32,
        CompiledPartial = 1L << 33
    }
}
