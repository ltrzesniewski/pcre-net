using System.Runtime.CompilerServices;
using PCRE.Wrapper;

namespace PCRE
{
    public sealed class PcreCallout
    {
        private readonly object _data; // See remark about JIT in PcreRegex
        private PcreMatch _match;

        internal PcreCallout(CalloutData data)
        {
            _data = data;
        }

        private CalloutData InternalData
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return (CalloutData)_data; }
        }

        public int Number
        {
            get { return InternalData.Number; }
        }

        public PcreMatch Match
        {
            get { return _match ?? (_match = new PcreMatch(InternalData.Match)); }
        }

        public int StartOffset
        {
            get { return InternalData.StartOffset; }
        }

        public int CurrentOffset
        {
            get { return InternalData.CurrentOffset; }
        }

        public int MaxCapture
        {
            get { return InternalData.MaxCapture; }
        }

        public int LastCapture
        {
            get { return InternalData.LastCapture; }
        }

        public int PatternPosition
        {
            get { return InternalData.PatternPosition; }
        }

        public int NextPatternItemLength
        {
            get { return InternalData.NextPatternItemLength; }
        }
    }
}
