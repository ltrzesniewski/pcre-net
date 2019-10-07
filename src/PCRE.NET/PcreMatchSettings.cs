using System;
using System.Diagnostics.CodeAnalysis;
using PCRE.Internal;

namespace PCRE
{
    public sealed class PcreMatchSettings
    {
        private static readonly PcreMatchSettings _defaultSettings = new PcreMatchSettings();

        private uint? _matchLimit;
        private uint? _depthLimit;
        private uint? _heapLimit;

        public PcreMatchOptions AdditionalOptions { get; set; }
        public int StartIndex { get; set; }

        [SuppressMessage("ReSharper", "DelegateSubtraction")]
        public event Func<PcreCallout, PcreCalloutResult> OnCallout
        {
            add => Callout += value;
            remove => Callout -= value;
        }

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

        public PcreJitStack JitStack { get; set; }

        internal Func<PcreCallout, PcreCalloutResult> Callout { get; private set; }

        internal static PcreMatchSettings GetSettings(int startIndex, PcreMatchOptions additionalOptions, Func<PcreCallout, PcreCalloutResult> onCallout)
        {
            if (startIndex == 0 && additionalOptions == PcreMatchOptions.None && onCallout == null)
                return _defaultSettings;

            var settings = new PcreMatchSettings
            {
                StartIndex = startIndex,
                AdditionalOptions = additionalOptions
            };

            if (onCallout != null)
                settings.OnCallout += onCallout;

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
