using System.Collections.Generic;

namespace PCRE.Tests.Pcre;

public class ExpectedResult
{
    public string SubjectLine { get; set; }
    public IList<ExpectedMatch> Matches { get; } = new List<ExpectedMatch>();

    public ExpectedResult(string subjectLine)
    {
        SubjectLine = subjectLine;
    }

    public override string ToString() => SubjectLine;
}
