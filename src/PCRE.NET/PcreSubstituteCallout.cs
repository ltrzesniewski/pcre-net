using System;
using PCRE.Internal;

namespace PCRE;

/// <summary>
/// A substitution callout function.
/// </summary>
public delegate PcreSubstituteCalloutResult PcreSubstituteCalloutFunc(PcreSubstituteCallout callout);

/// <summary>
/// A callout called after a substitution.
/// </summary>
/// <seealso cref="PcreRegex.Substitute(string,string)"/>
public readonly unsafe ref struct PcreSubstituteCallout
{
    private readonly InternalRegex16Bit _regex;
    private readonly ReadOnlySpan<char> _subject;
    private readonly Native.pcre2_substitute_callout_block* _callout;

    internal PcreSubstituteCallout(InternalRegex16Bit regex,
                                   ReadOnlySpan<char> subject,
                                   Native.pcre2_substitute_callout_block* callout)
    {
        _subject = subject;
        _regex = regex;
        _callout = callout;
    }

    /// <summary>
    /// Returns the match associated with the substitution.
    /// </summary>
    public PcreRefMatch Match => new(_subject, _regex, new Span<nuint>(_callout->ovector, 2 * (int)_callout->oveccount), null);

    /// <summary>
    /// The subject string.
    /// </summary>
    public ReadOnlySpan<char> Subject => _subject;

    /// <summary>
    /// The output so far.
    /// </summary>
    public ReadOnlySpan<char> Output => new(_callout->output, (int)_callout->output_offset_end);

    /// <summary>
    /// The current substitution result.
    /// </summary>
    public ReadOnlySpan<char> Substitution => new((char*)_callout->output + (int)_callout->output_offset_start, (int)_callout->output_offset_end - (int)_callout->output_offset_start);

    /// <summary>
    /// The total substitution count. It is 1 for the first callout, 2 for the second, and so on.
    /// </summary>
    public int SubstitutionCount => (int)_callout->subscount;
}
