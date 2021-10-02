using PCRE.Internal;

namespace PCRE
{
    /// <summary>
    /// Indicates what character sequences the <c>\R</c> escape sequence matches by default.
    /// </summary>
    public enum PcreBackslashR
    {
        /// <summary>
        /// No information.
        /// </summary>
        Default = 0,

        /// <summary>
        /// <c>PCRE2_BSR_UNICODE</c> - <c>\R</c> matches any Unicode line ending sequence.
        /// </summary>
        Unicode = (int)PcreConstants.BSR_UNICODE,

        /// <summary>
        /// <c>PCRE2_BSR_ANYCRLF</c> - <c>\R</c> matches only CR, LF, or CRLF.
        /// </summary>
        AnyCrLf = (int)PcreConstants.BSR_ANYCRLF
    }
}
