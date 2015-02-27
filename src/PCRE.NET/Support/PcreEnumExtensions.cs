using PCRE.Wrapper;

namespace PCRE.Support
{
    internal static class PcreEnumExtensions
    {
        private const PatternOptions FlippedOptions = PatternOptions.NoUtfCheck;

        public static PatternOptions ToPatternOptions(this PcreOptions options)
        {
            return ((PatternOptions)((long)options & 0xFFFFFFFF) ^ FlippedOptions) | PatternOptions.Utf;
        }

        public static PatternOptions ToPatternOptions(this PcreMatchOptions options)
        {
            return (PatternOptions)((long)options & 0xFFFFFFFF);
        }

        public static JitCompileOptions ToJitCompileOptions(this PcreOptions options)
        {
            var jitOptions = JitCompileOptions.None;

            if ((options & PcreOptions.Compiled) != 0)
                jitOptions |= JitCompileOptions.Complete;

            if ((options & PcreOptions.CompiledPartial) != 0)
                jitOptions |= JitCompileOptions.PartialSoft | JitCompileOptions.PartialHard;

            return jitOptions;
        }
    }
}
