using System;
using PCRE.Internal;

namespace PCRE.Dfa;

/// <summary>
/// Options for DFA matching.
/// </summary>
[Flags]
public enum PcreDfaMatchOptions : long
{
    /// <summary>
    /// No additional options.
    /// </summary>
    None = 0,

    /// <inheritdoc cref="PcreMatchOptions.Anchored"/>
    Anchored = PcreConstants.PCRE2_ANCHORED,

    /// <inheritdoc cref="PcreMatchOptions.EndAnchored"/>
    EndAnchored = PcreConstants.PCRE2_ENDANCHORED,

    /// <inheritdoc cref="PcreMatchOptions.NotBol"/>
    NotBol = PcreConstants.PCRE2_NOTBOL,

    /// <inheritdoc cref="PcreMatchOptions.NotEol"/>
    NotEol = PcreConstants.PCRE2_NOTEOL,

    /// <inheritdoc cref="PcreMatchOptions.NotEmpty"/>
    NotEmpty = PcreConstants.PCRE2_NOTEMPTY,

    /// <inheritdoc cref="PcreMatchOptions.NotEmptyAtStart"/>
    NotEmptyAtStart = PcreConstants.PCRE2_NOTEMPTY_ATSTART,

    /// <inheritdoc cref="PcreMatchOptions.NoUtfCheck"/>
    NoUtfCheck = PcreConstants.PCRE2_NO_UTF_CHECK,

    /// <summary>
    /// <c>PCRE2_PARTIAL_SOFT</c> - Enable partial matching mode. Still try to find a complete match if a partial match is found first.
    /// </summary>
    /// <remarks>
    /// <para>
    /// These have the same general effect as they do for <c>pcre2_match()</c>, but the details are slightly different.
    /// </para>
    /// <para>
    /// When <see cref="PartialHard"/> is set for <c>pcre2_dfa_match()</c>, it returns <see cref="PcreErrorCode.Partial"/> if the end of the subject is reached and there
    /// is still at least one matching possibility that requires additional characters.
    /// This happens even if some complete matches have already been found.
    /// </para>
    /// <para>
    /// When <see cref="PartialSoft"/> is set, the return code <see cref="PcreErrorCode.NoMatch"/> is converted into <see cref="PcreErrorCode.Partial"/> if the end of the subject is reached,
    /// there have been no complete matches, but there is still at least one matching possibility.
    /// </para>
    /// <para>
    /// The portion of the string that was inspected when the longest partial match was found is set as the first matching string in both cases.
    /// There is a more detailed discussion of partial and multi-segment matching, with examples, in the pcre2partial documentation.
    /// </para>
    /// </remarks>
    /// <see cref="PcreMatchOptions.PartialSoft"/>
    PartialSoft = PcreConstants.PCRE2_PARTIAL_SOFT,

    /// <summary>
    /// <c>PCRE2_PARTIAL_HARD</c> - Enable partial matching mode. Stop looking for a complete match if a partial match is found first.
    /// </summary>
    /// <remarks>
    /// <para>
    /// These have the same general effect as they do for <c>pcre2_match()</c>, but the details are slightly different.
    /// </para>
    /// <para>
    /// When <see cref="PartialHard"/> is set for <c>pcre2_dfa_match()</c>, it returns <see cref="PcreErrorCode.Partial"/> if the end of the subject is reached and there
    /// is still at least one matching possibility that requires additional characters.
    /// This happens even if some complete matches have already been found.
    /// </para>
    /// <para>
    /// When <see cref="PartialSoft"/> is set, the return code <see cref="PcreErrorCode.NoMatch"/> is converted into <see cref="PcreErrorCode.Partial"/> if the end of the subject is reached,
    /// there have been no complete matches, but there is still at least one matching possibility.
    /// </para>
    /// <para>
    /// The portion of the string that was inspected when the longest partial match was found is set as the first matching string in both cases.
    /// There is a more detailed discussion of partial and multi-segment matching, with examples, in the pcre2partial documentation.
    /// </para>
    /// </remarks>
    /// <see cref="PcreMatchOptions.PartialHard"/>
    PartialHard = PcreConstants.PCRE2_PARTIAL_HARD,

    /// <summary>
    /// <c>PCRE2_DFA_SHORTEST</c> - Stop at the first (and therefore shortest) match.
    /// </summary>
    /// <remarks>
    /// Setting the <see cref="DfaShortest"/> option causes the matching algorithm to stop as soon as it has found one match.
    /// Because of the way the alternative algorithm works, this is necessarily the shortest possible match at the first possible matching point in the subject string.
    /// </remarks>
    DfaShortest = PcreConstants.PCRE2_DFA_SHORTEST
}
