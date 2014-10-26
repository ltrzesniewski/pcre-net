using System.Collections.Generic;

namespace PCRE.Tests.Pcre
{
    public class ExpectedResult
    {
        public string SubjectLine { get; set; }
        public IList<ExpectedMatch> Matches { get; set; }

        public ExpectedResult()
        {
            Matches = new List<ExpectedMatch>();
        }

        public override string ToString()
        {
            return SubjectLine;
        }
    }
}
