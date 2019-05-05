using System;
using PCRE.Internal;

namespace PCRE
{
    [Flags]
    public enum PcreMatchOptions : long
    {
        None = 0,

        Anchored = PcreConstants.ANCHORED,

        NotBol = PcreConstants.NOTBOL,
        NotEol = PcreConstants.NOTEOL,
        NotEmpty = PcreConstants.NOTEMPTY,
        NotEmptyAtStart = PcreConstants.NOTEMPTY_ATSTART,

        NoStartOptimize = PcreConstants.NO_START_OPTIMIZE,

        PartialSoft = PcreConstants.PARTIAL_SOFT,
        PartialHard = PcreConstants.PARTIAL_HARD,

        NoUtfCheck = PcreConstants.NO_UTF_CHECK,
        NoJit = PcreConstants.NO_JIT,
        CopyMatchedSubject = PcreConstants.COPY_MATCHED_SUBJECT
    }
}
