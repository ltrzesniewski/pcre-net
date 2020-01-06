using PCRE.Internal;

namespace PCRE
{
    public sealed unsafe class PcreCallout
    {
        private readonly string _subject;
        private readonly InternalRegex _regex;
        private readonly uint _flags;
        private readonly uint[] _oVector;
        private readonly char* _markPtr;
        private PcreMatch _match;
        private PcreCalloutInfo _info;

        internal PcreCallout(string subject, InternalRegex regex, Native.pcre2_callout_block* callout)
        {
            _subject = subject;
            _regex = regex;
            _flags = callout->callout_flags;

            Number = (int)callout->callout_number;
            StartOffset = (int)callout->start_match;
            CurrentOffset = (int)callout->current_position;
            MaxCapture = (int)callout->capture_top;
            LastCapture = (int)callout->capture_last;
            PatternPosition = (int)callout->pattern_position;
            NextPatternItemLength = (int)callout->next_item_length;
            _markPtr = callout->mark;

            _oVector = new uint[callout->capture_top * 2];
            _oVector[0] = (uint)callout->start_match;
            _oVector[1] = (uint)callout->current_position;

            for (var i = 2; i < _oVector.Length; ++i)
                _oVector[i] = callout->offset_vector[i].ToUInt32();
        }

        public int Number { get; }

        public PcreMatch Match => _match ??= new PcreMatch(_subject, _regex, _oVector, _markPtr);

        public int StartOffset { get; }
        public int CurrentOffset { get; }
        public int MaxCapture { get; }
        public int LastCapture { get; }
        public int PatternPosition { get; }

        public int NextPatternItemLength { get; }
        public int StringOffset => Info.StringOffset;
        public string String => Info.String;

        public PcreCalloutInfo Info => _info ??= _regex.GetCalloutInfoByPatternPosition(PatternPosition);

        public bool StartMatch => (_flags & PcreConstants.CALLOUT_STARTMATCH) != 0;
        public bool Backtrack => (_flags & PcreConstants.CALLOUT_BACKTRACK) != 0;
    }
}
