using System;
using PCRE.Internal;

namespace PCRE;

/// <summary>
/// Additional options for the JIT compiler.
/// </summary>
[Flags]
public enum PcreJitCompileOptions : uint
{
    /// <summary>
    /// No additional options.
    /// </summary>
    None = 0,

    /// <summary>
    /// <c>PCRE2_JIT_COMPLETE</c> - Compile code for full matching.
    /// </summary>
    Complete = PcreConstants.PCRE2_JIT_COMPLETE,

    /// <summary>
    /// <c>PCRE2_JIT_PARTIAL_SOFT</c> - Compile code for soft partial matching.
    /// </summary>
    /// <see cref="PcreMatchOptions.PartialSoft"/>
    PartialSoft = PcreConstants.PCRE2_JIT_PARTIAL_SOFT,

    /// <summary>
    /// <c>PCRE2_JIT_PARTIAL_HARD</c> - Compile code for hard partial matching
    /// </summary>
    /// <see cref="PcreMatchOptions.PartialHard"/>
    PartialHard = PcreConstants.PCRE2_JIT_PARTIAL_HARD,
}
