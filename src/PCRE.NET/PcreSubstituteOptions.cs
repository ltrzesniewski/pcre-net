using System;
using PCRE.Internal;

namespace PCRE;

/// <summary>
/// Options for substitution. Those include valid <see cref="PcreMatchOptions"/>, and add substitution-specific
/// ones which all start with <c>Substitute</c>.
/// </summary>
[Flags]
public enum PcreSubstituteOptions : long
{
    /// <summary>
    /// No additional options.
    /// </summary>
    None = 0,

    /// <summary>
    /// <c>PCRE2_ANCHORED</c> - Make the start of the pattern anchored, so it can only match at the starting position.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="Anchored"/> option limits <c>pcre2_match()</c> to matching at the first matching position.
    /// If a pattern was compiled with <see cref="PcreOptions.Anchored"/>, or turned out to be anchored by virtue of its contents, it cannot be made unanchored at matching time.
    /// </para>
    /// <para>
    /// Note that setting the option at match time disables JIT matching.
    /// </para>
    /// </remarks>
    /// <seealso cref="PcreOptions.Anchored"/>
    Anchored = PcreConstants.PCRE2_ANCHORED,

    /// <summary>
    /// <c>PCRE2_ENDANCHORED</c> - Make the end of the pattern anchored, so it needs to match until the end of the subject string.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If the <see cref="EndAnchored"/> option is set, any string that <c>pcre2_match()</c> matches must be right at the end of the subject string.
    /// </para>
    /// <para>
    /// Note that setting the option at match time disables JIT matching.
    /// </para>
    /// </remarks>
    /// <seealso cref="PcreOptions.EndAnchored"/>
    EndAnchored = PcreConstants.PCRE2_ENDANCHORED,

    /// <summary>
    /// <c>PCRE2_NOTBOL</c> - Don't treat the first character of the subject string as a beginning of line.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This option specifies that first character of the subject string is not the beginning of a line, so the circumflex metacharacter should not match before it.
    /// </para>
    /// <para>
    /// Setting this without having set <see cref="PcreOptions.MultiLine"/> at compile time causes circumflex never to match.
    /// This option affects only the behaviour of the circumflex metacharacter. It does not affect <c>\A</c>.
    /// </para>
    /// </remarks>
    NotBol = PcreConstants.PCRE2_NOTBOL,

    /// <summary>
    /// <c>PCRE2_NOTEOL</c> - Don't treat the last character of the subject string as an end of line.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This option specifies that the end of the subject string is not the end of a line, so the dollar metacharacter should not match it nor (except in multiline mode) a newline immediately before it.
    /// </para>
    /// <para>
    /// Setting this without having set <see cref="PcreOptions.MultiLine"/> at compile time causes dollar never to match.
    /// This option affects only the behaviour of the dollar metacharacter. It does not affect <c>\Z</c> or <c>\z</c>.
    /// </para>
    /// </remarks>
    NotEol = PcreConstants.PCRE2_NOTEOL,

    /// <summary>
    /// <c>PCRE2_NOTEMPTY</c> - Don't consider the empty string as a valid match.
    /// </summary>
    /// <remarks>
    /// <para>
    /// An empty string is not considered to be a valid match if this option is set. If there are alternatives in the pattern, they are tried.
    /// If all the alternatives match the empty string, the entire match fails.
    /// </para>
    /// <para>
    /// For example, if the pattern <c>a?b?</c> is applied to a string not beginning with "a" or "b", it matches an empty string at the start of the subject.
    /// With <see cref="NotEmpty"/> set, this match is not valid, so <c>pcre2_match()</c> searches further into the string for occurrences of "a" or "b".
    /// </para>
    /// </remarks>
    NotEmpty = PcreConstants.PCRE2_NOTEMPTY,

    /// <summary>
    /// <c>PCRE2_NOTEMPTY_ATSTART</c> - Don't consider the empty string at the starting position as a valid match.
    /// </summary>
    /// <remarks>
    /// This is like <see cref="NotEmpty"/>, except that it locks out an empty string match only at the first matching position, that is, at the start of the subject plus the starting offset.
    /// An empty string match later in the subject is permitted. If the pattern is anchored, such a match can occur only if the pattern contains <c>\K</c>.
    /// </remarks>
    NotEmptyAtStart = PcreConstants.PCRE2_NOTEMPTY_ATSTART,

    /// <summary>
    /// <c>PCRE2_NO_UTF_CHECK</c> - Disable validation of UTF sequences in the subject string.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When <see cref="PcreOptions.Utf"/> is set at compile time, the validity of the subject as a UTF string is checked unless <see cref="NoUtfCheck"/> is passed to <c>pcre2_match()</c>
    /// or <see cref="PcreOptions.MatchInvalidUtf"/> was passed to <c>pcre2_compile()</c>. The latter special case is discussed in detail in the pcre2unicode documentation.
    /// </para>
    /// <para>
    /// In the default case, if a non-zero starting offset is given, the check is applied only to that part of the subject that could be inspected during matching,
    /// and there is a check that the starting offset points to the first code unit of a character or to the end of the subject.
    /// If there are no lookbehind assertions in the pattern, the check starts at the starting offset.
    /// Otherwise, it starts at the length of the longest lookbehind before the starting offset, or at the start of the subject if there are not that many characters before the starting offset.
    /// Note that the sequences <c>\b</c> and <c>\B</c> are one-character lookbehinds.
    /// </para>
    /// <para>
    /// The check is carried out before any other processing takes place, and a negative error code is returned if the check fails.
    /// There are several UTF error codes for each code unit width, corresponding to different problems with the code unit sequence.
    /// There are discussions about the validity of UTF-8 strings, UTF-16 strings, and UTF-32 strings in the pcre2unicode documentation.
    /// If you know that your subject is valid, and you want to skip this check for performance reasons, you can set the <see cref="NoUtfCheck"/> option when calling <c>pcre2_match()</c>.
    /// You might want to do this for the second and subsequent calls to <c>pcre2_match()</c> if you are making repeated calls to find multiple matches in the same subject string.
    /// </para>
    /// <para>
    /// Warning: Unless <see cref="PcreOptions.MatchInvalidUtf"/> was set at compile time, when <see cref="NoUtfCheck"/> is set at match time the effect of passing an invalid string as a subject,
    /// or an invalid value of startoffset, is undefined. Your program may crash or loop indefinitely or give wrong results.
    /// </para>
    /// </remarks>
    NoUtfCheck = PcreConstants.PCRE2_NO_UTF_CHECK,

    /// <summary>
    /// <c>PCRE2_NO_JIT</c> - Disable the JIT for this match and use the interpreter instead.
    /// </summary>
    /// <remarks>
    /// By default, if a pattern has been successfully processed by <c>pcre2_jit_compile()</c>, JIT is automatically used when <c>pcre2_match()</c> is called with options that JIT supports.
    /// Setting <see cref="NoJit"/> disables the use of JIT; it forces matching to be done by the interpreter.
    /// </remarks>
    NoJit = PcreConstants.PCRE2_NO_JIT,

    /// <summary>
    /// <c>PCRE2_DISABLE_RECURSELOOP_CHECK</c> - Disable the check for repeated recursion in the interpreter.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The use of recursion in patterns can lead to infinite loops. In the interpretive matcher these would be eventually caught by the match or heap limits,
    /// but this could take a long time and/or use a lot of memory if the limits are large. There is therefore a check at the start of each recursion.
    /// If the same group is still active from a previous call, and the current subject pointer is the same as it was at the start of that group, and the furthest inspected character
    /// of the subject has not changed, an error is generated.
    /// </para>
    /// <para>
    /// There are rare cases of matches that would complete, but nevertheless trigger this error. This option disables the check.
    /// It is provided mainly for testing when comparing JIT and interpretive behaviour.
    /// </para>
    /// </remarks>
    DisableRecurseLoopCheck = PcreConstants.PCRE2_DISABLE_RECURSELOOP_CHECK,

    /*
        SUBSTITUTE_OPTIONS =
            PCRE2_SUBSTITUTE_EXTENDED
            PCRE2_SUBSTITUTE_GLOBAL
            PCRE2_SUBSTITUTE_LITERAL
            PCRE2_SUBSTITUTE_MATCHED            (removed)
            PCRE2_SUBSTITUTE_OVERFLOW_LENGTH    (removed)
            PCRE2_SUBSTITUTE_REPLACEMENT_ONLY
            PCRE2_SUBSTITUTE_UNKNOWN_UNSET
            PCRE2_SUBSTITUTE_UNSET_EMPTY
    */

    /// <summary>
    /// <c>PCRE2_SUBSTITUTE_EXTENDED</c> - Add extra syntax to the replacement string.
    /// </summary>
    SubstituteExtended = PcreConstants.PCRE2_SUBSTITUTE_EXTENDED,

    /// <summary>
    /// <c>PCRE2_SUBSTITUTE_GLOBAL</c> - Replace every matching substring. By default, only the first match is replaced.
    /// </summary>
    /// <remarks>
    /// If this option is not set, only the first matching substring is replaced. The search for matches takes place in the original subject string (that is, previous replacements do not affect it).
    /// </remarks>
    SubstituteGlobal = PcreConstants.PCRE2_SUBSTITUTE_GLOBAL,

    /// <summary>
    /// <c>PCRE2_SUBSTITUTE_LITERAL</c> - Disables any interpretation of the replacement string.
    /// </summary>
    /// <remarks>
    /// This takes precedence over <see cref="SubstituteExtended"/>.
    /// </remarks>
    SubstituteLiteral = PcreConstants.PCRE2_SUBSTITUTE_LITERAL,

    /// <summary>
    /// <c>PCRE2_SUBSTITUTE_REPLACEMENT_ONLY</c> - Return only the replacement string.
    /// </summary>
    /// <remarks>
    /// By default, the whole subject is returned, with the first match replaced (unless <see cref="SubstituteGlobal"/>
    /// is used). This option will only return the first replacement, or all replacements concatenated when
    /// <see cref="SubstituteGlobal"/> is used.
    /// </remarks>
    SubstituteReplacementOnly = PcreConstants.PCRE2_SUBSTITUTE_REPLACEMENT_ONLY,

    /// <summary>
    /// <c>PCRE2_SUBSTITUTE_UNKNOWN_UNSET</c> - Causes references to capture groups that do not appear in the pattern to be treated as unset groups.
    /// </summary>
    /// <remarks>
    /// This option should be used with care, because it means that a typo in a group name or number no longer causes the <see cref="PcreErrorCode.NoSubstring"/> error.
    /// </remarks>
    SubstituteUnknownUnset = PcreConstants.PCRE2_SUBSTITUTE_UNKNOWN_UNSET,

    /// <summary>
    /// <c>PCRE2_SUBSTITUTE_UNSET_EMPTY</c> - Causes unset capture groups (including unknown groups when <see cref="SubstituteUnknownUnset"/> is set) to be treated as empty strings when inserted.
    /// </summary>
    /// <remarks>
    /// If this option is not set, an attempt to insert an unset group causes the <see cref="PcreErrorCode.Unset"/> error. This option does not influence the extended substitution syntax.
    /// </remarks>
    SubstituteUnsetEmpty = PcreConstants.PCRE2_SUBSTITUTE_UNSET_EMPTY,
}
