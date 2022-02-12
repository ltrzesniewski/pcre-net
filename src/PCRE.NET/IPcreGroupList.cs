using System.Collections.Generic;

namespace PCRE;

/// <summary>
/// The list of capturing groups.
/// </summary>
public interface IPcreGroupList : IReadOnlyList<PcreGroup>
{
    /// <summary>
    /// Returns the capturing group of a given name.
    /// </summary>
    /// <param name="name">The name of the capturing group.</param>
    PcreGroup this[string name] { get; }
}
