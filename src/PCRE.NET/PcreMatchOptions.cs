using System;
using PCRE.Internal;

namespace PCRE
{
    /// <summary>
    /// Options for NFA matching.
    /// </summary>
    [Flags]
    public enum PcreMatchOptions : long
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
        /// If a pattern was compiled with <see cref="PcreOptions.Anchored"/>, or turned out to be anchored by virtue of its contents, it cannot be made unachored at matching time.
        /// </para>
        /// <para>
        /// Note that setting the option at match time disables JIT matching.
        /// </para>
        /// </remarks>
        /// <seealso cref="PcreOptions.Anchored"/>
        Anchored = PcreConstants.ANCHORED,

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
        EndAnchored = PcreConstants.ENDANCHORED,

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
        NotBol = PcreConstants.NOTBOL,

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
        NotEol = PcreConstants.NOTEOL,

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
        NotEmpty = PcreConstants.NOTEMPTY,

        /// <summary>
        /// <c>PCRE2_NOTEMPTY_ATSTART</c> - Don't consider the empty string at the starting position as a valid match.
        /// </summary>
        /// <remarks>
        /// This is like <see cref="NotEmpty"/>, except that it locks out an empty string match only at the first matching position, that is, at the start of the subject plus the starting offset.
        /// An empty string match later in the subject is permitted. If the pattern is anchored, such a match can occur only if the pattern contains <c>\K</c>.
        /// </remarks>
        NotEmptyAtStart = PcreConstants.NOTEMPTY_ATSTART,

        /// <summary>
        /// <c>PCRE2_PARTIAL_SOFT</c> - Enable partial matching mode. Still try to find a complete match if a partial match is found first.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A partial match occurs if the end of the subject string is reached successfully, but there are not enough subject characters to complete the match.
        /// In addition, either at least one character must have been inspected or the pattern must contain a lookbehind, or the pattern must be one that could match an empty string.
        /// </para>
        /// <para>
        /// If this situation arises when <see cref="PartialSoft"/> (but not <see cref="PartialHard"/>) is set, matching continues by testing any remaining alternatives.
        /// Only if no complete match can be found is <see cref="PcreErrorCode.Partial"/> returned instead of <see cref="PcreErrorCode.NoMatch"/>.
        /// In other words, <see cref="PartialSoft"/> specifies that the caller is prepared to handle a partial match, but only if no complete match can be found.
        /// </para>
        /// <para>
        /// If <see cref="PartialHard"/> is set, it overrides <see cref="PartialSoft"/>. In this case, if a partial match is found, <c>pcre2_match()</c> immediately returns <see cref="PcreErrorCode.Partial"/>,
        /// without considering any other alternatives. In other words, when <see cref="PartialHard"/> is set, a partial match is considered to be more important that an alternative complete match.
        /// </para>
        /// <para>
        /// There is a more detailed discussion of partial and multi-segment matching, with examples, in the pcre2partial documentation.
        /// </para>
        /// </remarks>
        PartialSoft = PcreConstants.PARTIAL_SOFT,

        /// <summary>
        /// <c>PCRE2_PARTIAL_HARD</c> - Enable partial matching mode. Stop looking for a complete match if a partial match is found first.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A partial match occurs if the end of the subject string is reached successfully, but there are not enough subject characters to complete the match.
        /// In addition, either at least one character must have been inspected or the pattern must contain a lookbehind, or the pattern must be one that could match an empty string.
        /// </para>
        /// <para>
        /// If this situation arises when <see cref="PartialSoft"/> (but not <see cref="PartialHard"/>) is set, matching continues by testing any remaining alternatives.
        /// Only if no complete match can be found is <see cref="PcreErrorCode.Partial"/> returned instead of <see cref="PcreErrorCode.NoMatch"/>.
        /// In other words, <see cref="PartialSoft"/> specifies that the caller is prepared to handle a partial match, but only if no complete match can be found.
        /// </para>
        /// <para>
        /// If <see cref="PartialHard"/> is set, it overrides <see cref="PartialSoft"/>. In this case, if a partial match is found, <c>pcre2_match()</c> immediately returns <see cref="PcreErrorCode.Partial"/>,
        /// without considering any other alternatives. In other words, when <see cref="PartialHard"/> is set, a partial match is considered to be more important that an alternative complete match.
        /// </para>
        /// <para>
        /// There is a more detailed discussion of partial and multi-segment matching, with examples, in the pcre2partial documentation.
        /// </para>
        /// </remarks>
        PartialHard = PcreConstants.PARTIAL_HARD,

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
        NoUtfCheck = PcreConstants.NO_UTF_CHECK,

        /// <summary>
        /// <c>PCRE2_NO_JIT</c> - Disable the JIT for this match and use the interpreter instead.
        /// </summary>
        /// <remarks>
        /// By default, if a pattern has been successfully processed by <c>pcre2_jit_compile()</c>, JIT is automatically used when <c>pcre2_match()</c> is called with options that JIT supports.
        /// Setting <see cref="NoJit"/> disables the use of JIT; it forces matching to be done by the interpreter.
        /// </remarks>
        NoJit = PcreConstants.NO_JIT,
    }
}
