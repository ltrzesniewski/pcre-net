using System;
using System.Diagnostics.CodeAnalysis;
using PCRE.Internal;

namespace PCRE
{
    [Flags]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum PcreExtraCompileOptions : uint
    {
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
        /// If the <c>PCRE2_EXTRA_BAD_ESCAPE_IS_LITERAL</c> extra option is passed to <c>pcre2_compile()</c>, all unrecognized or malformed escape sequences are treated as single-character escapes.
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
        /// The option may be used with <c>PCRE2_LITERAL</c>. However, it is ignored if <c>PCRE2_EXTRA_MATCH_LINE</c> is also set.
        /// </remarks>
        MatchWord = PcreConstants.EXTRA_MATCH_WORD,

        /// <summary>
        /// <c>PCRE2_EXTRA_MATCH_LINE</c> - Require the pattern to match complete lines.
        /// </summary>
        /// <remarks>
        /// This option is provided for use by the <c>-x</c> option of pcre2grep. It causes the pattern only to match complete lines.
        /// This is achieved by automatically inserting the code for "<c>^(?:</c>" at the start of the compiled pattern and "<c>)$</c>" at the end.
        /// Thus, when <c>PCRE2_MULTILINE</c> is set, the matched line may be in the middle of the subject string. This option can be used with <c>PCRE2_LITERAL</c>.
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
        /// The original option <c>PCRE2_ALT_BSUX</c> causes PCRE2 to process <c>\U</c>, <c>\u</c>, and <c>\x</c> in the way that ECMAscript (aka JavaScript) does.
        /// Additional functionality was defined by ECMAscript 6; setting <c>PCRE2_EXTRA_ALT_BSUX</c> has the effect of <c>PCRE2_ALT_BSUX</c>, but in addition it recognizes <c>\u{hhh..}</c>
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
        AllowLookaroundBsK = PcreConstants.EXTRA_ALLOW_LOOKAROUND_BSK
    }
}
