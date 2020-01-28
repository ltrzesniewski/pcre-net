using System.Collections.Generic;

namespace PCRE.Tests.Pcre
{
    public class TestInput
    {
        public TestPattern Pattern { get; set; }
        public IList<string> SubjectLines { get; }

        public TestInput()
        {
            SubjectLines = new List<string>();
        }

        public override string ToString() => Pattern?.ToString() ?? "???";
    }
}
