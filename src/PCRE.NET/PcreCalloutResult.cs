using PCRE.Internal;

namespace PCRE
{
    public enum PcreCalloutResult
    {
        Pass = 0,
        Fail = 1,
        Abort = PcreConstants.ERROR_NOMATCH
    }
}
