using PCRE.Internal;

namespace PCRE
{
    /// <summary>
    /// Specifies the result of a callout.
    /// </summary>
    public enum PcreCalloutResult
    {
        /// <summary>
        /// Treat the callout as passing, and continue matching.
        /// </summary>
        Pass = 0,

        /// <summary>
        /// Treat the callout as failing, just as if a lookahead assertion had failed at this point.
        /// </summary>
        Fail = 1,

        /// <summary>
        /// Fail the whole match.
        /// </summary>
        Abort = PcreConstants.ERROR_NOMATCH
    }
}
