﻿using System;
using PCRE.Support;
using PCRE.Wrapper;

namespace PCRE
{
    public sealed class PcreMatchSettings
    {
        private uint? _matchLimit;
        private uint? _depthLimit;

        public PcreMatchOptions AdditionalOptions { get; set; }
        public int StartIndex { get; set; }
        public event Func<PcreCallout, PcreCalloutResult> OnCallout;

        public uint MatchLimit
        {
            get => _matchLimit ?? PcreBuildInfo.MatchLimit;
            set => _matchLimit = value;
        }

        public uint DepthLimit
        {
            get => _depthLimit ?? PcreBuildInfo.DepthLimit;
            set => _depthLimit = value;
        }

        public uint? OffsetLimit { get; set; }

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
                context.MatchLimit = _matchLimit.GetValueOrDefault();

            if (_depthLimit != null)
                context.DepthLimit = _depthLimit.GetValueOrDefault();

            if (OffsetLimit != null)
                context.OffsetLimit = OffsetLimit.GetValueOrDefault();

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
