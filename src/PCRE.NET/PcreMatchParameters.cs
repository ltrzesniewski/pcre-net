using System;
using PCRE.Support;
using PCRE.Wrapper;

namespace PCRE
{
    public class PcreMatchParameters
    {
        public PcreMatchOptions AdditionalOptions { get; set; }
        public int StartIndex { get; set; }
        public event Func<CalloutData, CalloutResult> OnCallout;

        internal MatchContext CreateMatchContext(string subject)
        {
            return new MatchContext
            {
                Subject = subject,
                StartIndex = StartIndex,
                AdditionalOptions = AdditionalOptions.ToPatternOptions(),
                CalloutHandler = OnCallout
            };
        }
    }
}
