using System;
using System.Diagnostics.CodeAnalysis;
using PCRE.Internal;

namespace PCRE;

/// <summary>
/// Information about a callout in the regex pattern.
/// </summary>
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

    /// <summary>
    /// The number associated with the callout, or zero for named callouts.
    /// </summary>
    public int Number { get; }

    /// <summary>
    /// The name of the callout, or null for numeric callouts.
    /// </summary>
    [SuppressMessage("Naming", "CA1720")]
    public string? String { get; }

    /// <summary>
    /// The length of the next item to be processed in the pattern string.
    /// </summary>
    /// <remarks>
    /// When the callout is at the end of the pattern, the length is zero. When the callout precedes an opening parenthesis,
    /// the length includes meta characters that follow the parenthesis. For example, in a callout before an assertion such as
    /// <c>(?=ab)</c> the length is 3. For an an alternation bar or a closing parenthesis, the length is one, unless a closing
    /// parenthesis is followed by a quantifier, in which case its length is included.
    /// </remarks>
    public int NextPatternItemLength { get; }

    /// <summary>
    /// The position of the callout in the pattern.
    /// </summary>
    public int PatternPosition { get; }

    /// <summary>
    /// The code unit offset to the start of the callout argument string within the original pattern string.
    /// </summary>
    public int StringOffset { get; }
}
