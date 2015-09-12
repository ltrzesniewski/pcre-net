using PCRE.Wrapper;

namespace PCRE
{
    public sealed class PcreCalloutInfo
    {
        private readonly CalloutInfo _info;

        internal PcreCalloutInfo(CalloutInfo info)
        {
            _info = info;
        }

        public int Number => _info.Number;
        public string String => _info.String;

        public int NextItemLength => _info.NextItemLength;
        public int PatternPosition => _info.PatternPosition;
        public int StringOffset => _info.StringOffset;
    }
}
