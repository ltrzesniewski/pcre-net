namespace PCRE.Tests.Pcre
{
    public class TestCase
    {
        public string TestFile { get; set; }

        public TestInput Input { get; set; }
        public TestOutput ExpectedResult { get; set; }

        public bool Jit { get; set; }
        public bool Span { get; set; }

        public override string ToString() => Input.ToString();
    }
}
