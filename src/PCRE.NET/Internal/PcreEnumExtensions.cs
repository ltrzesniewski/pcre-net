using System.Runtime.CompilerServices;

namespace PCRE.Internal;

internal static class PcreEnumExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ToPatternOptions(this PcreOptions options)
        => (uint)((long)options & 0xFFFFFFFF) | PcreConstants.PCRE2_UTF;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ToPatternOptions(this PcreMatchOptions options)
        => (uint)((long)options & 0xFFFFFFFF);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ToSubstituteOptions(this PcreSubstituteOptions options)
        => (uint)((long)options & 0xFFFFFFFF);

    public static PcreJitCompileOptions ToJitCompileOptions(this PcreOptions options)
    {
        var jitOptions = PcreJitCompileOptions.None;

        if ((options & PcreOptions.Compiled) != 0)
            jitOptions |= PcreJitCompileOptions.Complete;

        if ((options & PcreOptions.CompiledPartial) != 0)
            jitOptions |= PcreJitCompileOptions.PartialSoft | PcreJitCompileOptions.PartialHard;

        return jitOptions;
    }
}
