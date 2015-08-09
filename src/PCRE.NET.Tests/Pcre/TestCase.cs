using System.Collections.Generic;

namespace PCRE.Tests.Pcre
{
    public class TestCase
    {
        public TestPattern Pattern { get; set; }
        public IList<string> SubjectLines { get; }

        public TestCase()
        {
            SubjectLines = new List<string>();
        }

        public override string ToString() => Pattern?.ToString() ?? "???";
    }
}
