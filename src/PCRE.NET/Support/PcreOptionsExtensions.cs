using PCRE.Wrapper;

namespace PCRE.Support
{
    internal static class PcreOptionsExtensions
    {
        public static PcrePatternOptions ToPatternOptions(this PcreOptions options)
        {
            var result = PcrePatternOptions.Utf16
                         | PcrePatternOptions.NoUtf16Check
                         | PcrePatternOptions.Ucp;

            if ((options & PcreOptions.IgnoreCase) != 0)
                result |= PcrePatternOptions.CaseLess;

            if ((options & PcreOptions.MultiLine) != 0)
                result |= PcrePatternOptions.MultiLine;

            if ((options & PcreOptions.ExplicitCapture) != 0)
                result |= PcrePatternOptions.NoAutoCapture;

            if ((options & PcreOptions.Singleline) != 0)
                result |= PcrePatternOptions.DotAll;

            if ((options & PcreOptions.IgnorePatternWhitespace) != 0)
                result |= PcrePatternOptions.Extended;

            if ((options & PcreOptions.ECMAScript) != 0)
                result = (result | PcrePatternOptions.JavaScriptCompat) & ~PcrePatternOptions.Ucp;

            return result;
        }

        public static PcreStudyOptions? ToStudyOptions(this PcreOptions options)
        {
            if ((options & PcreOptions.Compiled) != 0)
                return PcreStudyOptions.JitCompile;

            if ((options & PcreOptions.Study) != 0)
                return PcreStudyOptions.None;

            return null;
        }
    }
}
