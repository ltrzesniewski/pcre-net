using System;
using PCRE.Internal;
using PCRE.Support;

namespace PCRE
{
    public sealed class PcreMatchSettings
    {
        private uint? _matchLimit;
        private uint? _depthLimit;
        private uint? _heapLimit;

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

        public uint HeapLimit
        {
            get => _heapLimit ?? PcreBuildInfo.HeapLimit;
            set => _heapLimit = value;
        }

        public uint? OffsetLimit { get; set; }

        public PcreJitStack JitStack { get; set; }

        void Dummy()
        {
            OnCallout?.Invoke(null);
        }

        internal void FillMatchInput(ref Native.match_input input)
        {
            input.additional_options = AdditionalOptions.ToPatternOptions();
            input.start_index = (uint)StartIndex;
            input.match_limit = _matchLimit.GetValueOrDefault();
            input.depth_limit = _depthLimit.GetValueOrDefault();
            input.heap_limit = _heapLimit.GetValueOrDefault();
            input.offset_limit = OffsetLimit.GetValueOrDefault();
        }

//        internal MatchContext CreateMatchContext(string subject)
//        {
//            var context = new MatchContext
//            {
//                Subject = subject,
//                StartIndex = StartIndex,
//                AdditionalOptions = AdditionalOptions.ToPatternOptions(),
//                CalloutHandler = WrapCallout(OnCallout)
//            };
//
//            if (_matchLimit != null)
//                context.MatchLimit = _matchLimit.GetValueOrDefault();
//
//            if (_depthLimit != null)
//                context.DepthLimit = _depthLimit.GetValueOrDefault();
//
//            if (_heapLimit != null)
//                context.HeapLimit = _heapLimit.GetValueOrDefault();
//
//            if (OffsetLimit != null)
//                context.OffsetLimit = OffsetLimit.GetValueOrDefault();
//
//            if (JitStack != null)
//                context.JitStack = JitStack.GetStack();
//
//            return context;
//        }
//
//        internal static Func<CalloutData, CalloutResult> WrapCallout(Func<PcreCallout, PcreCalloutResult> callout)
//        {
//            if (callout == null)
//                return null;
//
//            return data => (CalloutResult)callout(new PcreCallout(data));
//        }
    }
}
