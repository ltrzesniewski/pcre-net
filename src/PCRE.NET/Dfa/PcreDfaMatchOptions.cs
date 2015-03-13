using System;
using PCRE.Wrapper;

namespace PCRE.Dfa
{
    [Flags]
    public enum PcreDfaMatchOptions : long
    {
        None = 0,

        Anchored = PatternOptions.Anchored,

        NotBeginningOfLine = PatternOptions.NotBol,
        NotEndOfLine = PatternOptions.NotEol,
        NotEmpty = PatternOptions.NotEmpty,
        NotEmptyAtStart = PatternOptions.NotEmptyAtStart,

        NoStartOptimize = PatternOptions.NoStartOptimize,

        PartialSoft = PatternOptions.PartialSoft,
        PartialHard = PatternOptions.PartialHard,

        NoUtfCheck = PatternOptions.NoUtfCheck,

        ShortestMatch = PatternOptions.DfaShortest
    }
}
