using System;

namespace PCRE;

/// <summary>
/// Options for string splitting.
/// </summary>
[Flags]
public enum PcreSplitOptions : long
{
    /// <summary>
    /// No additional options.
    /// </summary>
    None = 0,

    /// <summary>
    /// Include captured groups as results of the split.
    /// </summary>
    IncludeGroupValues = 1
}
