using System.Runtime.CompilerServices;

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
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return (CalloutData) _data; }
        }

        public int Number => InternalData.Number;

        public PcreMatch Match => _match ?? (_match = new PcreMatch(InternalData.Match));

        public int StartOffset => InternalData.StartOffset;
        public int CurrentOffset => InternalData.CurrentOffset;
        public int MaxCapture => InternalData.MaxCapture;
        public int LastCapture => InternalData.LastCapture;
        public int PatternPosition => InternalData.PatternPosition;
        public int NextPatternItemLength => InternalData.NextPatternItemLength;
        public int StringOffset => InternalData.Info.StringOffset;
        public string String => InternalData.Info.String;

        public bool StartMatch => (InternalData.Flags & CalloutFlags.StartMatch) != 0;
        public bool Backtrack => (InternalData.Flags & CalloutFlags.Backtrack) != 0;
    }
}
