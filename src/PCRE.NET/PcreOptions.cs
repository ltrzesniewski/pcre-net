using System;
using System.Diagnostics.CodeAnalysis;
using PCRE.Internal;

namespace PCRE
{
    [Flags]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("Microsoft.Design", "CA1069")]
    public enum PcreOptions : long
    {
        None = 0,

        // Normal options

        /// <inheritdoc cref="Caseless"/>
        IgnoreCase = PcreConstants.CASELESS,

        /// <summary>
        /// <c>PCRE2_CASELESS</c> - If this bit is set, letters in the pattern match both upper and lower case letters in the subject. It is equivalent to Perl's /i option, and it can be changed within a pattern by a (?i) option setting. If either PCRE2_UTF or PCRE2_UCP is set, Unicode properties are used for all characters with more than one other case, and for all characters whose code points are greater than U+007F. Note that there are two ASCII characters, K and S, that, in addition to their lower case ASCII equivalents, are case-equivalent with U+212A (Kelvin sign) and U+017F (long S) respectively. For lower valued characters with only one other case, a lookup table is used for speed. When neither PCRE2_UTF nor PCRE2_UCP is set, a lookup table is used for all code points less than 256, and higher code points (available only in 16-bit or 32-bit mode) are treated as not having another case.
        /// </summary>
        Caseless = PcreConstants.CASELESS,

        /// <summary>
        /// <c>PCRE2_MULTILINE</c> - By default, for the purposes of matching "start of line" and "end of line", PCRE2 treats the subject string as consisting of a single line of characters, even if it actually contains newlines. The "start of line" metacharacter (^) matches only at the start of the string, and the "end of line" metacharacter ($) matches only at the end of the string, or before a terminating newline (except when PCRE2_DOLLAR_ENDONLY is set). Note, however, that unless PCRE2_DOTALL is set, the "any character" metacharacter (.) does not match at a newline. This behaviour (for ^, $, and dot) is the same as Perl.
        /// </summary>
        /// <remarks>
        /// When PCRE2_MULTILINE it is set, the "start of line" and "end of line" constructs match immediately following or immediately before internal newlines in the subject string, respectively, as well as at the very start and end. This is equivalent to Perl's /m option, and it can be changed within a pattern by a (?m) option setting. Note that the "start of line" metacharacter does not match after a newline at the end of the subject, for compatibility with Perl. However, you can change this by setting the PCRE2_ALT_CIRCUMFLEX option. If there are no newlines in a subject string, or no occurrences of ^ or $ in a pattern, setting PCRE2_MULTILINE has no effect.
        /// </remarks>
        MultiLine = PcreConstants.MULTILINE,

        /// <inheritdoc cref="DotAll"/>
        Singleline = PcreConstants.DOTALL,

        /// <summary>
        /// <c>PCRE2_DOTALL</c> - If this bit is set, a dot metacharacter in the pattern matches any character, including one that indicates a newline. However, it only ever matches one character, even if newlines are coded as CRLF. Without this option, a dot does not match when the current position in the subject is at a newline. This option is equivalent to Perl's /s option, and it can be changed within a pattern by a (?s) option setting. A negative class such as [^a] always matches newline characters, and the \N escape sequence always matches a non-newline character, independent of the setting of PCRE2_DOTALL.
        /// </summary>
        DotAll = PcreConstants.DOTALL,

        /// <inheritdoc cref="NoAutoCapture"/>
        ExplicitCapture = PcreConstants.NO_AUTO_CAPTURE,

        /// <summary>
        /// <c>PCRE2_NO_AUTO_CAPTURE</c> - If this option is set, it disables the use of numbered capturing parentheses in the pattern. Any opening parenthesis that is not followed by ? behaves as if it were followed by ?: but named parentheses can still be used for capturing (and they acquire numbers in the usual way). This is the same as Perl's /n option. Note that, when this option is set, references to capture groups (backreferences or recursion/subroutine calls) may only refer to named groups, though the reference can be by name or by number.
        /// </summary>
        NoAutoCapture = PcreConstants.NO_AUTO_CAPTURE,

        /// <inheritdoc cref="Extended"/>
        IgnorePatternWhitespace = PcreConstants.EXTENDED,

        /// <summary>
        /// <c>PCRE2_EXTENDED</c> - If this bit is set, most white space characters in the pattern are totally ignored except when escaped or inside a character class. However, white space is not allowed within sequences such as (?> that introduce various parenthesized groups, nor within numerical quantifiers such as {1,3}. Ignorable white space is permitted between an item and a following quantifier and between a quantifier and a following + that indicates possessiveness. PCRE2_EXTENDED is equivalent to Perl's /x option, and it can be changed within a pattern by a (?x) option setting.
        /// </summary>
        /// <remarks>
        /// When PCRE2 is compiled without Unicode support, PCRE2_EXTENDED recognizes as white space only those characters with code points less than 256 that are flagged as white space in its low-character table. The table is normally created by pcre2_maketables(), which uses the isspace() function to identify space characters. In most ASCII environments, the relevant characters are those with code points 0x0009 (tab), 0x000A (linefeed), 0x000B (vertical tab), 0x000C (formfeed), 0x000D (carriage return), and 0x0020 (space).
        /// When PCRE2 is compiled with Unicode support, in addition to these characters, five more Unicode "Pattern White Space" characters are recognized by PCRE2_EXTENDED. These are U+0085 (next line), U+200E (left-to-right mark), U+200F (right-to-left mark), U+2028 (line separator), and U+2029 (paragraph separator). This set of characters is the same as recognized by Perl's /x option. Note that the horizontal and vertical space characters that are matched by the \h and \v escapes in patterns are a much bigger set.
        /// As well as ignoring most white space, PCRE2_EXTENDED also causes characters between an unescaped # outside a character class and the next newline, inclusive, to be ignored, which makes it possible to include comments inside complicated patterns. Note that the end of this type of comment is a literal newline sequence in the pattern; escape sequences that happen to represent a newline do not count.
        /// Which characters are interpreted as newlines can be specified by a setting in the compile context that is passed to pcre2_compile() or by a special sequence at the start of the pattern, as described in the section entitled "Newline conventions" in the pcre2pattern documentation. A default is defined when PCRE2 is built.
        /// </remarks>
        Extended = PcreConstants.EXTENDED,

        /// <summary>
        /// <c>PCRE2_EXTENDED_MORE</c> - This option has the effect of PCRE2_EXTENDED, but, in addition, unescaped space and horizontal tab characters are ignored inside a character class. Note: only these two characters are ignored, not the full set of pattern white space characters that are ignored outside a character class. PCRE2_EXTENDED_MORE is equivalent to Perl's /xx option, and it can be changed within a pattern by a (?xx) option setting.
        /// </summary>
        ExtendedMore = PcreConstants.EXTENDED_MORE,


        JavaScript = PcreConstants.ALT_BSUX | PcreConstants.MATCH_UNSET_BACKREF,

        /// <summary>
        /// <c>PCRE2_ALT_BSUX</c> - This option request alternative handling of three escape sequences, which makes PCRE2's behaviour more like ECMAscript (aka JavaScript).
        /// </summary>
        /// <remarks>
        /// When it is set:
        /// (1) \U matches an upper case "U" character; by default \U causes a compile time error (Perl uses \U to upper case subsequent characters).
        /// (2) \u matches a lower case "u" character unless it is followed by four hexadecimal digits, in which case the hexadecimal number defines the code point to match. By default, \u causes a compile time error (Perl uses it to upper case the following character).
        /// (3) \x matches a lower case "x" character unless it is followed by two hexadecimal digits, in which case the hexadecimal number defines the code point to match. By default, as in Perl, a hexadecimal number is always expected after \x, but it may have zero, one, or two digits (so, for example, \xz matches a binary zero character followed by z).
        /// ECMAscript 6 added additional functionality to \u. This can be accessed using the PCRE2_EXTRA_ALT_BSUX extra option (see "Extra compile options" below). Note that this alternative escape handling applies only to patterns. Neither of these options affects the processing of replacement strings passed to pcre2_substitute().
        /// </remarks>
        AltBsUX = PcreConstants.ALT_BSUX,

        /// <summary>
        /// <c>PCRE2_MATCH_UNSET_BACKREF</c> - If this option is set, a backreference to an unset capture group matches an empty string (by default this causes the current matching alternative to fail). A pattern such as (\1)(a) succeeds when this option is set (assuming it can find an "a" in the subject), whereas it fails by default, for Perl compatibility. Setting this option makes PCRE2 behave more like ECMAscript (aka JavaScript).
        /// </summary>
        MatchUnsetBackref = PcreConstants.MATCH_UNSET_BACKREF,

        /// <summary>
        /// <c>PCRE2_LITERAL</c> - If this option is set, all meta-characters in the pattern are disabled, and it is treated as a literal string. Matching literal strings with a regular expression engine is not the most efficient way of doing it. If you are doing a lot of literal matching and are worried about efficiency, you should consider using other approaches. The only other main options that are allowed with PCRE2_LITERAL are: PCRE2_ANCHORED, PCRE2_ENDANCHORED, PCRE2_AUTO_CALLOUT, PCRE2_CASELESS, PCRE2_FIRSTLINE, PCRE2_MATCH_INVALID_UTF, PCRE2_NO_START_OPTIMIZE, PCRE2_NO_UTF_CHECK, PCRE2_UTF, and PCRE2_USE_OFFSET_LIMIT. The extra options PCRE2_EXTRA_MATCH_LINE and PCRE2_EXTRA_MATCH_WORD are also supported. Any other options cause an error.
        /// </summary>
        Literal = PcreConstants.LITERAL,

        /// <inheritdoc cref="Ucp"/>
        Unicode = PcreConstants.UCP,

        /// <summary>
        /// <c>PCRE2_UCP</c> - This option has two effects. Firstly, it change the way PCRE2 processes \B, \b, \D, \d, \S, \s, \W, \w, and some of the POSIX character classes. By default, only ASCII characters are recognized, but if PCRE2_UCP is set, Unicode properties are used instead to classify characters. More details are given in the section on generic character types in the pcre2pattern page. If you set PCRE2_UCP, matching one of the items it affects takes much longer.
        /// </summary>
        /// <remarks>
        /// The second effect of PCRE2_UCP is to force the use of Unicode properties for upper/lower casing operations on characters with code points greater than 127, even when PCRE2_UTF is not set. This makes it possible, for example, to process strings in the 16-bit UCS-2 code. This option is available only if PCRE2 has been compiled with Unicode support (which is the default).
        /// </remarks>
        Ucp = PcreConstants.UCP,

        /// <summary>
        /// <c>PCRE2_MATCH_INVALID_UTF</c> - This option forces PCRE2_UTF (see below) and also enables support for matching by pcre2_match() in subject strings that contain invalid UTF sequences. This facility is not supported for DFA matching. For details, see the pcre2unicode documentation.
        /// </summary>
        MatchInvalidUtf = PcreConstants.MATCH_INVALID_UTF,

        /// <summary>
        /// <c>PCRE2_ANCHORED</c> - If this bit is set, the pattern is forced to be "anchored", that is, it is constrained to match only at the first matching point in the string that is being searched (the "subject string"). This effect can also be achieved by appropriate constructs in the pattern itself, which is the only way to do it in Perl.
        /// </summary>
        Anchored = PcreConstants.ANCHORED,

        /// <summary>
        /// <c>PCRE2_ENDANCHORED</c> - If this bit is set, the end of any pattern match must be right at the end of the string being searched (the "subject string"). If the pattern match succeeds by reaching (*ACCEPT), but does not reach the end of the subject, the match fails at the current starting point. For unanchored patterns, a new match is then tried at the next starting point. However, if the match succeeds by reaching the end of the pattern, but not the end of the subject, backtracking occurs and an alternative match may be found.
        /// </summary>
        /// <remarks>
        /// Consider these two patterns:
        /// .(*ACCEPT)|..
        /// .|..
        /// If matched against "abc" with PCRE2_ENDANCHORED set, the first matches "c" whereas the second matches "bc". The effect of PCRE2_ENDANCHORED can also be achieved by appropriate constructs in the pattern itself, which is the only way to do it in Perl.
        /// For DFA matching with pcre2_dfa_match(), PCRE2_ENDANCHORED applies only to the first (that is, the longest) matched string. Other parallel matches, which are necessarily substrings of the first one, must obviously end before the end of the subject.
        /// </remarks>
        EndAnchored = PcreConstants.ENDANCHORED,

        /// <summary>
        /// <c>PCRE2_UNGREEDY</c> - This option inverts the "greediness" of the quantifiers so that they are not greedy by default, but become greedy if followed by "?". It is not compatible with Perl. It can also be set by a (?U) option setting within the pattern.
        /// </summary>
        Ungreedy = PcreConstants.UNGREEDY,

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
        /// <c>PCRE2_FIRSTLINE</c> - If this option is set, the start of an unanchored pattern match must be before or at the first newline in the subject string following the start of matching, though the matched text may continue over the newline. If startoffset is non-zero, the limiting newline is not necessarily the first newline in the subject. For example, if the subject string is "abc\nxyz" (where \n represents a single-character newline) a pattern match for "yz" succeeds with PCRE2_FIRSTLINE if startoffset is greater than 3. See also PCRE2_USE_OFFSET_LIMIT, which provides a more general limiting facility. If PCRE2_FIRSTLINE is set with an offset limit, a match must occur in the first line and also within the offset limit. In other words, whichever limit comes first is used.
        /// </summary>
        FirstLine = PcreConstants.FIRSTLINE,

        /// <summary>
        /// <c>PCRE2_DUPNAMES</c> - If this bit is set, names used to identify capture groups need not be unique. This can be helpful for certain types of pattern when it is known that only one instance of the named group can ever be matched. There are more details of named capture groups below; see also the pcre2pattern documentation.
        /// </summary>
        DupNames = PcreConstants.DUPNAMES,

        /// <summary>
        /// <c>PCRE2_AUTO_CALLOUT</c> - If this bit is set, pcre2_compile() automatically inserts callout items, all with number 255, before each pattern item, except immediately before or after an explicit callout in the pattern. For discussion of the callout facility, see the pcre2callout documentation.
        /// </summary>
        AutoCallout = PcreConstants.AUTO_CALLOUT,

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
        /// <c>PCRE2_NO_AUTO_POSSESS</c> - If this option is set, it disables "auto-possessification", which is an optimization that, for example, turns a+b into a++b in order to avoid backtracks into a+ that can never be successful. However, if callouts are in use, auto-possessification means that some callouts are never taken. You can set this option if you want the matching functions to do a full unoptimized search and run all the callouts, but it is mainly provided for testing purposes.
        /// </summary>
        NoAutoPossess = PcreConstants.NO_AUTO_POSSESS,

        /// <summary>
        /// <c>PCRE2_DOLLAR_ENDONLY</c> - If this bit is set, a dollar metacharacter in the pattern matches only at the end of the subject string. Without this option, a dollar also matches immediately before a newline at the end of the string (but not before any other newlines). The PCRE2_DOLLAR_ENDONLY option is ignored if PCRE2_MULTILINE is set. There is no equivalent to this option in Perl, and no way to set it within a pattern.
        /// </summary>
        DollarEndOnly = PcreConstants.DOLLAR_ENDONLY,

        /// <summary>
        /// <c>PCRE2_ALT_CIRCUMFLEX</c> - In multiline mode (when PCRE2_MULTILINE is set), the circumflex metacharacter matches at the start of the subject (unless PCRE2_NOTBOL is set), and also after any internal newline. However, it does not match after a newline at the end of the subject, for compatibility with Perl. If you want a multiline circumflex also to match after a terminating newline, you must set PCRE2_ALT_CIRCUMFLEX.
        /// </summary>
        AltCircumflex = PcreConstants.ALT_CIRCUMFLEX,

        /// <summary>
        /// <c>PCRE2_ALT_VERBNAMES</c> - By default, for compatibility with Perl, the name in any verb sequence such as (*MARK:NAME) is any sequence of characters that does not include a closing parenthesis. The name is not processed in any way, and it is not possible to include a closing parenthesis in the name. However, if the PCRE2_ALT_VERBNAMES option is set, normal backslash processing is applied to verb names and only an unescaped closing parenthesis terminates the name. A closing parenthesis can be included in a name either as \) or between \Q and \E. If the PCRE2_EXTENDED or PCRE2_EXTENDED_MORE option is set with PCRE2_ALT_VERBNAMES, unescaped whitespace in verb names is skipped and #-comments are recognized, exactly as in the rest of the pattern.
        /// </summary>
        AltVerbNames = PcreConstants.ALT_VERBNAMES,

        /// <summary>
        /// <c>PCRE2_ALLOW_EMPTY_CLASS</c> - By default, for compatibility with Perl, a closing square bracket that immediately follows an opening one is treated as a data character for the class. When PCRE2_ALLOW_EMPTY_CLASS is set, it terminates the class, which therefore contains no characters and so can never match.
        /// </summary>
        AllowEmptyClass = PcreConstants.ALLOW_EMPTY_CLASS,

        /// <summary>
        /// <c>PCRE2_NO_DOTSTAR_ANCHOR</c> - If this option is set, it disables an optimization that is applied when .* is the first significant item in a top-level branch of a pattern, and all the other branches also start with .* or with \A or \G or ^. The optimization is automatically disabled for .* if it is inside an atomic group or a capture group that is the subject of a backreference, or if the pattern contains (*PRUNE) or (*SKIP). When the optimization is not disabled, such a pattern is automatically anchored if PCRE2_DOTALL is set for all the .* items and PCRE2_MULTILINE is not set for any ^ items. Otherwise, the fact that any match must start either at the start of the subject or following a newline is remembered. Like other optimizations, this can cause callouts to be skipped.
        /// </summary>
        NoDotStarAnchor = PcreConstants.NO_DOTSTAR_ANCHOR,

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
        /// <c>PCRE2_NEVER_UCP</c> - This option locks out the use of Unicode properties for handling \B, \b, \D, \d, \S, \s, \W, \w, and some of the POSIX character classes, as described for the PCRE2_UCP option below. In particular, it prevents the creator of the pattern from enabling this facility by starting the pattern with (*UCP). This option may be useful in applications that process patterns from external sources. The option combination PCRE_UCP and PCRE_NEVER_UCP causes an error.
        /// </summary>
        NeverUcp = PcreConstants.NEVER_UCP,

        /// <summary>
        /// <c>PCRE2_NEVER_BACKSLASH_C</c> - This option locks out the use of \C in the pattern that is being compiled. This escape can cause unpredictable behaviour in UTF-8 or UTF-16 modes, because it may leave the current matching point in the middle of a multi-code-unit character. This option may be useful in applications that process patterns from external sources. Note that there is also a build-time option that permanently locks out the use of \C.
        /// </summary>
        NeverBackslashC = PcreConstants.NEVER_BACKSLASH_C,

        /// <summary>
        /// <c>PCRE2_USE_OFFSET_LIMIT</c> - This option must be set for pcre2_compile() if pcre2_set_offset_limit() is going to be used to set a non-default offset limit in a match context for matches that use this pattern. An error is generated if an offset limit is set without this option. For more details, see the description of pcre2_set_offset_limit() in the section that describes match contexts. See also the PCRE2_FIRSTLINE option above.
        /// </summary>
        UseOffsetLimit = PcreConstants.USE_OFFSET_LIMIT,

        // Extra options

        Compiled = 1L << 32,
        CompiledPartial = 1L << 33
    }
}
