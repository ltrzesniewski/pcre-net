using PCRE.Wrapper;

namespace PCRE
{
    public enum PcreCalloutResult
    {
        Pass = CalloutResult.Pass,
        Fail = CalloutResult.Fail,
        Abort = CalloutResult.NoMatch
    }
}
