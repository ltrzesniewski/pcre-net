using System;
using PCRE.Internal;

namespace PCRE
{
    public sealed class PcreMatchSettings
    {
        internal static PcreMatchSettings Default { get; } = new();

        private uint? _matchLimit;
        private uint? _depthLimit;
        private uint? _heapLimit;

        public PcreMatchOptions AdditionalOptions { get; set; }
        public int StartIndex { get; set; }

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

        public uint HeapLimit
        {
            get => _heapLimit ?? PcreBuildInfo.HeapLimit;
            set => _heapLimit = value;
        }

        public uint? OffsetLimit { get; set; }

        public PcreJitStack? JitStack { get; set; }

        internal Func<PcreCallout, PcreCalloutResult>? Callout { get; private set; }
        internal PcreRefCalloutFunc? RefCallout { get; private set; }

        public void SetCallout(Func<PcreCallout, PcreCalloutResult>? callout)
        {
            Callout = callout;
            RefCallout = null;
        }

        public void SetCallout(PcreRefCalloutFunc? callout)
        {
            Callout = null;
            RefCallout = callout;
        }

        internal static PcreMatchSettings GetSettings(int startIndex, PcreMatchOptions additionalOptions, Func<PcreCallout, PcreCalloutResult>? callout)
        {
            if (startIndex == 0 && additionalOptions == PcreMatchOptions.None && callout == null)
                return Default;

            var settings = new PcreMatchSettings
            {
                StartIndex = startIndex,
                AdditionalOptions = additionalOptions
            };

            settings.SetCallout(callout);

            return settings;
        }

        internal static PcreMatchSettings GetSettings(int startIndex, PcreMatchOptions additionalOptions, PcreRefCalloutFunc? callout)
        {
            if (startIndex == 0 && additionalOptions == PcreMatchOptions.None && callout == null)
                return Default;

            var settings = new PcreMatchSettings
            {
                StartIndex = startIndex,
                AdditionalOptions = additionalOptions
            };

            settings.SetCallout(callout);

            return settings;
        }

        internal void FillMatchInput(ref Native.match_input input)
        {
            input.match_limit = _matchLimit.GetValueOrDefault();
            input.depth_limit = _depthLimit.GetValueOrDefault();
            input.heap_limit = _heapLimit.GetValueOrDefault();
            input.offset_limit = OffsetLimit.GetValueOrDefault();
            input.jit_stack = JitStack?.GetStack() ?? IntPtr.Zero;
        }
    }
}
