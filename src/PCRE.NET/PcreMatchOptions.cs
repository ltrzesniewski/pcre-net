using System;
using PCRE.Internal;

namespace PCRE
{
    [Flags]
    public enum PcreMatchOptions : long
    {
        None = 0,

        /// <summary>
        /// <c>PCRE2_ANCHORED</c> - Make the start of the pattern anchored, so it can only match at the starting position.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The <c>PCRE2_ANCHORED</c> option limits <c>pcre2_match()</c> to matching at the first matching position.
        /// If a pattern was compiled with <c>PCRE2_ANCHORED</c>, or turned out to be anchored by virtue of its contents, it cannot be made unachored at matching time.
        /// </para>
        /// <para>
        /// Note that setting the option at match time disables JIT matching.
        /// </para>
        /// </remarks>
        /// <seealso cref="PcreOptions.Anchored"/>
        Anchored = PcreConstants.ANCHORED,

        /// <inheritdoc cref="PcreOptions.NotBol"/>
        NotBol = PcreConstants.NOTBOL,

        /// <inheritdoc cref="PcreOptions.NotEol"/>
        NotEol = PcreConstants.NOTEOL,

        /// <inheritdoc cref="PcreOptions.NotEmpty"/>
        NotEmpty = PcreConstants.NOTEMPTY,

        /// <inheritdoc cref="PcreOptions.NotEmptyAtStart"/>
        NotEmptyAtStart = PcreConstants.NOTEMPTY_ATSTART,

        /// <inheritdoc cref="PcreOptions.NoStartOptimize"/>
        NoStartOptimize = PcreConstants.NO_START_OPTIMIZE,

        /// <summary>
        /// <c>PCRE2_PARTIAL_SOFT</c> - Enable partial matching mode. Still try to find a complete match if a partial match is found first.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A partial match occurs if the end of the subject string is reached successfully, but there are not enough subject characters to complete the match.
        /// In addition, either at least one character must have been inspected or the pattern must contain a lookbehind, or the pattern must be one that could match an empty string.
        /// </para>
        /// <para>
        /// If this situation arises when <c>PCRE2_PARTIAL_SOFT</c> (but not <c>PCRE2_PARTIAL_HARD</c>) is set, matching continues by testing any remaining alternatives.
        /// Only if no complete match can be found is <c>PCRE2_ERROR_PARTIAL</c> returned instead of <c>PCRE2_ERROR_NOMATCH</c>.
        /// In other words, <c>PCRE2_PARTIAL_SOFT</c> specifies that the caller is prepared to handle a partial match, but only if no complete match can be found.
        /// </para>
        /// <para>
        /// If <c>PCRE2_PARTIAL_HARD</c> is set, it overrides <c>PCRE2_PARTIAL_SOFT</c>. In this case, if a partial match is found, <c>pcre2_match()</c> immediately returns <c>PCRE2_ERROR_PARTIAL</c>,
        /// without considering any other alternatives. In other words, when <c>PCRE2_PARTIAL_HARD</c> is set, a partial match is considered to be more important that an alternative complete match.
        /// </para>
        /// <para>
        /// There is a more detailed discussion of partial and multi-segment matching, with examples, in the pcre2partial documentation.
        /// </para>
        /// </remarks>
        PartialSoft = PcreConstants.PARTIAL_SOFT,

        /// <summary>
        /// <c>PCRE2_PARTIAL_SOFT</c> - Enable partial matching mode. Stop looking for a complete match if a partial match is found first.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A partial match occurs if the end of the subject string is reached successfully, but there are not enough subject characters to complete the match.
        /// In addition, either at least one character must have been inspected or the pattern must contain a lookbehind, or the pattern must be one that could match an empty string.
        /// </para>
        /// <para>
        /// If this situation arises when <c>PCRE2_PARTIAL_SOFT</c> (but not <c>PCRE2_PARTIAL_HARD</c>) is set, matching continues by testing any remaining alternatives.
        /// Only if no complete match can be found is <c>PCRE2_ERROR_PARTIAL</c> returned instead of <c>PCRE2_ERROR_NOMATCH</c>.
        /// In other words, <c>PCRE2_PARTIAL_SOFT</c> specifies that the caller is prepared to handle a partial match, but only if no complete match can be found.
        /// </para>
        /// <para>
        /// If <c>PCRE2_PARTIAL_HARD</c> is set, it overrides <c>PCRE2_PARTIAL_SOFT</c>. In this case, if a partial match is found, <c>pcre2_match()</c> immediately returns <c>PCRE2_ERROR_PARTIAL</c>,
        /// without considering any other alternatives. In other words, when <c>PCRE2_PARTIAL_HARD</c> is set, a partial match is considered to be more important that an alternative complete match.
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
        /// When <c>PCRE2_UTF</c> is set at compile time, the validity of the subject as a UTF string is checked unless <c>PCRE2_NO_UTF_CHECK</c> is passed to <c>pcre2_match()</c>
        /// or <c>PCRE2_MATCH_INVALID_UTF</c> was passed to <c>pcre2_compile()</c>. The latter special case is discussed in detail in the pcre2unicode documentation.
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
        /// If you know that your subject is valid, and you want to skip this check for performance reasons, you can set the <c>PCRE2_NO_UTF_CHECK</c> option when calling <c>pcre2_match()</c>.
        /// You might want to do this for the second and subsequent calls to <c>pcre2_match()</c> if you are making repeated calls to find multiple matches in the same subject string.
        /// </para>
        /// <para>
        /// Warning: Unless <c>PCRE2_MATCH_INVALID_UTF</c> was set at compile time, when <c>PCRE2_NO_UTF_CHECK</c> is set at match time the effect of passing an invalid string as a subject,
        /// or an invalid value of startoffset, is undefined. Your program may crash or loop indefinitely or give wrong results.
        /// </para>
        /// </remarks>
        NoUtfCheck = PcreConstants.NO_UTF_CHECK,

        /// <summary>
        /// <c>PCRE2_NO_JIT</c> - Disable the JIT for this match and use the interpreter instead.
        /// </summary>
        /// <remarks>
        /// By default, if a pattern has been successfully processed by <c>pcre2_jit_compile()</c>, JIT is automatically used when <c>pcre2_match()</c> is called with options that JIT supports.
        /// Setting <c>PCRE2_NO_JIT</c> disables the use of JIT; it forces matching to be done by the interpreter.
        /// </remarks>
        NoJit = PcreConstants.NO_JIT,
    }
}
