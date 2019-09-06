using System.Collections.Generic;

namespace PCRE.Tests.Pcre
{
    public class TestCase
    {
        public TestPattern Pattern { get; }
        public IList<string> SubjectLines { get; } = new List<string>();

        public TestCase(TestPattern pattern)
        {
            Pattern = pattern;
        }

        public override string ToString() => Pattern?.ToString() ?? "???";
    }
}
