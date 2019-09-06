using System;
using System.Diagnostics.CodeAnalysis;
using PCRE.Internal;

namespace PCRE
{
    public sealed class PcreMatchSettings
    {
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

        public PcreJitStack? JitStack { get; set; }

        internal Func<PcreCallout, PcreCalloutResult>? Callout { get; private set; }

        internal void FillMatchInput(ref Native.match_input input)
        {
            input.additional_options = AdditionalOptions.ToPatternOptions();
            input.start_index = (uint)StartIndex;
            input.match_limit = _matchLimit.GetValueOrDefault();
            input.depth_limit = _depthLimit.GetValueOrDefault();
            input.heap_limit = _heapLimit.GetValueOrDefault();
            input.offset_limit = OffsetLimit.GetValueOrDefault();
            input.jit_stack = JitStack?.GetStack() ?? IntPtr.Zero;
        }
    }
}
