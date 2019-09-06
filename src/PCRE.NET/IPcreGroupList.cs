using System.Collections.Generic;

namespace PCRE
{
    public interface IPcreGroupList : IReadOnlyList<PcreGroup>
    {
        PcreGroup? this[string name] { get; }
    }
}
