using System;
using System.Diagnostics.CodeAnalysis;
using PCRE.Internal;

namespace PCRE
{
    [Flags]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum PcreExtraCompileOptions : uint
    {
        None = 0,
        BadEscapeIsLiteral = PcreConstants.EXTRA_BAD_ESCAPE_IS_LITERAL,
        MatchWord = PcreConstants.EXTRA_MATCH_WORD,
        MatchLine = PcreConstants.EXTRA_MATCH_LINE,
        EscapedCrIsLf = PcreConstants.EXTRA_ESCAPED_CR_IS_LF,
        AltBsUX = PcreConstants.EXTRA_ALT_BSUX
    }
}
