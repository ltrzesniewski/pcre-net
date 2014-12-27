using PCRE.Wrapper;

namespace PCRE.Support
{
    internal static class PcreOptionsExtensions
    {
        private const PatternOptions FlippedOptions = PatternOptions.NoUtf16Check;

        public static PatternOptions ToPatternOptions(this PcreOptions options)
        {
            return ((PatternOptions)((long)options & 0xFFFFFFFF) ^ FlippedOptions) | PatternOptions.Utf16;
        }

        public static PatternOptions ToPatternOptions(this PcreMatchOptions options)
        {
            return (PatternOptions)((long)options & 0xFFFFFFFF);
        }

        public static StudyOptions? ToStudyOptions(this PcreOptions options)
        {
            if ((options & PcreOptions.Compiled) != 0)
                return StudyOptions.JitCompile;

            if ((options & PcreOptions.Studied) != 0)
                return StudyOptions.None;

            return null;
        }
    }
}
