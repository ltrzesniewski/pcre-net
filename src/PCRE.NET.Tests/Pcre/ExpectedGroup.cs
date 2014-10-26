namespace PCRE.Tests.Pcre
{
    public class ExpectedGroup
    {
        public static readonly ExpectedGroup Unset = new ExpectedGroup();

        public bool IsMatch { get; private set; }
        public string Value { get; private set; }

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
