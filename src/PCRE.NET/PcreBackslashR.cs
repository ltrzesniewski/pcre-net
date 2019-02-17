using PCRE.Internal;

namespace PCRE
{
    public enum PcreBackslashR
    {
        Default = 0,
        Unicode = (int)PcreConstants.BSR_UNICODE,
        AnyCrLf = (int)PcreConstants.BSR_ANYCRLF
    }
}
