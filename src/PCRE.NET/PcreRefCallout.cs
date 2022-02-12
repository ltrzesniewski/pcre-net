using System;
using System.Diagnostics.CodeAnalysis;
using PCRE.Internal;

namespace PCRE;

/// <summary>
/// A callout function.
/// </summary>
public delegate PcreCalloutResult PcreRefCalloutFunc(PcreRefCallout callout);

/// <inheritdoc cref="PcreCallout"/>
public unsafe ref struct PcreRefCallout
{
    private readonly ReadOnlySpan<char> _subject;
    private readonly InternalRegex _regex;
    private readonly Native.pcre2_callout_block* _callout;

    internal Span<nuint> OutputVector;
    private bool _oVectorInitialized;

    internal PcreRefCallout(ReadOnlySpan<char> subject, InternalRegex regex, Native.pcre2_callout_block* callout, Span<nuint> outputVector)
    {
        _subject = subject;
        _regex = regex;
        _callout = callout;
        OutputVector = outputVector;
        _oVectorInitialized = false;
    }

    /// <inheritdoc cref="PcreCallout.Number"/>
    public readonly int Number => (int)_callout->callout_number;

    /// <inheritdoc cref="PcreCallout.Match"/>
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

            return new PcreRefMatch(_subject, _regex, OutputVector, _callout->mark);
        }
    }

    /// <inheritdoc cref="PcreCallout.StartOffset"/>
    public readonly int StartOffset => (int)_callout->start_match;

    /// <inheritdoc cref="PcreCallout.CurrentOffset"/>
    public readonly int CurrentOffset => (int)_callout->current_position;

    /// <inheritdoc cref="PcreCallout.MaxCapture"/>
    public readonly int MaxCapture => (int)_callout->capture_top;

    /// <inheritdoc cref="PcreCallout.LastCapture"/>
    public readonly int LastCapture => (int)_callout->capture_last;

    /// <inheritdoc cref="PcreCallout.PatternPosition"/>
    public readonly int PatternPosition => (int)_callout->pattern_position;

    /// <inheritdoc cref="PcreCallout.NextPatternItemLength"/>
    public readonly int NextPatternItemLength => (int)_callout->next_item_length;

    /// <inheritdoc cref="PcreCallout.StringOffset"/>
    public readonly int StringOffset => Info.StringOffset;

    /// <inheritdoc cref="PcreCallout.String"/>
    [SuppressMessage("Naming", "CA1720")]
    public readonly string? String => Info.String;

    /// <inheritdoc cref="PcreCallout.Info"/>
    public readonly PcreCalloutInfo Info => _regex.GetCalloutInfoByPatternPosition(PatternPosition);

    /// <inheritdoc cref="PcreCallout.StartMatch"/>
    public readonly bool StartMatch => (_callout->callout_flags & PcreConstants.CALLOUT_STARTMATCH) != 0;

    /// <inheritdoc cref="PcreCallout.Backtrack"/>
    public readonly bool Backtrack => (_callout->callout_flags & PcreConstants.CALLOUT_BACKTRACK) != 0;
}
