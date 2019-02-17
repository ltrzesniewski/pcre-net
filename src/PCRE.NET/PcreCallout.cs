using PCRE.Internal;

namespace PCRE
{
    public sealed unsafe class PcreCallout
    {
        private readonly string _subject;
        private readonly InternalRegex _regex;
        private readonly uint _flags;
        private readonly uint[] _oVector;
        private PcreMatch _match;

        internal PcreCallout(string subject, InternalRegex regex, ref Native.pcre2_callout_block callout)
        {
            _subject = subject;
            _regex = regex;
            _flags = callout.callout_flags;

            Number = (int)callout.callout_number;
            StartOffset = (int)callout.start_match;
            CurrentOffset = (int)callout.current_position;
            MaxCapture = (int)callout.capture_top;
            LastCapture = (int)callout.capture_last;
            PatternPosition = (int)callout.pattern_position;
            NextPatternItemLength = (int)callout.next_item_length;

            _oVector = new uint[callout.capture_top * 2];
            _oVector[0] = (uint)callout.start_match;
            _oVector[1] = (uint)callout.current_position;

            for (var i = 2; i < _oVector.Length; ++i)
                _oVector[i] = callout.offset_vector[i].ToUInt32();
        }

        public int Number { get; }

        public PcreMatch Match => _match ?? (_match = new PcreMatch(_subject, _regex, _oVector));

        public int StartOffset { get; }
        public int CurrentOffset { get; }
        public int MaxCapture { get; }
        public int LastCapture { get; }
        public int PatternPosition { get; }

        public int NextPatternItemLength { get; }
//        public int StringOffset => InternalData.Info.StringOffset;
//        public string String => InternalData.Info.String;

        public bool StartMatch => (_flags & PcreConstants.CALLOUT_STARTMATCH) != 0;
        public bool Backtrack => (_flags & PcreConstants.CALLOUT_BACKTRACK) != 0;
    }
}
