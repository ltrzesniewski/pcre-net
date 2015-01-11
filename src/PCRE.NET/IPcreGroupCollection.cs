using System.Collections.Generic;

namespace PCRE
{
    public interface IPcreGroupCollection : IReadOnlyCollection<PcreGroup>
    {
        PcreGroup this[int index] { get; }
        PcreGroup this[string name] { get; }
    }
}
