﻿using System;
using System.Diagnostics.CodeAnalysis;
using PCRE.Internal;

namespace PCRE;

/// <summary>
/// Extra pattern compile options.
/// </summary>
[Flags]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum PcreExtraCompileOptions : uint
{
    /// <summary>
    /// No additional options.
    /// </summary>
    None = 0,

    /// <summary>
    /// <c>PCRE2_EXTRA_BAD_ESCAPE_IS_LITERAL</c> - Treat unrecognized escapes as literals.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is a dangerous option. Use with care. By default, an unrecognized escape such as <c>\j</c> or a malformed one such as <c>\x{2z}</c> causes a compile-time error
    /// when detected by <c>pcre2_compile()</c>.
    /// </para>
    /// <para>
    /// Perl is somewhat inconsistent in handling such items: for example, <c>\j</c> is treated as a literal "j",
    /// and non-hexadecimal digits in <c>\x{}</c> are just ignored, though warnings are given in both cases if Perl's warning switch is enabled.
    /// However, a malformed octal number after <c>\o{</c> always causes an error in Perl.
    /// </para>
    /// <para>
    /// If the <see cref="BadEscapeIsLiteral"/> extra option is passed to <c>pcre2_compile()</c>, all unrecognized or malformed escape sequences are treated as single-character escapes.
    /// For example, <c>\j</c> is a literal "j" and <c>\x{2z}</c> is treated as the literal string "x{2z}".
    /// </para>
    /// <para>
    /// Setting this option means that typos in patterns may go undetected and have unexpected results.
    /// Also note that a sequence such as <c>[\N{]</c> is interpreted as a malformed attempt at <c>[\N{...}]</c> and so is treated as <c>[N{]</c> whereas <c>[\N]</c> gives an error
    /// because an unqualified <c>\N</c> is a valid escape sequence but is not supported in a character class.
    /// </para>
    /// <para>
    /// To reiterate: this is a dangerous option. Use with great care.
    /// </para>
    /// </remarks>
    BadEscapeIsLiteral = PcreConstants.EXTRA_BAD_ESCAPE_IS_LITERAL,

    /// <summary>
    /// <c>PCRE2_EXTRA_MATCH_WORD</c> - Require word boundaries at the start and end of a match.
    /// </summary>
    /// <remarks>
    /// This option is provided for use by the <c>-w</c> option of pcre2grep.
    /// It causes the pattern only to match strings that have a word boundary at the start and the end.
    /// This is achieved by automatically inserting the code for "<c>\b(?:</c>" at the start of the compiled pattern and "<c>)\b</c>" at the end.
    /// The option may be used with <see cref="PcreOptions.Literal"/>. However, it is ignored if <see cref="MatchLine"/> is also set.
    /// </remarks>
    MatchWord = PcreConstants.EXTRA_MATCH_WORD,

    /// <summary>
    /// <c>PCRE2_EXTRA_MATCH_LINE</c> - Require the pattern to match complete lines.
    /// </summary>
    /// <remarks>
    /// This option is provided for use by the <c>-x</c> option of pcre2grep. It causes the pattern only to match complete lines.
    /// This is achieved by automatically inserting the code for "<c>^(?:</c>" at the start of the compiled pattern and "<c>)$</c>" at the end.
    /// Thus, when <see cref="PcreOptions.MultiLine"/> is set, the matched line may be in the middle of the subject string. This option can be used with <see cref="PcreOptions.Literal"/>.
    /// </remarks>
    MatchLine = PcreConstants.EXTRA_MATCH_LINE,

    /// <summary>
    /// <c>PCRE2_EXTRA_ESCAPED_CR_IS_LF</c> - Treat <c>\r</c> as <c>\n</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// There are some legacy applications where the escape sequence <c>\r</c> in a pattern is expected to match a newline.
    /// If this option is set, <c>\r</c> in a pattern is converted to <c>\n</c> so that it matches a LF (linefeed) instead of a CR (carriage return) character.
    /// </para>
    /// <para>
    /// The option does not affect a literal CR in the pattern, nor does it affect CR specified as an explicit code point such as <c>\x{0D}</c>.
    /// </para>
    /// </remarks>
    EscapedCrIsLf = PcreConstants.EXTRA_ESCAPED_CR_IS_LF,

    /// <summary>
    /// <c>PCRE2_EXTRA_ALT_BSUX</c> - Alternative handling of some escape sequences (ECMAScript 6 compliant behavior).
    /// </summary>
    /// <remarks>
    /// The original option <see cref="PcreOptions.AltBsUX"/> causes PCRE2 to process <c>\U</c>, <c>\u</c>, and <c>\x</c> in the way that ECMAscript (aka JavaScript) does.
    /// Additional functionality was defined by ECMAscript 6; setting <see cref="AltBsUX"/> has the effect of <see cref="PcreOptions.AltBsUX"/>, but in addition it recognizes <c>\u{hhh..}</c>
    /// as a hexadecimal character code, where <c>hhh..</c> is any number of hexadecimal digits.
    /// </remarks>
    AltBsUX = PcreConstants.EXTRA_ALT_BSUX,

    /// <summary>
    /// <c>PCRE2_EXTRA_ALLOW_LOOKAROUND_BSK</c> - Allow <c>\K</c> in lookarounds.
    /// </summary>
    /// <remarks>
    /// Since release 10.38 PCRE2 has forbidden the use of <c>\K</c> within lookaround assertions, following Perl's lead.
    /// This option is provided to re-enable the previous behaviour (act in positive lookarounds, ignore in negative ones) in case anybody is relying on it.
    /// </remarks>
    AllowLookaroundBsK = PcreConstants.EXTRA_ALLOW_LOOKAROUND_BSK,

    /// <summary>
    /// <c>PCRE2_EXTRA_CASELESS_RESTRICT</c> - Disable mixed ASCII/non-ASCII case folding.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When either <see cref="PcreOptions.Ucp"/> or <see cref="PcreOptions.Utf"/> is set, caseless matching follows Unicode rules, which allow for more than two cases per character.
    /// There are two case-equivalent character sets that contain both ASCII and non-ASCII characters.
    /// The ASCII letter S is case-equivalent to U+017f (long S) and the ASCII letter K is case-equivalent to U+212a (Kelvin sign).
    /// This option disables recognition of case-equivalences that cross the ASCII/non-ASCII boundary.
    /// In a caseless match, both characters must either be ASCII or non-ASCII.
    /// </para>
    /// <para>
    /// The option can be changed with a pattern by the <c>(?r)</c> option setting.
    /// </para>
    /// </remarks>
    CaselessRestrict = PcreConstants.EXTRA_CASELESS_RESTRICT,

    /// <summary>
    /// <c>PCRE2_EXTRA_ASCII_BSD</c> - Forces <c>\d</c> to match only ASCII digits, even when <see cref="PcreOptions.Ucp"/> is set.
    /// </summary>
    /// <remarks>
    /// It can also be changed within a pattern by means of the <c>(?aD)</c> option setting.
    /// </remarks>
    AsciiBsD = PcreConstants.EXTRA_ASCII_BSD,

    /// <summary>
    /// <c>PCRE2_EXTRA_ASCII_BSS</c> - Forces <c>\s</c> to match only ASCII digits, even when <see cref="PcreOptions.Ucp"/> is set.
    /// </summary>
    /// <remarks>
    /// It can also be changed within a pattern by means of the <c>(?aS)</c> option setting.
    /// </remarks>
    AsciiBsS = PcreConstants.EXTRA_ASCII_BSS,

    /// <summary>
    /// <c>PCRE2_EXTRA_ASCII_BSW</c> - Forces <c>\w</c> to match only ASCII digits, even when <see cref="PcreOptions.Ucp"/> is set.
    /// </summary>
    /// <remarks>
    /// It can also be changed within a pattern by means of the <c>(?aW)</c> option setting.
    /// </remarks>
    AsciiBsW = PcreConstants.EXTRA_ASCII_BSW,

    /// <summary>
    /// <c>PCRE2_EXTRA_ASCII_POSIX</c> - Forces all the POSIX character classes, including <c>[:digit:]</c> and <c>[:xdigit:]</c>, to match only ASCII characters, even when <see cref="PcreOptions.Ucp"/> is set.
    /// </summary>
    /// <remarks>
    /// It can be changed within a pattern by means of the <c>(?aP)</c> option setting, but note that this also sets <see cref="AsciiDigit"/> in order to ensure that <c>(?-aP)</c> unsets all ASCII restrictions for POSIX classes.
    /// </remarks>
    AsciiPosix = PcreConstants.EXTRA_ASCII_POSIX,

    /// <summary>
    /// <c>PCRE2_EXTRA_ASCII_DIGIT</c> - Forces the POSIX character classes <c>[:digit:]</c> and <c>[:xdigit:]</c> to match only ASCII digits, even when <see cref="PcreOptions.Ucp"/> is set.
    /// </summary>
    /// <remarks>
    /// It can be changed within a pattern by means of the <c>(?aT)</c> option setting.
    /// </remarks>
    AsciiDigit = PcreConstants.EXTRA_ASCII_DIGIT,

    /// <summary>
    /// <c>PCRE2_EXTRA_PYTHON_OCTAL</c> - Follow Python's rules for interpreting octal escape sequences.
    /// </summary>
    /// <remarks>
    /// The rules for handling sequences such as <c>\14</c>, which could be an octal number or a back reference are different. Details are given in the pcre2pattern documentation.
    /// </remarks>
    PythonOctal = PcreConstants.EXTRA_PYTHON_OCTAL,

    /// <summary>
    /// <c>PCRE2_EXTRA_NO_BS0</c> - Lock out the use of the sequence <c>\0</c> unless at least one more octal digit follows.
    /// </summary>
    NoBs0 = PcreConstants.EXTRA_NO_BS0,

    /// <summary>
    /// <c>PCRE2_EXTRA_NEVER_CALLOUT</c> - Treat callouts in the pattern as a syntax error.
    /// </summary>
    /// <remarks>
    /// If this option is set, PCRE2 treats callouts in the pattern as a syntax error, returning <see cref="PcreErrorCode.CalloutCallerDisabled"/>.
    /// This is useful if the application knows that a callout will not be provided to <c>Match</c>, so that callouts in the pattern are not silently ignored.
    /// </remarks>
    NeverCallout = PcreConstants.EXTRA_NEVER_CALLOUT,

    /// <summary>
    /// <c>PCRE2_EXTRA_TURKISH_CASING</c> - Alters case-equivalence of the 'i' letters to follow the alphabet used by Turkish and Azeri languages.
    /// </summary>
    /// <remarks>
    /// The option can be changed within a pattern by the <c>(*TURKISH_CASING)</c> start-of-pattern setting. Either the UTF or UCP options must be set.
    /// This option cannot be combined with <see cref="CaselessRestrict"/>.
    /// </remarks>
    TurkishCasing = PcreConstants.EXTRA_TURKISH_CASING,

    /// <summary>
    /// <c>PCRE2_EXTRA_ALLOW_SURROGATE_ESCAPES</c> - Allow surrogate escapes in UTF-8 and UTF-32.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This option applies when compiling a pattern in UTF-8 or UTF-32 mode. It is forbidden in UTF-16 mode, and ignored in non-UTF modes.
    /// Unicode "surrogate" code points in the range 0xd800 to 0xdfff are used in pairs in UTF-16 to encode code points with values in the range 0x10000 to 0x10ffff.
    /// The surrogates cannot therefore be represented in UTF-16. They can be represented in UTF-8 and UTF-32, but are defined as invalid code points, and cause errors
    /// if encountered in a UTF-8 or UTF-32 string that is being checked for validity by PCRE2.
    /// </para>
    /// <para>
    /// These values also cause errors if encountered in escape sequences such as <c>\x{d912}</c> within a pattern.
    /// However, it seems that some applications, when using PCRE2 to check for unwanted characters in UTF-8 strings, explicitly test for the surrogates using escape sequences.
    /// The <see cref="PcreOptions.NoUtfCheck"/> option does not disable the error that occurs, because it applies only to the testing of input strings for UTF validity.
    /// </para>
    /// <para>
    /// If the extra option <see cref="AllowSurrogateEscapes"/> is set, surrogate code point values in UTF-8 and UTF-32 patterns no longer provoke errors and are incorporated in the compiled pattern.
    /// However, they can only match subject characters if the matching function is called with <see cref="PcreOptions.NoUtfCheck"/> set.
    /// </para>
    /// </remarks>
    [Obsolete("PCRE.NET only supports UTF-16 mode.")]
    AllowSurrogateEscapes = PcreConstants.EXTRA_ALLOW_SURROGATE_ESCAPES,
}
