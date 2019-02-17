using PCRE.Internal;

namespace PCRE
{
    public enum PcreNewLine
    {
        Default = 0,
        Cr = (int)PcreConstants.NEWLINE_CR,
        Lf = (int)PcreConstants.NEWLINE_LF,
        CrLf = (int)PcreConstants.NEWLINE_CRLF,
        Any = (int)PcreConstants.NEWLINE_ANY,
        AnyCrLf = (int)PcreConstants.NEWLINE_ANYCRLF,
        Nul = (int)PcreConstants.NEWLINE_NUL
    }
}
