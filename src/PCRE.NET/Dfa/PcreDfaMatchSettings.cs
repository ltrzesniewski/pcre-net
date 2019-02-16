using System;
using PCRE.Support;

namespace PCRE.Dfa
{
    public sealed class PcreDfaMatchSettings
    {
        public PcreDfaMatchOptions AdditionalOptions { get; set; }
        public int StartIndex { get; set; }
        public event Func<PcreCallout, PcreCalloutResult> OnCallout;

        public uint MaxResults { get; set; }
        public uint WorkspaceSize { get; set; }

        public PcreDfaMatchSettings()
        {
            WorkspaceSize = 128;
            MaxResults = 128;
        }

        internal MatchContext CreateMatchContext(string subject)
        {
            var context = new MatchContext
            {
                Subject = subject,
                StartIndex = StartIndex,
                AdditionalOptions = ((PcreMatchOptions)AdditionalOptions).ToPatternOptions(),
                CalloutHandler = PcreMatchSettings.WrapCallout(OnCallout),
                DfaMaxResults = (AdditionalOptions & PcreDfaMatchOptions.DfaShortest) != 0 ? 1 : MaxResults,
                DfaWorkspaceSize = WorkspaceSize
            };

            return context;
        }
    }
}
