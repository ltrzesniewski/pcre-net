using System;
using System.Diagnostics.CodeAnalysis;
using PCRE.Internal;

namespace PCRE.Dfa
{
    /// <summary>
    /// The settings for a DFA match.
    /// </summary>
    public sealed class PcreDfaMatchSettings
    {
        private static readonly PcreDfaMatchSettings _defaultSettings = new();

        /// <summary>
        /// Additional options.
        /// </summary>
        public PcreDfaMatchOptions AdditionalOptions { get; set; }

        /// <summary>
        /// The index at which the match should be attempted.
        /// </summary>
        public int StartIndex { get; set; }

        /// <summary>
        /// The maximum number of results to return for a given match index.
        /// </summary>
        public uint MaxResults { get; set; } = 128;

        /// <summary>
        /// The workspace vector size.
        /// </summary>
        /// <remarks>
        /// The workspace vector should contain at least 20 elements. It is used for keeping track of multiple paths through the pattern tree.
        /// More workspace is needed for patterns and subjects where there are a lot of potential matches.
        /// </remarks>
        public uint WorkspaceSize { get; set; } = 128;

        /// <summary>
        /// A function to be called when a callout point is reached during the match.
        /// </summary>
        [SuppressMessage("ReSharper", "DelegateSubtraction")]
        [SuppressMessage("Microsoft.Design", "CA1003")]
        [SuppressMessage("Microsoft.Design", "CA1030")]
        public event Func<PcreCallout, PcreCalloutResult>? OnCallout
        {
            add => Callout += value;
            remove => Callout -= value;
        }

        internal Func<PcreCallout, PcreCalloutResult>? Callout { get; private set; }

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
