using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using PCRE.Internal;

namespace PCRE.Dfa;

/// <summary>
/// DFA (deterministic finite automaton) matching API.
/// </summary>
/// <remarks>
/// This is a matching algorithm that scans the subject string just once (not counting lookaround assertions), and does not backtrack.
/// This has different characteristics to the normal algorithm, and is not compatible with Perl. Some of the features of PCRE2 patterns are not supported.
/// </remarks>
public sealed class PcreDfaRegex
{
    private readonly InternalRegex16Bit _regex;

    internal PcreDfaRegex(InternalRegex16Bit regex)
    {
        _regex = regex;
    }

    /// <include file='../PcreRegex.xml' path='/doc/method[@name="DfaMatch"]/*'/>
    /// <include file='../PcreRegex.xml' path='/doc/param[@name="subject"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="dfaMatch"]/*'/>
    /// </remarks>
    [Pure]
    public PcreDfaMatchResult Match(string subject)
        => Match(subject, 0, PcreDfaMatchOptions.None);

    /// <include file='../PcreRegex.xml' path='/doc/method[@name="DfaMatch"]/*'/>
    /// <include file='../PcreRegex.xml' path='/doc/param[@name="subject" or @name="options"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="dfaMatch"]/*'/>
    /// </remarks>
    [Pure]
    public PcreDfaMatchResult Match(string subject, PcreDfaMatchOptions options)
        => Match(subject, 0, options);

    /// <include file='../PcreRegex.xml' path='/doc/method[@name="DfaMatch"]/*'/>
    /// <include file='../PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="dfaMatch" or @name="startIndex"]/*'/>
    /// </remarks>
    [Pure]
    public PcreDfaMatchResult Match(string subject, int startIndex)
        => Match(subject, startIndex, PcreDfaMatchOptions.None);

    /// <include file='../PcreRegex.xml' path='/doc/method[@name="DfaMatch"]/*'/>
    /// <include file='../PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex" or @name="options"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="dfaMatch" or @name="startIndex"]/*'/>
    /// </remarks>
    [Pure]
    public PcreDfaMatchResult Match(string subject, int startIndex, PcreDfaMatchOptions options)
        => Match(subject, PcreDfaMatchSettings.GetSettings(startIndex, options));

    /// <include file='../PcreRegex.xml' path='/doc/method[@name="DfaMatch"]/*'/>
    /// <include file='../PcreRegex.xml' path='/doc/param[@name="subject" or @name="settings"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="dfaMatch"]/*'/>
    /// </remarks>
    public PcreDfaMatchResult Match(string subject, PcreDfaMatchSettings settings)
    {
        if (subject == null)
            throw new ArgumentNullException(nameof(subject));

        if (settings == null)
            throw new ArgumentNullException(nameof(settings));

        if (settings.StartIndex < 0 || settings.StartIndex > subject.Length)
            throw new ArgumentException("Invalid start index.");

        return _regex.DfaMatch(subject, settings, settings.StartIndex, ((PcreMatchOptions)settings.AdditionalOptions).ToPatternOptions());
    }

    /// <include file='../PcreRegex.xml' path='/doc/method[@name="DfaMatches"]/*'/>
    /// <include file='../PcreRegex.xml' path='/doc/param[@name="subject"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="dfaMatches"]/*'/>
    /// </remarks>
    [Pure]
    public IEnumerable<PcreDfaMatchResult> Matches(string subject)
        => Matches(subject, 0);

    /// <include file='../PcreRegex.xml' path='/doc/method[@name="DfaMatches"]/*'/>
    /// <include file='../PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="dfaMatches" or @name="startIndex"]/*'/>
    /// </remarks>
    [Pure]
    public IEnumerable<PcreDfaMatchResult> Matches(string subject, int startIndex)
        => Matches(subject, PcreDfaMatchSettings.GetSettings(startIndex, PcreDfaMatchOptions.None));

    /// <include file='../PcreRegex.xml' path='/doc/method[@name="DfaMatches"]/*'/>
    /// <include file='../PcreRegex.xml' path='/doc/param[@name="subject" or @name="settings"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="dfaMatches"]/*'/>
    /// </remarks>
    [Pure]
    public IEnumerable<PcreDfaMatchResult> Matches(string subject, PcreDfaMatchSettings settings)
    {
        if (subject == null)
            throw new ArgumentNullException(nameof(subject));

        if (settings == null)
            throw new ArgumentNullException(nameof(settings));

        if (settings.StartIndex < 0 || settings.StartIndex > subject.Length)
            throw new ArgumentException("Invalid start index.");

        return MatchesIterator(subject, settings);
    }

    private IEnumerable<PcreDfaMatchResult> MatchesIterator(string subject, PcreDfaMatchSettings settings)
    {
        var additionalOptions = ((PcreMatchOptions)settings.AdditionalOptions).ToPatternOptions();

        var match = _regex.DfaMatch(subject, settings, settings.StartIndex, additionalOptions);
        if (!match.Success)
            yield break;

        yield return match;

        additionalOptions |= PcreConstants.PCRE2_NO_UTF_CHECK;

        while (true)
        {
            var nextIndex = match.Index + 1;

            if (nextIndex > subject.Length)
                yield break;

            if (nextIndex < subject.Length && char.IsLowSurrogate(subject[nextIndex]))
                ++nextIndex;

            match = _regex.DfaMatch(subject, settings, nextIndex, additionalOptions);
            if (!match.Success)
                yield break;

            yield return match;
        }
    }
}
