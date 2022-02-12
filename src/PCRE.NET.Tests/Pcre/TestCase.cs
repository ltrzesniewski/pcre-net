namespace PCRE.Tests.Pcre;

public class TestCase
{
    public string TestFile { get; }

    public TestInput Input { get; }
    public TestOutput ExpectedResult { get; }

    public bool Jit { get; }
    public ApiKind ApiKind { get; }

    public TestCase(string testFile, TestInput input, TestOutput expectedResult, bool jit, ApiKind apiKind)
    {
        TestFile = testFile;
        Input = input;
        ExpectedResult = expectedResult;
        Jit = jit;
        ApiKind = apiKind;
    }

    public override string ToString() => Input.ToString();
}

public enum ApiKind
{
    String,
    Span,
    MatchBuffer
}
