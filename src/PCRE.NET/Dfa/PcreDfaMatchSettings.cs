using System;
using System.Diagnostics.CodeAnalysis;
using PCRE.Internal;

namespace PCRE.Dfa
{
    public sealed class PcreDfaMatchSettings
    {
        private static readonly PcreDfaMatchSettings _defaultSettings = new PcreDfaMatchSettings();

        public PcreDfaMatchOptions AdditionalOptions { get; set; }
        public int StartIndex { get; set; }

        public uint MaxResults { get; set; } = 128;
        public uint WorkspaceSize { get; set; } = 128;

        [SuppressMessage("ReSharper", "DelegateSubtraction")]
        public event Func<PcreCallout, PcreCalloutResult> OnCallout
        {
            add => Callout += value;
            remove => Callout -= value;
        }

        internal Func<PcreCallout, PcreCalloutResult> Callout { get; private set; }

        internal static PcreDfaMatchSettings GetSettings(int startIndex, PcreDfaMatchOptions options)
        {
            if (startIndex == 0 && options == PcreDfaMatchOptions.None)
                return _defaultSettings;

            return new PcreDfaMatchSettings
            {
                StartIndex = startIndex,
                AdditionalOptions = options
            };
        }

        internal void FillMatchInput(ref Native.dfa_match_input input)
        {
            input.max_results = (AdditionalOptions & PcreDfaMatchOptions.DfaShortest) != 0 ? 1 : Math.Max(1, MaxResults);
            input.workspace_size = WorkspaceSize;
        }
    }
}
