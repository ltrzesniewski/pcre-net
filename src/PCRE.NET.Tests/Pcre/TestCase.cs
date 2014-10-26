using System.Collections.Generic;

namespace PCRE.Tests.Pcre
{
    public class TestCase
    {
        public TestPattern Pattern { get; set; }
        public IList<string> SubjectLines { get; private set; }

        public TestCase()
        {
            SubjectLines = new List<string>();
        }

        public override string ToString()
        {
            return Pattern != null ? Pattern.ToString() : "???";
        }
    }
}
