using System;
using System.Diagnostics.CodeAnalysis;
using PCRE.Internal;

namespace PCRE
{
    public sealed unsafe class PcreCalloutInfo
    {
        internal PcreCalloutInfo(ref Native.pcre2_callout_enumerate_block info)
        {
            Number = (int)info.callout_number;
            String = info.callout_string != IntPtr.Zero ? new string((char*)info.callout_string) : null;
            NextPatternItemLength = (int)info.next_item_length;
            PatternPosition = (int)info.pattern_position;
            StringOffset = (int)info.callout_string_offset;
        }

        public int Number { get; }

        [SuppressMessage("Naming", "CA1720")]
        public string? String { get; }

        public int NextPatternItemLength { get; }
        public int PatternPosition { get; }
        public int StringOffset { get; }
    }
}
