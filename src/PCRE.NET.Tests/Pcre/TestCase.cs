using System.Collections.Generic;

namespace PCRE.Tests.Pcre
{
    public class TestCase
    {
        public TestPattern Pattern { get; set; }
        public PcreRegex Regex { get; set; }
        public IList<string> SubjectLines { get; private set; }
        public bool Skip { get; set; }

        public TestCase()
        {
            SubjectLines = new List<string>();
        }
    }
}
