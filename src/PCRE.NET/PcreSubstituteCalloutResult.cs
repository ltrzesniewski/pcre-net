namespace PCRE;

/// <summary>
/// Specifies the result of a substitution callout.
/// </summary>
public enum PcreSubstituteCalloutResult
{
    /// <summary>
    /// Accept the substitution and continue processing if <see cref="PcreSubstituteOptions.SubstituteGlobal"/> was set.
    /// </summary>
    Pass = 0,

    /// <summary>
    /// Reject the current substitution and continue the search.
    /// </summary>
    Fail = 1,

    /// <summary>
    /// Reject the current substitution and stop looking for the next match.
    /// </summary>
    Abort = -1
}
