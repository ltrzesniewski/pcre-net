using PCRE.Wrapper;

namespace PCRE
{
    public sealed class Pcre
    {
        private readonly PcrePattern _re;

        public static PcreInformation BuildInfo { get { return PcreInformation.Instance; } }

        public Pcre(string pattern)
        {
            _re = new PcrePattern(pattern, 0);
        }

        public bool IsMatch(string subject)
        {
            return _re.IsMatch(subject);
        }
    }
}
