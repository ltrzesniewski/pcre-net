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
        /// <c>PCRE2_EXTRA_BAD_ESCAPE_IS_LITERAL</c> - This is a dangerous option. Use with care. By default, an unrecognized escape such as \j or a malformed one such as \x{2z} causes a compile-time error when detected by pcre2_compile(). Perl is somewhat inconsistent in handling such items: for example, \j is treated as a literal "j", and non-hexadecimal digits in \x{} are just ignored, though warnings are given in both cases if Perl's warning switch is enabled. However, a malformed octal number after \o{ always causes an error in Perl.
        /// </summary>
        /// <remarks>
        /// If the PCRE2_EXTRA_BAD_ESCAPE_IS_LITERAL extra option is passed to pcre2_compile(), all unrecognized or malformed escape sequences are treated as single-character escapes. For example, \j is a literal "j" and \x{2z} is treated as the literal string "x{2z}". Setting this option means that typos in patterns may go undetected and have unexpected results. Also note that a sequence such as [\N{] is interpreted as a malformed attempt at [\N{...}] and so is treated as [N{] whereas [\N] gives an error because an unqualified \N is a valid escape sequence but is not supported in a character class. To reiterate: this is a dangerous option. Use with great care.
        /// </remarks>
        BadEscapeIsLiteral = PcreConstants.EXTRA_BAD_ESCAPE_IS_LITERAL,

        /// <summary>
        /// <c>PCRE2_EXTRA_MATCH_WORD</c> - This option is provided for use by the -w option of pcre2grep. It causes the pattern only to match strings that have a word boundary at the start and the end. This is achieved by automatically inserting the code for "\b(?:" at the start of the compiled pattern and ")\b" at the end. The option may be used with PCRE2_LITERAL. However, it is ignored if PCRE2_EXTRA_MATCH_LINE is also set.
        /// </summary>
        MatchWord = PcreConstants.EXTRA_MATCH_WORD,

        /// <summary>
        /// <c>PCRE2_EXTRA_MATCH_LINE</c> - This option is provided for use by the -x option of pcre2grep. It causes the pattern only to match complete lines. This is achieved by automatically inserting the code for "^(?:" at the start of the compiled pattern and ")$" at the end. Thus, when PCRE2_MULTILINE is set, the matched line may be in the middle of the subject string. This option can be used with PCRE2_LITERAL.
        /// </summary>
        MatchLine = PcreConstants.EXTRA_MATCH_LINE,

        /// <summary>
        /// <c>PCRE2_EXTRA_ESCAPED_CR_IS_LF</c> - There are some legacy applications where the escape sequence \r in a pattern is expected to match a newline. If this option is set, \r in a pattern is converted to \n so that it matches a LF (linefeed) instead of a CR (carriage return) character. The option does not affect a literal CR in the pattern, nor does it affect CR specified as an explicit code point such as \x{0D}.
        /// </summary>
        EscapedCrIsLf = PcreConstants.EXTRA_ESCAPED_CR_IS_LF,

        /// <summary>
        /// <c>PCRE2_EXTRA_ALT_BSUX</c> - The original option PCRE2_ALT_BSUX causes PCRE2 to process \U, \u, and \x in the way that ECMAscript (aka JavaScript) does. Additional functionality was defined by ECMAscript 6; setting PCRE2_EXTRA_ALT_BSUX has the effect of PCRE2_ALT_BSUX, but in addition it recognizes \u{hhh..} as a hexadecimal character code, where hhh.. is any number of hexadecimal digits.
        /// </summary>
        AltBsUX = PcreConstants.EXTRA_ALT_BSUX,

        /// <summary>
        /// <c>PCRE2_EXTRA_ALLOW_LOOKAROUND_BSK</c> - Since release 10.38 PCRE2 has forbidden the use of \K within lookaround assertions, following Perl's lead. This option is provided to re-enable the previous behaviour (act in positive lookarounds, ignore in negative ones) in case anybody is relying on it.
        /// </summary>
        AllowLookaroundBsK = PcreConstants.EXTRA_ALLOW_LOOKAROUND_BSK
    }
}
