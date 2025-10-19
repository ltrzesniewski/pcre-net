using System.Diagnostics.CodeAnalysis;
using PCRE.Internal;

namespace PCRE;

/// <summary>
/// Represents the state during a callout invocation.
/// </summary>
public sealed unsafe class PcreCallout
{
    private readonly string _subject;
    private readonly InternalRegex16Bit _regex;
    private readonly uint _flags;
    private readonly nuint[] _oVector;
    private readonly char* _markPtr;
    private PcreMatch? _match;
    private PcreCalloutInfo? _info;

    internal PcreCallout(string subject, InternalRegex16Bit regex, Native.pcre2_callout_block* callout)
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
        _markPtr = (char*)callout->mark;

        _oVector = new nuint[callout->capture_top * 2];
        _oVector[0] = callout->start_match;
        _oVector[1] = callout->current_position;

        for (var i = 2; i < _oVector.Length; ++i)
            _oVector[i] = callout->offset_vector[i];
    }

    /// <inheritdoc cref="PcreCalloutInfo.Number"/>
    public int Number { get; }

    /// <summary>
    /// Returns the current match status.
    /// </summary>
    public PcreMatch Match => _match ??= new PcreMatch(_subject, _regex, _oVector, _markPtr);

    /// <summary>
    /// The offset within the subject at which the current match attempt started.
    /// </summary>
    /// <remarks>
    /// If the escape sequence <c>\K</c> has been encountered, this value is changed to reflect the modified starting point.
    /// If the pattern is not anchored, the callout function may be called several times from the same point in the pattern for different starting points in the subject.
    /// </remarks>
    public int StartOffset { get; }

    /// <summary>
    /// The offset within the subject of the current match pointer.
    /// </summary>
    public int CurrentOffset { get; }

    /// <summary>
    /// One more than the number of the highest numbered captured substring so far.
    /// </summary>
    /// <remarks>
    /// If no substrings have yet been captured, the value is 1.
    /// </remarks>
    public int MaxCapture { get; }

    /// <summary>
    /// The number of the most recently captured substring.
    /// </summary>
    /// <remarks>
    /// If no substrings have yet been captured, the value is 0.
    /// </remarks>
    public int LastCapture { get; }

    /// <inheritdoc cref="PcreCalloutInfo.PatternPosition"/>
    public int PatternPosition { get; }

    /// <inheritdoc cref="PcreCalloutInfo.NextPatternItemLength"/>
    public int NextPatternItemLength { get; }

    /// <inheritdoc cref="PcreCalloutInfo.StringOffset"/>
    public int StringOffset => Info.StringOffset;

    /// <inheritdoc cref="PcreCalloutInfo.String"/>
    [SuppressMessage("Naming", "CA1720")]
    public string? String => Info.String;

    /// <summary>
    /// Returns information about the callout.
    /// </summary>
    public PcreCalloutInfo Info => _info ??= _regex.GetCalloutInfoByPatternPosition(PatternPosition);

    /// <summary>
    /// <c>PCRE2_CALLOUT_STARTMATCH</c> - This is set for the first callout after the start of matching for each new starting position in the subject.
    /// </summary>
    /// <remarks>
    /// Always false for DFA matching, or if the JIT is used.
    /// </remarks>
    public bool StartMatch => (_flags & PcreConstants.CALLOUT_STARTMATCH) != 0;

    /// <summary>
    /// <c>PCRE2_CALLOUT_BACKTRACK</c> - This is set if there has been a matching backtrack since the previous callout, or since the start of matching if this is the first callout from a match run.
    /// </summary>
    /// <remarks>
    /// Always false for DFA matching, or if the JIT is used.
    /// </remarks>
    public bool Backtrack => (_flags & PcreConstants.CALLOUT_BACKTRACK) != 0;
}
