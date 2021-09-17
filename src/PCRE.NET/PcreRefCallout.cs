using System;
using System.Diagnostics.CodeAnalysis;
using PCRE.Internal;

namespace PCRE
{
    public delegate PcreCalloutResult PcreRefCalloutFunc(PcreRefCallout callout);

    public unsafe ref struct PcreRefCallout
    {
        private readonly ReadOnlySpan<char> _subject;
        private readonly InternalRegex _regex;
        private readonly Native.pcre2_callout_block* _callout;

        internal Span<uint> OutputVector;
        private bool _oVectorInitialized;

        internal PcreRefCallout(ReadOnlySpan<char> subject, InternalRegex regex, Native.pcre2_callout_block* callout, Span<uint> outputVector)
        {
            _subject = subject;
            _regex = regex;
            _callout = callout;
            OutputVector = outputVector;
            _oVectorInitialized = false;
        }

        public readonly int Number => (int)_callout->callout_number;

        public PcreRefMatch Match
        {
            get
            {
                if (!_oVectorInitialized)
                {
                    if (OutputVector.Length == 0)
                        OutputVector = new uint[_callout->capture_top * 2];

                    OutputVector[0] = (uint)_callout->start_match;
                    OutputVector[1] = (uint)_callout->current_position;

                    for (var i = 2; i < OutputVector.Length; ++i)
                        OutputVector[i] = (uint)_callout->offset_vector[i];

                    _oVectorInitialized = true;
                }

                return new PcreRefMatch(_subject, _regex, OutputVector, _callout->mark);
            }
        }

        public readonly int StartOffset => (int)_callout->start_match;
        public readonly int CurrentOffset => (int)_callout->current_position;
        public readonly int MaxCapture => (int)_callout->capture_top;
        public readonly int LastCapture => (int)_callout->capture_last;
        public readonly int PatternPosition => (int)_callout->pattern_position;

        public readonly int NextPatternItemLength => (int)_callout->next_item_length;
        public readonly int StringOffset => Info.StringOffset;

        [SuppressMessage("Naming", "CA1720")]
        public readonly string? String => Info.String;

        public readonly PcreCalloutInfo Info => _regex.GetCalloutInfoByPatternPosition(PatternPosition);

        public readonly bool StartMatch => (_callout->callout_flags & PcreConstants.CALLOUT_STARTMATCH) != 0;
        public readonly bool Backtrack => (_callout->callout_flags & PcreConstants.CALLOUT_BACKTRACK) != 0;
    }
}
