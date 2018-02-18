using System;
using PCRE.Wrapper;

namespace PCRE
{
    [Flags]
    public enum PcreExtraCompileOptions : uint
    {
        None = 0,
        BadEscapeIsLiteral = ExtraCompileOptions.BadEscapeIsLiteral,
        MatchWord = ExtraCompileOptions.MatchWord,
        MatchLine = ExtraCompileOptions.MatchLine
    }
}
