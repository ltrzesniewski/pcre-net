using System.Collections.Generic;

namespace PCRE.Tests.Pcre
{
    public class TestCase
    {
        public TestPattern Pattern { get; set; }
        public PcreRegex Regex { get; set; }
        public IList<string> RawSubjectLines { get; private set; }
        public IList<string> SubjectLines { get; private set; }
        public bool Skip { get; set; }

        public TestCase()
        {
            RawSubjectLines = new List<string>();
            SubjectLines = new List<string>();
        }

        public override string ToString()
        {
            return Pattern != null ? Pattern.ToString() : "???";
        }
    }
}
