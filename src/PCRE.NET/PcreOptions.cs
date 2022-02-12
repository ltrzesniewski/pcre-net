using System;
using System.Diagnostics.CodeAnalysis;
using PCRE.Internal;

namespace PCRE;

/// <summary>
/// Pattern compile options.
/// </summary>
[Flags]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("Microsoft.Design", "CA1069")]
public enum PcreOptions : long
{
    /// <summary>
    /// No additional options.
    /// </summary>
    None = 0,

    // --- Synonyms for compatibility with the .NET Regex class ---

    /// <inheritdoc cref="Caseless"/>
    IgnoreCase = PcreConstants.CASELESS,

    /// <inheritdoc cref="DotAll"/>
    Singleline = PcreConstants.DOTALL,

    /// <inheritdoc cref="NoAutoCapture"/>
    ExplicitCapture = PcreConstants.NO_AUTO_CAPTURE,

    /// <inheritdoc cref="Extended"/>
    IgnorePatternWhitespace = PcreConstants.EXTENDED,

    /// <inheritdoc cref="Ucp"/>
    Unicode = PcreConstants.UCP,

    /// <summary>
    /// Enable JavaScript-compatible mode.
    /// </summary>
    /// <remarks>
    /// This enables <see cref="AltBsUX"/> and <see cref="MatchUnsetBackref"/>.
    /// </remarks>
    JavaScript = PcreConstants.ALT_BSUX | PcreConstants.MATCH_UNSET_BACKREF,

    // --- Extra options ---

    /// <summary>
    /// JIT-compile the pattern for better performance.
    /// </summary>
    Compiled = 1L << 32,

    /// <summary>
    /// Enable support for partial matching in a JIT-compiled pattern.
    /// </summary>
    CompiledPartial = 1L << 33,

    // --- Normal options ---

    /// <summary>
    /// <c>PCRE2_CASELESS</c> - Case-insensitive matching.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If this bit is set, letters in the pattern match both upper and lower case letters in the subject.
    /// </para>
    /// <para>
    /// It is equivalent to Perl's <c>/i</c> option, and it can be changed within a pattern by a <c>(?i)</c> option setting.
    /// </para>
    /// <para>
    /// If either <see cref="Utf"/> or <see cref="Ucp"/> is set, Unicode properties are used for all characters with more than one other case,and for all characters whose code points are greater than U+007F.
    /// Note that there are two ASCII characters, K and S, that, in addition to their lower case ASCII equivalents, are case-equivalent with U+212A (Kelvin sign) and U+017F (long S) respectively.
    /// For lower valued characters with only one other case, a lookup table is used for speed. When neither <see cref="Utf"/> nor <see cref="Ucp"/> is set,
    /// a lookup table is used for all code points less than 256, and higher code points (available only in 16-bit or 32-bit mode) are treated as not having another case.
    /// </para>
    /// </remarks>
    Caseless = PcreConstants.CASELESS,

    /// <summary>
    /// <c>PCRE2_MULTILINE</c> - Multiline mode. Make <c>^</c> and <c>$</c> match at the beginning and end, respectively, of any line.
    /// </summary>
    /// <remarks>
    /// <para>
    /// By default, for the purposes of matching "start of line" and "end of line", PCRE2 treats the subject string as consisting of a single line of characters, even if it actually contains newlines.
    /// The "start of line" metacharacter (<c>^</c>) matches only at the start of the string, and the "end of line" metacharacter (<c>$</c>) matches only at the end of the string,
    /// or before a terminating newline (except when <see cref="DollarEndOnly"/> is set).
    /// Note, however, that unless <see cref="DotAll"/> is set, the "any character" metacharacter (<c>.</c>) does not match at a newline.
    /// This behaviour (for <c>^</c>, <c>$</c>, and dot) is the same as Perl.
    /// </para>
    /// <para>
    /// When <see cref="MultiLine"/> it is set, the "start of line" and "end of line" constructs match immediately following or immediately before internal newlines in the subject string,
    /// respectively, as well as at the very start and end.
    /// </para>
    /// <para>
    /// This is equivalent to Perl's <c>/m</c> option, and it can be changed within a pattern by a <c>(?m)</c> option setting.
    /// </para>
    /// <para>
    /// Note that the "start of line" metacharacter does not match after a newline at the end of the subject, for compatibility with Perl.
    /// However, you can change this by setting the <see cref="AltCircumflex"/> option.
    /// If there are no newlines in a subject string, or no occurrences of <c>^</c> or <c>$</c> in a pattern, setting <see cref="MultiLine"/> has no effect.
    /// </para>
    /// </remarks>
    MultiLine = PcreConstants.MULTILINE,

    /// <summary>
    /// <c>PCRE2_DOTALL</c> - Single-line mode. Make the dot (<c>.</c>) match any character (including newlines).
    /// </summary>
    /// <remarks>
    /// <para>
    /// If this bit is set, a dot metacharacter in the pattern matches any character, including one that indicates a newline.
    /// However, it only ever matches one character, even if newlines are coded as CRLF. Without this option, a dot does not match when the current position in the subject is at a newline.
    /// </para>
    /// <para>
    /// This option is equivalent to Perl's <c>/s</c> option, and it can be changed within a pattern by a <c>(?s)</c> option setting.
    /// </para>
    /// <para>
    /// A negative class such as <c>[^a]</c> always matches newline characters, and the <c>\N</c> escape sequence always matches a non-newline character, independent of the setting of <see cref="DotAll"/>.
    /// </para>
    /// </remarks>
    DotAll = PcreConstants.DOTALL,

    /// <summary>
    /// <c>PCRE2_NO_AUTO_CAPTURE</c> - Disable capturing by unnamed groups. This makes capturing explicit.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If this option is set, it disables the use of numbered capturing parentheses in the pattern.
    /// Any opening parenthesis that is not followed by <c>?</c> behaves as if it were followed by <c>?:</c> but named parentheses can still be used for capturing (and they acquire numbers in the usual way).
    /// </para>
    /// <para>
    /// This is the same as Perl's <c>/n</c> option.
    /// </para>
    /// <para>
    /// Note that, when this option is set, references to capture groups (backreferences or recursion/subroutine calls) may only refer to named groups, though the reference can be by name or by number.
    /// </para>
    /// </remarks>
    NoAutoCapture = PcreConstants.NO_AUTO_CAPTURE,

    /// <summary>
    /// <c>PCRE2_EXTENDED</c> - Extended mode. Ignore unescaped whitespace in the pattern. Enable comments marked with <c>#</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If this bit is set, most white space characters in the pattern are totally ignored except when escaped or inside a character class.
    /// However, white space is not allowed within sequences such as <c>(?&gt;</c> that introduce various parenthesized groups, nor within numerical quantifiers such as <c>{1,3}</c>.
    /// Ignorable white space is permitted between an item and a following quantifier and between a quantifier and a following <c>+</c> that indicates possessiveness.
    /// </para>
    /// <para>
    /// <see cref="Extended"/> is equivalent to Perl's <c>/x</c> option, and it can be changed within a pattern by a <c>(?x)</c> option setting.
    /// </para>
    /// <para>
    /// When PCRE2 is compiled without Unicode support, <see cref="Extended"/> recognizes as white space only those characters with code points less than 256 that are flagged as white space
    /// in its low-character table. The table is normally created by <c>pcre2_maketables()</c>, which uses the <c>isspace()</c> function to identify space characters.
    /// In most ASCII environments, the relevant characters are those with code points 0x0009 (tab), 0x000A (linefeed), 0x000B (vertical tab), 0x000C (formfeed), 0x000D (carriage return), and 0x0020 (space).
    /// </para>
    /// <para>
    /// When PCRE2 is compiled with Unicode support, in addition to these characters, five more Unicode "Pattern White Space" characters are recognized by <see cref="Extended"/>.
    /// These are U+0085 (next line), U+200E (left-to-right mark), U+200F (right-to-left mark), U+2028 (line separator), and U+2029 (paragraph separator).
    /// This set of characters is the same as recognized by Perl's <c>/x</c> option.
    /// Note that the horizontal and vertical space characters that are matched by the <c>\h</c> and <c>\v</c> escapes in patterns are a much bigger set.
    /// </para>
    /// <para>
    /// As well as ignoring most white space, <see cref="Extended"/> also causes characters between an unescaped <c>#</c> outside a character class and the next newline,
    /// inclusive, to be ignored, which makes it possible to include comments inside complicated patterns.
    /// Note that the end of this type of comment is a literal newline sequence in the pattern; escape sequences that happen to represent a newline do not count.
    /// Which characters are interpreted as newlines can be specified by a setting in the compile context that is passed to <c>pcre2_compile()</c> or by a special sequence
    /// at the start of the pattern, as described in the section entitled "Newline conventions" in the pcre2pattern documentation. A default is defined when PCRE2 is built.
    /// </para>
    /// </remarks>
    Extended = PcreConstants.EXTENDED,

    /// <summary>
    /// <c>PCRE2_EXTENDED_MORE</c> - Extended mode that additionally ignores unescaped whitespace in character classes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This option has the effect of <see cref="Extended"/>, but, in addition, unescaped space and horizontal tab characters are ignored inside a character class.
    /// Note: only these two characters are ignored, not the full set of pattern white space characters that are ignored outside a character class.
    /// </para>
    /// <para>
    /// <see cref="ExtendedMore"/> is equivalent to Perl's <c>/xx</c> option, and it can be changed within a pattern by a <c>(?xx)</c> option setting.
    /// </para>
    /// </remarks>
    ExtendedMore = PcreConstants.EXTENDED_MORE,

    /// <summary>
    /// <c>PCRE2_ALT_BSUX</c> - Alternative handling of some escape sequences (ECMAScript-compliant behavior).
    /// </summary>
    /// <remarks>
    /// <para>
    /// This option request alternative handling of three escape sequences, which makes PCRE2's behaviour more like ECMAscript (aka JavaScript).
    /// When it is set:
    /// </para>
    /// <list type="bullet">
    /// <item>
    /// <c>\U</c> matches an upper case "U" character; by default <c>\U</c> causes a compile time error (Perl uses <c>\U</c> to upper case subsequent characters).
    /// </item>
    /// <item>
    /// <c>\u</c> matches a lower case "u" character unless it is followed by four hexadecimal digits, in which case the hexadecimal number defines the code point to match.
    /// By default, <c>\u</c> causes a compile time error (Perl uses it to upper case the following character).
    /// </item>
    /// <item>
    /// <c>\x</c> matches a lower case "x" character unless it is followed by two hexadecimal digits, in which case the hexadecimal number defines the code point to match.
    /// By default, as in Perl, a hexadecimal number is always expected after <c>\x</c>, but it may have zero, one, or two digits (so, for example, <c>\xz</c> matches a binary zero character followed by z).
    /// </item>
    /// </list>
    /// <para>
    /// ECMAscript 6 added additional functionality to <c>\u</c>. This can be accessed using the <see cref="PcreExtraCompileOptions.AltBsUX"/> extra option (see <see cref="PcreExtraCompileOptions"/>).
    /// Note that this alternative escape handling applies only to patterns. Neither of these options affects the processing of replacement strings passed to <c>pcre2_substitute()</c>.
    /// </para>
    /// </remarks>
    AltBsUX = PcreConstants.ALT_BSUX,

    /// <summary>
    /// <c>PCRE2_MATCH_UNSET_BACKREF</c> - Make backreferences to unset groups match an empty string (ECMAScript-compliant behavior).
    /// </summary>
    /// <remarks>
    /// If this option is set, a backreference to an unset capture group matches an empty string (by default this causes the current matching alternative to fail).
    /// A pattern such as <c>(\1)(a)</c> succeeds when this option is set (assuming it can find an "a" in the subject), whereas it fails by default, for Perl compatibility.
    /// Setting this option makes PCRE2 behave more like ECMAscript (aka JavaScript).
    /// </remarks>
    MatchUnsetBackref = PcreConstants.MATCH_UNSET_BACKREF,

    /// <summary>
    /// <c>PCRE2_LITERAL</c> - Treat the pattern as a literal string.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If this option is set, all meta-characters in the pattern are disabled, and it is treated as a literal string.
    /// Matching literal strings with a regular expression engine is not the most efficient way of doing it.
    /// If you are doing a lot of literal matching and are worried about efficiency, you should consider using other approaches.
    /// </para>
    /// <para>
    /// The only other main options that are allowed with <see cref="Literal"/> are: <see cref="Anchored"/>, <see cref="EndAnchored"/>, <see cref="AutoCallout"/>, <see cref="Caseless"/>,
    /// <see cref="FirstLine"/>, <see cref="MatchInvalidUtf"/>, <see cref="NoStartOptimize"/>, <see cref="NoUtfCheck"/>, <see cref="Utf"/>, and <see cref="UseOffsetLimit"/>.
    /// The extra options <see cref="PcreExtraCompileOptions.MatchLine"/> and <see cref="PcreExtraCompileOptions.MatchWord"/> are also supported. Any other options cause an error.
    /// </para>
    /// </remarks>
    Literal = PcreConstants.LITERAL,

    /// <summary>
    /// <c>PCRE2_UCP</c> - Use Unicode character properties.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This option has two effects. Firstly, it changes the way PCRE2 processes <c>\B</c>, <c>\b</c>, <c>\D</c>, <c>\d</c>, <c>\S</c>, <c>\s</c>, <c>\W</c>, <c>\w</c>,
    /// and some of the POSIX character classes. By default, only ASCII characters are recognized, but if <see cref="Ucp"/> is set, Unicode properties are used instead to classify characters.
    /// More details are given in the section on generic character types in the pcre2pattern page. If you set <see cref="Ucp"/>, matching one of the items it affects takes much longer.
    /// </para>
    /// <para>
    /// The second effect of <see cref="Ucp"/> is to force the use of Unicode properties for upper/lower casing operations on characters with code points greater than 127,
    /// even when <see cref="Utf"/> is not set.
    /// This makes it possible, for example, to process strings in the 16-bit UCS-2 code. This option is available only if PCRE2 has been compiled with Unicode support (which is the default).
    /// </para>
    /// </remarks>
    Ucp = PcreConstants.UCP,

    /// <summary>
    /// <c>PCRE2_MATCH_INVALID_UTF</c> - Support matching invalid UTF sequences.
    /// </summary>
    /// <remarks>
    /// This option forces <see cref="Utf"/> and also enables support for matching by <c>pcre2_match()</c> in subject strings that contain invalid UTF sequences.
    /// This facility is not supported for DFA matching. For details, see the pcre2unicode documentation.
    /// </remarks>
    MatchInvalidUtf = PcreConstants.MATCH_INVALID_UTF,

    /// <summary>
    /// <c>PCRE2_ANCHORED</c> - Make the start of the pattern anchored, so it can only match at the starting position.
    /// </summary>
    /// <remarks>
    /// If this bit is set, the pattern is forced to be "anchored", that is, it is constrained to match only at the first matching point in the string that is being searched (the "subject string").
    /// This effect can also be achieved by appropriate constructs in the pattern itself, which is the only way to do it in Perl.
    /// </remarks>
    /// <seealso cref="PcreMatchOptions.Anchored"/>
    Anchored = PcreConstants.ANCHORED,

    /// <summary>
    /// <c>PCRE2_ENDANCHORED</c> - Make the end of the pattern anchored, so it needs to match until the end of the subject string.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If this bit is set, the end of any pattern match must be right at the end of the string being searched (the "subject string").
    /// </para>
    /// <para>
    /// If the pattern match succeeds by reaching <c>(*ACCEPT)</c>, but does not reach the end of the subject, the match fails at the current starting point.
    /// For unanchored patterns, a new match is then tried at the next starting point.
    /// However, if the match succeeds by reaching the end of the pattern, but not the end of the subject, backtracking occurs and an alternative match may be found.
    /// </para>
    /// <para>
    /// Consider these two patterns:
    /// <list type="bullet">
    /// <item><c>.(*ACCEPT)|..</c></item>
    /// <item><c>.|..</c></item>
    /// </list>
    /// If matched against "abc" with <see cref="EndAnchored"/> set, the first matches "c" whereas the second matches "bc".
    /// The effect of <see cref="EndAnchored"/> can also be achieved by appropriate constructs in the pattern itself, which is the only way to do it in Perl.
    /// </para>
    /// <para>
    /// For DFA matching with <c>pcre2_dfa_match()</c>, <see cref="EndAnchored"/> applies only to the first (that is, the longest) matched string.
    /// Other parallel matches, which are necessarily substrings of the first one, must obviously end before the end of the subject.
    /// </para>
    /// </remarks>
    /// <seealso cref="PcreMatchOptions.EndAnchored"/>
    EndAnchored = PcreConstants.ENDANCHORED,

    /// <summary>
    /// <c>PCRE2_UNGREEDY</c> - Makes the quantifiers ungreedy by default.
    /// </summary>
    /// <remarks>
    /// This option inverts the "greediness" of the quantifiers so that they are not greedy by default, but become greedy if followed by "<c>?</c>".
    /// It is not compatible with Perl. It can also be set by a <c>(?U)</c> option setting within the pattern.
    /// </remarks>
    Ungreedy = PcreConstants.UNGREEDY,

    /// <summary>
    /// <c>PCRE2_FIRSTLINE</c> - Match only until the first newline after the starting position.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If this option is set, the start of an unanchored pattern match must be before or at the first newline in the subject string following the start of matching,
    /// though the matched text may continue over the newline. If startoffset is non-zero, the limiting newline is not necessarily the first newline in the subject.
    /// </para>
    /// <para>
    /// For example, if the subject string is "abc\nxyz" (where <c>\n</c> represents a single-character newline) a pattern match for "yz" succeeds with <see cref="FirstLine"/>
    /// if startoffset is greater than 3.
    /// See also <see cref="UseOffsetLimit"/>, which provides a more general limiting facility.
    /// </para>
    /// <para>
    /// If <see cref="FirstLine"/> is set with an offset limit, a match must occur in the first line and also within the offset limit. In other words, whichever limit comes first is used.
    /// </para>
    /// </remarks>
    FirstLine = PcreConstants.FIRSTLINE,

    /// <summary>
    /// <c>PCRE2_DUPNAMES</c> - Allow duplicate names for capturing groups.
    /// </summary>
    /// <remarks>
    /// If this bit is set, names used to identify capture groups need not be unique.
    /// This can be helpful for certain types of pattern when it is known that only one instance of the named group can ever be matched.
    /// See also the pcre2pattern documentation.
    /// </remarks>
    DupNames = PcreConstants.DUPNAMES,

    /// <summary>
    /// <c>PCRE2_AUTO_CALLOUT</c> - Automatically insert callouts before each pattern item except next to other callouts.
    /// </summary>
    /// <remarks>
    /// If this bit is set, <c>pcre2_compile()</c> automatically inserts callout items, all with number 255, before each pattern item,
    /// except immediately before or after an explicit callout in the pattern. For discussion of the callout facility, see the pcre2callout documentation.
    /// </remarks>
    AutoCallout = PcreConstants.AUTO_CALLOUT,

    /// <summary>
    /// <c>PCRE2_NO_START_OPTIMIZE</c> - Disables optimizations applied at the start of a match.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is an option whose main effect is at matching time. It does not change what <c>pcre2_compile()</c> generates, but it does affect the output of the JIT compiler.
    /// </para>
    /// <para>
    /// There are a number of optimizations that may occur at the start of a match, in order to speed up the process.
    /// For example, if it is known that an unanchored match must start with a specific code unit value, the matching code searches the subject for that value,
    /// and fails immediately if it cannot find it, without actually running the main matching function.
    /// </para>
    /// <para>
    /// This means that a special item such as <c>(*COMMIT)</c> at the start of a pattern is not considered until after a suitable starting point for the match has been found.
    /// Also, when callouts or <c>(*MARK)</c> items are in use, these "start-up" optimizations can cause them to be skipped if the pattern is never actually used.
    /// </para>
    /// <para>
    /// The start-up optimizations are in effect a pre-scan of the subject that takes place before the pattern is run.
    /// The <see cref="NoStartOptimize"/> option disables the start-up optimizations, possibly causing performance to suffer,
    /// but ensuring that in cases where the result is "no match", the callouts do occur, and that items such as <c>(*COMMIT)</c> and <c>(*MARK)</c> are considered
    /// at every possible starting position in the subject string.
    /// </para>
    /// <para>
    /// Setting <see cref="NoStartOptimize"/> may change the outcome of a matching operation. Consider the pattern <c>(*COMMIT)ABC</c>.
    /// When this is compiled, PCRE2 records the fact that a match must start with the character "A".
    /// Suppose the subject string is "DEFABC". The start-up optimization scans along the subject, finds "A" and runs the first match attempt from there.
    /// The <c>(*COMMIT)</c> item means that the pattern must match the current starting position, which in this case, it does.
    /// However, if the same match is run with <see cref="NoStartOptimize"/> set, the initial scan along the subject string does not happen.
    /// The first match attempt is run starting from "D" and when this fails, <c>(*COMMIT)</c> prevents any further matches being tried, so the overall result is "no match".
    /// </para>
    /// <para>
    /// As another start-up optimization makes use of a minimum length for a matching subject, which is recorded when possible. Consider the pattern <c>(*MARK:1)B(*MARK:2)(X|Y)</c>.
    /// The minimum length for a match is two characters. If the subject is "XXBB", the "starting character" optimization skips "XX", then tries to match "BB", which is long enough.
    /// In the process, <c>(*MARK:2)</c> is encountered and remembered. When the match attempt fails, the next "B" is found, but there is only one character left, so there are no more attempts,
    /// and "no match" is returned with the "last mark seen" set to "2".
    /// If <see cref="NoStartOptimize"/> is set, however, matches are tried at every possible starting position, including at the end of the subject,
    /// where <c>(*MARK:1)</c> is encountered, but there is no "B", so the "last mark seen" that is returned is "1".
    /// In this case, the optimizations do not affect the overall match result, which is still "no match", but they do affect the auxiliary information that is returned.
    /// </para>
    /// </remarks>
    NoStartOptimize = PcreConstants.NO_START_OPTIMIZE,

    /// <summary>
    /// <c>PCRE2_NO_AUTO_POSSESS</c> - Disable the auto-possessification optimization.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If this option is set, it disables "auto-possessification", which is an optimization that, for example, turns <c>a+b</c> into <c>a++b</c>
    /// in order to avoid backtracks into <c>a+</c> that can never be successful.
    /// </para>
    /// <para>
    /// However, if callouts are in use, auto-possessification means that some callouts are never taken.
    /// You can set this option if you want the matching functions to do a full unoptimized search and run all the callouts, but it is mainly provided for testing purposes.
    /// </para>
    /// </remarks>
    NoAutoPossess = PcreConstants.NO_AUTO_POSSESS,

    /// <summary>
    /// <c>PCRE2_DOLLAR_ENDONLY</c> - Make <c>$</c> match only at the end of the subject string.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If this bit is set, a dollar metacharacter in the pattern matches only at the end of the subject string.
    /// Without this option, a dollar also matches immediately before a newline at the end of the string (but not before any other newlines).
    /// </para>
    /// <para>
    /// The <see cref="DollarEndOnly"/> option is ignored if <see cref="MultiLine"/> is set.
    /// There is no equivalent to this option in Perl, and no way to set it within a pattern.
    /// </para>
    /// </remarks>
    DollarEndOnly = PcreConstants.DOLLAR_ENDONLY,

    /// <summary>
    /// <c>PCRE2_ALT_CIRCUMFLEX</c> - Make <c>^</c> match after a newline at the end of the subject string.
    /// </summary>
    /// <remarks>
    /// In multiline mode (when <see cref="MultiLine"/> is set), the circumflex metacharacter matches at the start of the subject (unless <see cref="PcreMatchOptions.NotBol"/> is set),
    /// and also after any internal newline. However, it does not match after a newline at the end of the subject, for compatibility with Perl.
    /// If you want a multiline circumflex also to match after a terminating newline, you must set <see cref="AltCircumflex"/>.
    /// </remarks>
    AltCircumflex = PcreConstants.ALT_CIRCUMFLEX,

    /// <summary>
    /// <c>PCRE2_ALT_VERBNAMES</c> - Allow escaped closing parentheses in a verb name.
    /// </summary>
    /// <remarks>
    /// <para>
    /// By default, for compatibility with Perl, the name in any verb sequence such as <c>(*MARK:NAME)</c> is any sequence of characters that does not include a closing parenthesis.
    /// The name is not processed in any way, and it is not possible to include a closing parenthesis in the name.
    /// </para>
    /// <para>
    /// However, if the <see cref="AltVerbNames"/> option is set, normal backslash processing is applied to verb names and only an unescaped closing parenthesis terminates the name.
    /// A closing parenthesis can be included in a name either as <c>\)</c> or between <c>\Q</c> and <c>\E</c>.
    /// </para>
    /// <para>
    /// If the <see cref="Extended"/> or <see cref="ExtendedMore"/> option is set with <see cref="AltVerbNames"/>, unescaped whitespace in verb names is skipped
    /// and <c>#</c>-comments are recognized, exactly as in the rest of the pattern.
    /// </para>
    /// </remarks>
    AltVerbNames = PcreConstants.ALT_VERBNAMES,

    /// <summary>
    /// <c>PCRE2_ALLOW_EMPTY_CLASS</c> - Allow empty character classes.
    /// </summary>
    /// <remarks>
    /// By default, for compatibility with Perl, a closing square bracket that immediately follows an opening one is treated as a data character for the class.
    /// When <see cref="AllowEmptyClass"/> is set, it terminates the class, which therefore contains no characters and so can never match.
    /// </remarks>
    AllowEmptyClass = PcreConstants.ALLOW_EMPTY_CLASS,

    /// <summary>
    /// <c>PCRE2_NO_DOTSTAR_ANCHOR</c> - Disable optimizing branches starting with <c>.*</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If this option is set, it disables an optimization that is applied when <c>.*</c> is the first significant item in a top-level branch of a pattern,
    /// and all the other branches also start with <c>.*</c> or with <c>\A</c> or <c>\G</c> or <c>^</c>.
    /// </para>
    /// <para>
    /// The optimization is automatically disabled for <c>.*</c> if it is inside an atomic group or a capture group that is the subject of a backreference,
    /// or if the pattern contains <c>(*PRUNE)</c> or <c>(*SKIP)</c>.
    /// </para>
    /// <para>
    /// When the optimization is not disabled, such a pattern is automatically anchored if <see cref="DotAll"/> is set for all the <c>.*</c> items and <see cref="MultiLine"/>
    /// is not set for any <c>^</c> items.
    /// Otherwise, the fact that any match must start either at the start of the subject or following a newline is remembered.
    /// </para>
    /// <para>
    /// Like other optimizations, this can cause callouts to be skipped.
    /// </para>
    /// </remarks>
    NoDotStarAnchor = PcreConstants.NO_DOTSTAR_ANCHOR,

    /// <summary>
    /// <c>PCRE2_NO_UTF_CHECK</c> - Disable validation of UTF sequences in the pattern string.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When <see cref="Utf"/> is set, the validity of the pattern as a UTF string is automatically checked.
    /// There are discussions about the validity of UTF-8 strings, UTF-16 strings, and UTF-32 strings in the pcre2unicode document.
    /// If an invalid UTF sequence is found, <c>pcre2_compile()</c> returns a negative error code.
    /// </para>
    /// <para>
    /// If you know that your pattern is a valid UTF string, and you want to skip this check for performance reasons, you can set the <see cref="NoUtfCheck"/> option.
    /// When it is set, the effect of passing an invalid UTF string as a pattern is undefined. It may cause your program to crash or loop.
    /// </para>
    /// <para>
    /// Note that this option can also be passed to <c>pcre2_match()</c> and <c>pcre_dfa_match()</c>, to suppress UTF validity checking of the subject string.
    /// Note also that setting <see cref="NoUtfCheck"/> at compile time does not disable the error that is given if an escape sequence for an invalid Unicode code point
    /// is encountered in the pattern. In particular, the so-called "surrogate" code points (0xd800 to 0xdfff) are invalid.
    /// If you want to allow escape sequences such as <c>\x{d800}</c> you can set the <see cref="PcreExtraCompileOptions.AllowSurrogateEscapes"/> extra option, as described in
    /// <see cref="PcreExtraCompileOptions"/>. However, this is possible only in UTF-8 and UTF-32 modes, because these values are not representable in UTF-16.
    /// </para>
    /// </remarks>
    NoUtfCheck = PcreConstants.NO_UTF_CHECK,

    /// <summary>
    /// <c>PCRE2_NEVER_UCP</c> - Disallow using Unicode character properties.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This option locks out the use of Unicode properties for handling <c>\B</c>, <c>\b</c>, <c>\D</c>, <c>\d</c>, <c>\S</c>, <c>\s</c>, <c>\W</c>, <c>\w</c>,
    /// and some of the POSIX character classes, as described for the <see cref="Ucp"/> option.
    /// </para>
    /// <para>
    /// In particular, it prevents the creator of the pattern from enabling this facility by starting the pattern with <c>(*UCP)</c>.
    /// This option may be useful in applications that process patterns from external sources. The option combination <see cref="Ucp"/> and <see cref="NeverUcp"/> causes an error.
    /// </para>
    /// </remarks>
    NeverUcp = PcreConstants.NEVER_UCP,

    /// <summary>
    /// <c>PCRE2_NEVER_BACKSLASH_C</c> - Disallow using <c>\C</c> in the pattern.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This option locks out the use of <c>\C</c> in the pattern that is being compiled.
    /// This escape can cause unpredictable behaviour in UTF-8 or UTF-16 modes, because it may leave the current matching point in the middle of a multi-code-unit character.
    /// </para>
    /// <para>
    /// This option may be useful in applications that process patterns from external sources. Note that there is also a build-time option that permanently locks out the use of <c>\C</c>.
    /// </para>
    /// </remarks>
    NeverBackslashC = PcreConstants.NEVER_BACKSLASH_C,

    /// <summary>
    /// <c>PCRE2_USE_OFFSET_LIMIT</c> - Allow setting a non-default offset limit.
    /// </summary>
    /// <remarks>
    /// This option must be set for <c>pcre2_compile()</c> if <c>pcre2_set_offset_limit()</c> is going to be used to set a non-default offset limit in a match context
    /// for matches that use this pattern. An error is generated if an offset limit is set without this option.
    /// For more details, see the description of <c>pcre2_set_offset_limit()</c> in the section that describes match contexts. See also the <see cref="FirstLine"/> option.
    /// </remarks>
    UseOffsetLimit = PcreConstants.USE_OFFSET_LIMIT,

    /// <summary>
    /// <c>PCRE2_UTF</c> - Enable UTF mode.
    /// </summary>
    /// <remarks>
    /// This option causes PCRE2 to regard both the pattern and the subject strings that are subsequently processed as strings of UTF characters instead of single-code-unit strings.
    /// It is available when PCRE2 is built to include Unicode support (which is the default). If Unicode support is not available, the use of this option provokes an error.
    /// Details of how <see cref="Utf"/> changes the behaviour of PCRE2 are given in the pcre2unicode page.
    /// In particular, note that it changes the way <see cref="Caseless"/> handles characters with code points greater than 127.
    /// </remarks>
    [Obsolete("PCRE.NET always enables UTF mode.")]
    Utf = PcreConstants.UTF,
}
