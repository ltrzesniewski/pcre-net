using System;
using System.Diagnostics.CodeAnalysis;
using PCRE.Internal;

namespace PCRE
{
    [Flags]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum PcreOptions : long
    {
        None = 0,

        // Normal options
        IgnoreCase = PcreConstants.CASELESS,
        Caseless = PcreConstants.CASELESS,

        MultiLine = PcreConstants.MULTILINE,

        Singleline = PcreConstants.DOTALL,
        DotAll = PcreConstants.DOTALL,

        ExplicitCapture = PcreConstants.NO_AUTO_CAPTURE,
        NoAutoCapture = PcreConstants.NO_AUTO_CAPTURE,

        IgnorePatternWhitespace = PcreConstants.EXTENDED,
        Extended = PcreConstants.EXTENDED,
        ExtendedMore = PcreConstants.EXTENDED_MORE,

        JavaScript = PcreConstants.ALT_BSUX | PcreConstants.MATCH_UNSET_BACKREF,
        AltBsUX = PcreConstants.ALT_BSUX,
        MatchUnsetBackref = PcreConstants.MATCH_UNSET_BACKREF,
        Literal = PcreConstants.LITERAL,

        Unicode = PcreConstants.UCP,
        Ucp = PcreConstants.UCP,
        MatchInvalidUtf = PcreConstants.MATCH_INVALID_UTF,

        Anchored = PcreConstants.ANCHORED,
        EndAnchored = PcreConstants.ENDANCHORED,
        Ungreedy = PcreConstants.UNGREEDY,
        NotBol = PcreConstants.NOTBOL,
        NotEol = PcreConstants.NOTEOL,
        NotEmpty = PcreConstants.NOTEMPTY,
        NotEmptyAtStart = PcreConstants.NOTEMPTY_ATSTART,
        FirstLine = PcreConstants.FIRSTLINE,
        DupNames = PcreConstants.DUPNAMES,
        AutoCallout = PcreConstants.AUTO_CALLOUT,

        NoStartOptimize = PcreConstants.NO_START_OPTIMIZE,
        NoAutoPossess = PcreConstants.NO_AUTO_POSSESS,
        DollarEndOnly = PcreConstants.DOLLAR_ENDONLY,

        AltCircumflex = PcreConstants.ALT_CIRCUMFLEX,
        AltVerbNames = PcreConstants.ALT_VERBNAMES,
        AllowEmptyClass = PcreConstants.ALLOW_EMPTY_CLASS,
        NoDotStarAnchor = PcreConstants.NO_DOTSTAR_ANCHOR,

        NoUtfCheck = PcreConstants.NO_UTF_CHECK,
        NeverUcp = PcreConstants.NEVER_UCP,
        NeverBackslashC = PcreConstants.NEVER_BACKSLASH_C,
        UseOffsetLimit = PcreConstants.USE_OFFSET_LIMIT,

        // Extra options
        Compiled = 1L << 32,
        CompiledPartial = 1L << 33
    }
}
