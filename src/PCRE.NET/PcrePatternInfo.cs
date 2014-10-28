using PCRE.Wrapper;

namespace PCRE
{
    public sealed class PcrePatternInfo
    {
        private readonly InternalRegex _re;

        internal PcrePatternInfo(InternalRegex re, string pattern, PcreOptions options)
        {
            _re = re;
            PatternString = pattern;
            Options = options;
        }

        public string PatternString { get; private set; }
        public PcreOptions Options { get; private set; }

        public int MaxBackReference
        {
            get { return _re.GetInfoInt32(InfoKey.BackRefMax); }
        }

        public int CaptureCount
        {
            get { return _re.CaptureCount; }
        }

        public bool IsCompiled
        {
            get { return _re.GetInfoInt32(InfoKey.Jit) != 0; }
        }

        public bool CanMatchEmptyString
        {
            get { return _re.GetInfoInt32(InfoKey.MatchEmpty) != 0; }
        }

        public int MaxLookBehind
        {
            get { return _re.GetInfoInt32(InfoKey.MaxLookBehind); }
        }

        public int MinSubjectLength
        {
            get { return _re.GetInfoInt32(InfoKey.MinLength); }
        }

        public int NamedGroupsCount
        {
            get { return _re.GetInfoInt32(InfoKey.NameCount); }
        }

        public int RecursionLimit
        {
            get { return _re.GetInfoInt32(InfoKey.RecursionLimit); }
        }
    }
}
