using System.Collections.Generic;

namespace PCRE.Tests.Pcre;

public class ExpectedMatch
{
    public IList<ExpectedGroup> Groups { get; } = new List<ExpectedGroup>();
    public string? RemainingString { get; set; }
    public string? Mark { get; set; }
}
