using System;
using System.Diagnostics.CodeAnalysis;
using PCRE.Internal;

namespace PCRE;

/// <summary>
/// A callout function.
/// </summary>
public delegate PcreCalloutResult PcreRefCalloutFunc(PcreRefCallout callout);

/// <inheritdoc cref="PcreCallout"/>
[ForwardTo8Bit]
public unsafe ref struct PcreRefCallout
{
    private readonly ReadOnlySpan<char> _subject;
    private readonly InternalRegex16Bit _regex;
    private readonly Native.pcre2_callout_block* _callout;

    internal Span<nuint> OutputVector;
    private bool _oVectorInitialized;

    [ForwardTo8Bit]
    internal PcreRefCallout(ReadOnlySpan<char> subject, InternalRegex16Bit regex, Native.pcre2_callout_block* callout, Span<nuint> outputVector)
    {
        _subject = subject;
        _regex = regex;
        _callout = callout;
        OutputVector = outputVector;
        _oVectorInitialized = false;
    }

    /// <inheritdoc cref="PcreCallout.Number"/>
    [ForwardTo8Bit]
    public readonly int Number => (int)_callout->callout_number;

    /// <inheritdoc cref="PcreCallout.Match"/>
    [ForwardTo8Bit]
    public PcreRefMatch Match
    {
        get
        {
            if (!_oVectorInitialized)
            {
                OutputVector = OutputVector.Length == 0
                    ? new nuint[_callout->capture_top * 2]
                    : OutputVector.Slice(0, (int)_callout->capture_top * 2);

                OutputVector[0] = _callout->start_match;
                OutputVector[1] = _callout->current_position;

                for (var i = 2; i < OutputVector.Length; ++i)
                    OutputVector[i] = _callout->offset_vector[i];

                _oVectorInitialized = true;
            }

            return new PcreRefMatch(_subject, _regex, OutputVector, (char*)_callout->mark);
        }
    }

    /// <inheritdoc cref="PcreCallout.StartOffset"/>
    [ForwardTo8Bit]
    public readonly int StartOffset => (int)_callout->start_match;

    /// <inheritdoc cref="PcreCallout.CurrentOffset"/>
    [ForwardTo8Bit]
    public readonly int CurrentOffset => (int)_callout->current_position;

    /// <inheritdoc cref="PcreCallout.MaxCapture"/>
    [ForwardTo8Bit]
    public readonly int MaxCapture => (int)_callout->capture_top;

    /// <inheritdoc cref="PcreCallout.LastCapture"/>
    [ForwardTo8Bit]
    public readonly int LastCapture => (int)_callout->capture_last;

    /// <inheritdoc cref="PcreCallout.PatternPosition"/>
    [ForwardTo8Bit]
    public readonly int PatternPosition => (int)_callout->pattern_position;

    /// <inheritdoc cref="PcreCallout.NextPatternItemLength"/>
    [ForwardTo8Bit]
    public readonly int NextPatternItemLength => (int)_callout->next_item_length;

    /// <inheritdoc cref="PcreCallout.StringOffset"/>
    [ForwardTo8Bit]
    public readonly int StringOffset => Info.StringOffset;

    /// <inheritdoc cref="PcreCallout.String"/>
    [ForwardTo8Bit]
    [SuppressMessage("Naming", "CA1720")]
    public readonly string? String => Info.String;

    /// <inheritdoc cref="PcreCallout.Info"/>
    [ForwardTo8Bit]
    public readonly PcreCalloutInfo Info => _regex.GetCalloutInfoByPatternPosition(PatternPosition);

    /// <inheritdoc cref="PcreCallout.StartMatch"/>
    [ForwardTo8Bit]
    public readonly bool StartMatch => (_callout->callout_flags & PcreConstants.PCRE2_CALLOUT_STARTMATCH) != 0;

    /// <inheritdoc cref="PcreCallout.Backtrack"/>
    [ForwardTo8Bit]
    public readonly bool Backtrack => (_callout->callout_flags & PcreConstants.PCRE2_CALLOUT_BACKTRACK) != 0;
}
