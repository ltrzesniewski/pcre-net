using System.Collections.Generic;

namespace PCRE.Tests.Pcre;

public class TestInput
{
    public TestPattern Pattern { get; }
    public IList<string> SubjectLines { get; } = new List<string>();

    public TestInput(TestPattern pattern)
    {
        Pattern = pattern;
    }

    public override string ToString() => Pattern.ToString();
}
