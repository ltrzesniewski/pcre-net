using PCRE.Support;
using PCRE.Wrapper;

namespace PCRE
{
    public sealed class PcreRegex
    {
        private readonly PcrePattern _re;

        public static PcreBuildInfo BuildInfo
        {
            get { return PcreBuildInfo.Instance; }
        }

        public PcrePatternInfo PaternInfo { get; private set; }

        public PcreRegex(string pattern, PcreOptions options = PcreOptions.None)
        {
            _re = new PcrePattern(pattern, options.ToPatternOptions(), options.ToStudyOptions());
            PaternInfo = new PcrePatternInfo(_re);
        }

        public bool IsMatch(string subject)
        {
            return _re.IsMatch(subject);
        }
    }
}
