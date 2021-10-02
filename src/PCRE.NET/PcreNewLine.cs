using PCRE.Internal;

namespace PCRE
{
    /// <summary>
    /// Specifies which character sequence is treated as a newline.
    /// </summary>
    public enum PcreNewLine
    {
        /// <summary>
        /// No information.
        /// </summary>
        Default = 0,

        /// <summary>
        /// <c>PCRE2_NEWLINE_CR</c> - Carriage return (CR).
        /// </summary>
        Cr = (int)PcreConstants.NEWLINE_CR,

        /// <summary>
        /// <c>PCRE2_NEWLINE_LF</c> - Linefeed (LF).
        /// </summary>
        Lf = (int)PcreConstants.NEWLINE_LF,

        /// <summary>
        /// <c>PCRE2_NEWLINE_CRLF</c> - Carriage return, linefeed (CRLF).
        /// </summary>
        CrLf = (int)PcreConstants.NEWLINE_CRLF,

        /// <summary>
        /// <c>PCRE2_NEWLINE_ANY</c> - Any Unicode line ending.
        /// </summary>
        Any = (int)PcreConstants.NEWLINE_ANY,

        /// <summary>
        /// <c>PCRE2_NEWLINE_ANYCRLF</c> - Any of CR, LF, or CRLF.
        /// </summary>
        AnyCrLf = (int)PcreConstants.NEWLINE_ANYCRLF,

        /// <summary>
        /// <c>PCRE2_NEWLINE_NUL</c> - The NUL character (binary zero).
        /// </summary>
        Nul = (int)PcreConstants.NEWLINE_NUL,
    }
}
