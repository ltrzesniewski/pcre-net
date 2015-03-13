using System;
using PCRE.Support;
using PCRE.Wrapper;

namespace PCRE
{
    public sealed class PcreMatchSettings
    {
        private uint? _matchLimit;
        private uint? _recursionLimit;

        public PcreMatchOptions AdditionalOptions { get; set; }
        public int StartIndex { get; set; }
        public event Func<PcreCallout, PcreCalloutResult> OnCallout;

        public uint MatchLimit
        {
            get { return _matchLimit ?? PcreBuildInfo.MatchLimit; }
            set { _matchLimit = value; }
        }

        public uint RecursionLimit
        {
            get { return _recursionLimit ?? PcreBuildInfo.RecursionLimit; }
            set { _recursionLimit = value; }
        }

        internal MatchContext CreateMatchContext(string subject)
        {
            var context = new MatchContext
            {
                Subject = subject,
                StartIndex = StartIndex,
                AdditionalOptions = AdditionalOptions.ToPatternOptions(),
                CalloutHandler = WrapCallout(OnCallout)
            };

            if (_matchLimit != null)
                context.MatchLimit = _matchLimit.Value;

            if (_recursionLimit != null)
                context.RecursionLimit = _recursionLimit.Value;

            return context;
        }

        internal static Func<CalloutData, CalloutResult> WrapCallout(Func<PcreCallout, PcreCalloutResult> callout)
        {
            if (callout == null)
                return null;

            return data => (CalloutResult)callout(new PcreCallout(data));
        }
    }
}
