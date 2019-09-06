using NUnit.Framework;

namespace PCRE.Tests.Pcre
{
    [TestFixture]
    public class RunnerTests
    {
        [Test]
        [TestCase(@"foo\tbar", ExpectedResult = "foo\tbar")]
        [TestCase(@"foo \r\n bar", ExpectedResult = "foo \r\n bar")]
        [TestCase(@"foo\Ybar", ExpectedResult = @"fooYbar")]
        [TestCase(@"foo \x41 bar", ExpectedResult = @"foo A bar")]
        [TestCase(@"foo \073 bar", ExpectedResult = @"foo ; bar")]
        [TestCase(@"\0", ExpectedResult = "\0")]
        [TestCase(@"\00", ExpectedResult = "\0")]
        [TestCase(@"\000", ExpectedResult = "\0")]
        [TestCase(@"\0000", ExpectedResult = "\0" + "0")]
        [TestCase(@"\100", ExpectedResult = "@")]
        [TestCase(@"\x41", ExpectedResult = @"A")]
        [TestCase(@"\xA", ExpectedResult = "\n")]
        [TestCase(@"\xAz", ExpectedResult = "\nz")]
        [TestCase(@"\x{A}z", ExpectedResult = "\nz")]
        [TestCase(@"\x{0A}z", ExpectedResult = "\nz")]
        [TestCase(@"\x{00A}z", ExpectedResult = "\nz")]
        [TestCase(@"\x{000A}z", ExpectedResult = "\nz")]
        [TestCase(@"a\\z", ExpectedResult = "a\\z")]
        public string should_unescape_subject(string input)
        {
            return input.UnescapeSubject();
        }

        [Test]
        [TestCase(@"$\?", ExpectedResult = @"$\?")]
        [TestCase(@"\xA", ExpectedResult = "\n")]
        [TestCase(@"\xAz", ExpectedResult = "\nz")]
        [TestCase(@"\x{A}z", ExpectedResult = "\nz")]
        [TestCase(@"\x{0A}z", ExpectedResult = "\nz")]
        [TestCase(@"\x{00A}z", ExpectedResult = "\nz")]
        [TestCase(@"\x{000A}z", ExpectedResult = "\nz")]
        public string should_unescape_group(string input)
        {
            return input.UnescapeGroup()!;
        }
    }
}
