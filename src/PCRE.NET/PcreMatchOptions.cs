using System;
using PCRE.Internal;

namespace PCRE
{
    [Flags]
    public enum PcreMatchOptions : long
    {
        None = 0,

        /// <summary>
        /// <c>PCRE2_ANCHORED</c> - The PCRE2_ANCHORED option limits pcre2_match() to matching at the first matching position. If a pattern was compiled with PCRE2_ANCHORED, or turned out to be anchored by virtue of its contents, it cannot be made unachored at matching time. Note that setting the option at match time disables JIT matching.
        /// </summary>
        Anchored = PcreConstants.ANCHORED,

        /// <summary>
        /// <c>PCRE2_NOTBOL</c> - This option specifies that first character of the subject string is not the beginning of a line, so the circumflex metacharacter should not match before it. Setting this without having set PCRE2_MULTILINE at compile time causes circumflex never to match. This option affects only the behaviour of the circumflex metacharacter. It does not affect \A.
        /// </summary>
        NotBol = PcreConstants.NOTBOL,

        /// <summary>
        /// <c>PCRE2_NOTEOL</c> - This option specifies that the end of the subject string is not the end of a line, so the dollar metacharacter should not match it nor (except in multiline mode) a newline immediately before it. Setting this without having set PCRE2_MULTILINE at compile time causes dollar never to match. This option affects only the behaviour of the dollar metacharacter. It does not affect \Z or \z.
        /// </summary>
        NotEol = PcreConstants.NOTEOL,

        /// <summary>
        /// <c>PCRE2_NOTEMPTY</c> - An empty string is not considered to be a valid match if this option is set. If there are alternatives in the pattern, they are tried. If all the alternatives match the empty string, the entire match fails.
        /// </summary>
        /// <remarks>
        /// For example, if the pattern
        /// a?b?
        /// is applied to a string not beginning with "a" or "b", it matches an empty string at the start of the subject. With PCRE2_NOTEMPTY set, this match is not valid, so pcre2_match() searches further into the string for occurrences of "a" or "b".
        /// </remarks>
        NotEmpty = PcreConstants.NOTEMPTY,

        /// <summary>
        /// <c>PCRE2_NOTEMPTY_ATSTART</c> - This is like PCRE2_NOTEMPTY, except that it locks out an empty string match only at the first matching position, that is, at the start of the subject plus the starting offset. An empty string match later in the subject is permitted. If the pattern is anchored, such a match can occur only if the pattern contains \K.
        /// </summary>
        NotEmptyAtStart = PcreConstants.NOTEMPTY_ATSTART,

        /// <summary>
        /// <c>PCRE2_NO_START_OPTIMIZE</c> - This is an option whose main effect is at matching time. It does not change what pcre2_compile() generates, but it does affect the output of the JIT compiler.
        /// </summary>
        /// <remarks>
        /// There are a number of optimizations that may occur at the start of a match, in order to speed up the process. For example, if it is known that an unanchored match must start with a specific code unit value, the matching code searches the subject for that value, and fails immediately if it cannot find it, without actually running the main matching function. This means that a special item such as (*COMMIT) at the start of a pattern is not considered until after a suitable starting point for the match has been found. Also, when callouts or (*MARK) items are in use, these "start-up" optimizations can cause them to be skipped if the pattern is never actually used. The start-up optimizations are in effect a pre-scan of the subject that takes place before the pattern is run.
        /// The PCRE2_NO_START_OPTIMIZE option disables the start-up optimizations, possibly causing performance to suffer, but ensuring that in cases where the result is "no match", the callouts do occur, and that items such as (*COMMIT) and (*MARK) are considered at every possible starting position in the subject string.
        /// Setting PCRE2_NO_START_OPTIMIZE may change the outcome of a matching operation. Consider the pattern
        ///   (*COMMIT)ABC
        /// When this is compiled, PCRE2 records the fact that a match must start with the character "A". Suppose the subject string is "DEFABC". The start-up optimization scans along the subject, finds "A" and runs the first match attempt from there. The (*COMMIT) item means that the pattern must match the current starting position, which in this case, it does. However, if the same match is run with PCRE2_NO_START_OPTIMIZE set, the initial scan along the subject string does not happen. The first match attempt is run starting from "D" and when this fails, (*COMMIT) prevents any further matches being tried, so the overall result is "no match".
        /// As another start-up optimization makes use of a minimum length for a matching subject, which is recorded when possible. Consider the pattern
        ///   (*MARK:1)B(*MARK:2)(X|Y)
        /// The minimum length for a match is two characters. If the subject is "XXBB", the "starting character" optimization skips "XX", then tries to match "BB", which is long enough. In the process, (*MARK:2) is encountered and remembered. When the match attempt fails, the next "B" is found, but there is only one character left, so there are no more attempts, and "no match" is returned with the "last mark seen" set to "2". If NO_START_OPTIMIZE is set, however, matches are tried at every possible starting position, including at the end of the subject, where (*MARK:1) is encountered, but there is no "B", so the "last mark seen" that is returned is "1". In this case, the optimizations do not affect the overall match result, which is still "no match", but they do affect the auxiliary information that is returned.
        /// </remarks>
        NoStartOptimize = PcreConstants.NO_START_OPTIMIZE,

        /// <summary>
        /// <c>PCRE2_PARTIAL_SOFT</c> - These options turn on the partial matching feature. A partial match occurs if the end of the subject string is reached successfully, but there are not enough subject characters to complete the match. In addition, either at least one character must have been inspected or the pattern must contain a lookbehind, or the pattern must be one that could match an empty string.
        /// </summary>
        /// <remarks>
        /// If this situation arises when PCRE2_PARTIAL_SOFT (but not PCRE2_PARTIAL_HARD) is set, matching continues by testing any remaining alternatives. Only if no complete match can be found is PCRE2_ERROR_PARTIAL returned instead of PCRE2_ERROR_NOMATCH. In other words, PCRE2_PARTIAL_SOFT specifies that the caller is prepared to handle a partial match, but only if no complete match can be found.
        /// If PCRE2_PARTIAL_HARD is set, it overrides PCRE2_PARTIAL_SOFT. In this case, if a partial match is found, pcre2_match() immediately returns PCRE2_ERROR_PARTIAL, without considering any other alternatives. In other words, when PCRE2_PARTIAL_HARD is set, a partial match is considered to be more important that an alternative complete match.
        /// There is a more detailed discussion of partial and multi-segment matching, with examples, in the pcre2partial documentation.
        /// </remarks>
        PartialSoft = PcreConstants.PARTIAL_SOFT,

        /// <summary>
        /// <c>PCRE2_PARTIAL_HARD</c> - These options turn on the partial matching feature. A partial match occurs if the end of the subject string is reached successfully, but there are not enough subject characters to complete the match. In addition, either at least one character must have been inspected or the pattern must contain a lookbehind, or the pattern must be one that could match an empty string.
        /// </summary>
        /// <remarks>
        /// If this situation arises when PCRE2_PARTIAL_SOFT (but not PCRE2_PARTIAL_HARD) is set, matching continues by testing any remaining alternatives. Only if no complete match can be found is PCRE2_ERROR_PARTIAL returned instead of PCRE2_ERROR_NOMATCH. In other words, PCRE2_PARTIAL_SOFT specifies that the caller is prepared to handle a partial match, but only if no complete match can be found.
        /// If PCRE2_PARTIAL_HARD is set, it overrides PCRE2_PARTIAL_SOFT. In this case, if a partial match is found, pcre2_match() immediately returns PCRE2_ERROR_PARTIAL, without considering any other alternatives. In other words, when PCRE2_PARTIAL_HARD is set, a partial match is considered to be more important that an alternative complete match.
        /// There is a more detailed discussion of partial and multi-segment matching, with examples, in the pcre2partial documentation.
        /// </remarks>
        PartialHard = PcreConstants.PARTIAL_HARD,

        /// <summary>
        /// <c>PCRE2_NO_UTF_CHECK</c> - When PCRE2_UTF is set, the validity of the pattern as a UTF string is automatically checked. There are discussions about the validity of UTF-8 strings, UTF-16 strings, and UTF-32 strings in the pcre2unicode document. If an invalid UTF sequence is found, pcre2_compile() returns a negative error code.
        /// If you know that your pattern is a valid UTF string, and you want to skip this check for performance reasons, you can set the PCRE2_NO_UTF_CHECK option. When it is set, the effect of passing an invalid UTF string as a pattern is undefined. It may cause your program to crash or loop.
        /// </summary>
        /// <remarks>
        /// Note that this option can also be passed to pcre2_match() and pcre_dfa_match(), to suppress UTF validity checking of the subject string.
        /// Note also that setting PCRE2_NO_UTF_CHECK at compile time does not disable the error that is given if an escape sequence for an invalid Unicode code point is encountered in the pattern. In particular, the so-called "surrogate" code points (0xd800 to 0xdfff) are invalid. If you want to allow escape sequences such as \x{d800} you can set the PCRE2_EXTRA_ALLOW_SURROGATE_ESCAPES extra option, as described in the section entitled "Extra compile options" below. However, this is possible only in UTF-8 and UTF-32 modes, because these values are not representable in UTF-16.
        /// </remarks>
        NoUtfCheck = PcreConstants.NO_UTF_CHECK,

        /// <summary>
        /// <c>PCRE2_NO_JIT</c> - By default, if a pattern has been successfully processed by pcre2_jit_compile(), JIT is automatically used when pcre2_match() is called with options that JIT supports. Setting PCRE2_NO_JIT disables the use of JIT; it forces matching to be done by the interpreter.
        /// </summary>
        NoJit = PcreConstants.NO_JIT,

        /// <summary>
        /// <c>PCRE2_COPY_MATCHED_SUBJECT</c> - By default, a pointer to the subject is remembered in the match data block so that, after a successful match, it can be referenced by the substring extraction functions. This means that the subject's memory must not be freed until all such operations are complete. For some applications where the lifetime of the subject string is not guaranteed, it may be necessary to make a copy of the subject string, but it is wasteful to do this unless the match is successful. After a successful match, if PCRE2_COPY_MATCHED_SUBJECT is set, the subject is copied and the new pointer is remembered in the match data block instead of the original subject pointer. The memory allocator that was used for the match block itself is used. The copy is automatically freed when pcre2_match_data_free() is called to free the match data block. It is also automatically freed if the match data block is re-used for another match operation.
        /// </summary>
        CopyMatchedSubject = PcreConstants.COPY_MATCHED_SUBJECT
    }
}
