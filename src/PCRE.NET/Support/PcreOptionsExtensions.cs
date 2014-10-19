using PCRE.Wrapper;

namespace PCRE.Support
{
    internal static class PcreOptionsExtensions
    {
        private const PcrePatternOptions FlippedOptions = PcrePatternOptions.Utf16
                                                          | PcrePatternOptions.NoUtf16Check
                                                          | PcrePatternOptions.Ucp;

        public static PcrePatternOptions ToPatternOptions(this PcreOptions options)
        {
            return ((PcrePatternOptions)((long)options & 0xFFFFFFFF) ^ FlippedOptions) | PcrePatternOptions.Utf16;
        }

        public static PcreStudyOptions? ToStudyOptions(this PcreOptions options)
        {
            if ((options & PcreOptions.Compiled) != 0)
                return PcreStudyOptions.JitCompile;

            if ((options & PcreOptions.Studied) != 0)
                return PcreStudyOptions.None;

            return null;
        }
    }
}
