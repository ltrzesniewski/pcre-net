using System;
using PCRE.Internal;

namespace PCRE
{
    [Flags]
    public enum PcreJitCompileOptions : uint
    {
        None = 0,
        Complete = PcreConstants.JIT_COMPLETE,
        PartialSoft = PcreConstants.JIT_PARTIAL_SOFT,
        PartialHard = PcreConstants.JIT_PARTIAL_HARD,
        InvalidUtf = PcreConstants.JIT_INVALID_UTF
    }
}
