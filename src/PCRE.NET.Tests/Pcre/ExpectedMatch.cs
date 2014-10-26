using System.Collections.Generic;

namespace PCRE.Tests.Pcre
{
    public class ExpectedMatch
    {
        public IList<string> Captures { get; set; }
        public string RemainingString { get; set; }
        public string MarkName { get; set; }

        public ExpectedMatch()
        {
            Captures = new List<string>();
        }
    }
}
