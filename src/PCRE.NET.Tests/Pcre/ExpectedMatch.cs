using System.Collections.Generic;

namespace PCRE.Tests.Pcre
{
    public class ExpectedMatch
    {
        public IList<ExpectedGroup> Groups { get; set; }
        public string RemainingString { get; set; }
        public string Mark { get; set; }

        public ExpectedMatch()
        {
            Groups = new List<ExpectedGroup>();
        }
    }
}
