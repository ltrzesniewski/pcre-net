namespace PCRE.Tests.Pcre
{
    public class TestCase
    {
        public string TestFile { get; }

        public TestInput Input { get; }
        public TestOutput ExpectedResult { get; }

        public bool Jit { get; }
        public bool Span { get; }

        public TestCase(string testFile, TestInput input, TestOutput expectedResult, bool jit, bool span)
        {
            TestFile = testFile;
            Input = input;
            ExpectedResult = expectedResult;
            Jit = jit;
            Span = span;
        }

        public override string ToString() => Input.ToString();
    }
}
