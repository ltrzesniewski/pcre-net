using PCRE.Wrapper;

namespace PCRE
{
    public sealed class PcrePatternInfo
    {
        private readonly PcrePattern _re;

        internal PcrePatternInfo(PcrePattern re)
        {
            _re = re;
        }

        public int MaxBackReference
        {
            get { return _re.GetInfoInt32(PatternInfoKey.BackRefMax); }
        }

        public int CaptureCount
        {
            get { return _re.GetInfoInt32(PatternInfoKey.CaptureCount); }
        }

        public bool IsCompiled
        {
            get { return _re.GetInfoInt32(PatternInfoKey.Jit) != 0; }
        }

        public bool CanMatchEmptyString
        {
            get { return _re.GetInfoInt32(PatternInfoKey.MatchEmpty) != 0; }
        }

        public int MaxLookBehind
        {
            get { return _re.GetInfoInt32(PatternInfoKey.MaxLookBehind); }
        }

        public int MinSubjectLength
        {
            get { return _re.GetInfoInt32(PatternInfoKey.MinLength); }
        }

        public int NamedGroupsCount
        {
            get { return _re.GetInfoInt32(PatternInfoKey.NameCount); }
        }

        public int RecursionLimit
        {
            get { return _re.GetInfoInt32(PatternInfoKey.RecursionLimit); }
        }
    }
}
