using System.Collections.Generic;

namespace PCRE.Tests.Pcre
{
    public class TestOutput
    {
        public TestPattern Pattern { get; set; }
        public IList<ExpectedResult> ExpectedResults { get; private set; }

        public TestOutput()
        {
            ExpectedResults = new List<ExpectedResult>();
        }

        public override string ToString()
        {
            return Pattern != null ? Pattern.ToString() : "???";
        }
    }
}
