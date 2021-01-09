using PCRE.Internal;

namespace PCRE
{
    public sealed unsafe class PcreCalloutInfo
    {
        internal PcreCalloutInfo(ref Native.pcre2_callout_enumerate_block info)
        {
            Number = (int)info.callout_number;
            String = info.callout_string != null ? new string(info.callout_string) : null;
            NextPatternItemLength = (int)info.next_item_length;
            PatternPosition = (int)info.pattern_position;
            StringOffset = (int)info.callout_string_offset;
        }

        public int Number { get; }
        public string? String { get; }

        public int NextPatternItemLength { get; }
        public int PatternPosition { get; }
        public int StringOffset { get; }
    }
}
