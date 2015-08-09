namespace PCRE.Tests.Pcre
{
    public class ExpectedGroup
    {
        public static readonly ExpectedGroup Unset = new ExpectedGroup();

        public bool IsMatch { get; }
        public string Value { get; }

        private ExpectedGroup()
        {
            IsMatch = false;
            Value = "<unset>";
        }

        public ExpectedGroup(string value)
        {
            Value = value;
            IsMatch = true;
        }
    }
}
