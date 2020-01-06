using System;
using PCRE.Internal;

namespace PCRE
{
    public delegate PcreCalloutResult PcreRefCalloutFunc(PcreRefCallout callout);

    public unsafe ref struct PcreRefCallout
    {
        private readonly ReadOnlySpan<char> _subject;
        private readonly InternalRegex _regex;
        private readonly Native.pcre2_callout_block* _callout;

        private uint[] _oVector;
        private PcreCalloutInfo _info;

        internal PcreRefCallout(ReadOnlySpan<char> subject, InternalRegex regex, Native.pcre2_callout_block* callout)
        {
            _subject = subject;
            _regex = regex;
            _callout = callout;

            _oVector = null;
            _info = null;
        }

        public int Number => (int)_callout->callout_number;

        public PcreRefMatch Match
        {
            get
            {
                if (_oVector == null)
                {
                    _oVector = new uint[_callout->capture_top * 2];
                    _oVector[0] = (uint)_callout->start_match;
                    _oVector[1] = (uint)_callout->current_position;

                    for (var i = 2; i < _oVector.Length; ++i)
                        _oVector[i] = _callout->offset_vector[i].ToUInt32();
                }

                return new PcreRefMatch(_subject, _regex, _oVector, _callout->mark);
            }
        }

        public int StartOffset => (int)_callout->start_match;
        public int CurrentOffset => (int)_callout->current_position;
        public int MaxCapture => (int)_callout->capture_top;
        public int LastCapture => (int)_callout->capture_last;
        public int PatternPosition => (int)_callout->pattern_position;

        public int NextPatternItemLength => (int)_callout->next_item_length;
        public int StringOffset => Info.StringOffset;
        public string String => Info.String;

        public PcreCalloutInfo Info => _info ??= _regex.GetCalloutInfoByPatternPosition(PatternPosition);

        public bool StartMatch => (_callout->callout_flags & PcreConstants.CALLOUT_STARTMATCH) != 0;
        public bool Backtrack => (_callout->callout_flags & PcreConstants.CALLOUT_BACKTRACK) != 0;
    }
}
