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
            if ((options & PcreOptions.Compiled) != 0)
                return JitCompileOptions.Complete | JitCompileOptions.PartialSoft | JitCompileOptions.PartialHard;

            return JitCompileOptions.None;
        }
    }
}
