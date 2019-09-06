using System.Collections.Generic;

namespace PCRE.Tests.Pcre
{
    public class TestOutput
    {
        public TestPattern Pattern { get; }
        public IList<ExpectedResult> ExpectedResults { get; } = new List<ExpectedResult>();

        public TestOutput(TestPattern pattern)
        {
            Pattern = pattern;
        }

        public override string ToString() => Pattern.ToString();
    }
}
