using NUnit.Framework;

namespace PCRE.Tests.Pcre
{
    [TestFixture]
    public class RunnerTests
    {
        [Test]
        [TestCase(@"foo\tbar", Result = "foo\tbar")]
        [TestCase(@"foo \r\n bar", Result = "foo \r\n bar")]
        [TestCase(@"foo\Ybar", Result = @"fooYbar")]
        [TestCase(@"foo \x41 bar", Result = @"foo A bar")]
        [TestCase(@"foo \073 bar", Result = @"foo ; bar")]
        [TestCase(@"\0", Result = "\0")]
        [TestCase(@"\00", Result = "\0")]
        [TestCase(@"\000", Result = "\0")]
        [TestCase(@"\0000", Result = "\0" + "0")]
        [TestCase(@"\100", Result = "@")]
        [TestCase(@"\x41", Result = @"A")]
        [TestCase(@"\xA", Result = "\n")]
        [TestCase(@"\xAz", Result = "\nz")]
        [TestCase(@"\x{A}z", Result = "\nz")]
        [TestCase(@"\x{0A}z", Result = "\nz")]
        [TestCase(@"\x{00A}z", Result = "\nz")]
        [TestCase(@"\x{000A}z", Result = "\nz")]
        [TestCase(@"a\\z", Result = "a\\z")]
        public string should_unescape_subject(string input)
        {
            return input.UnescapeSubject();
        }

        [Test]
        [TestCase(@"$\?", Result = @"$\?")]
        [TestCase(@"\xA", Result = "\n")]
        [TestCase(@"\xAz", Result = "\nz")]
        [TestCase(@"\x{A}z", Result = "\nz")]
        [TestCase(@"\x{0A}z", Result = "\nz")]
        [TestCase(@"\x{00A}z", Result = "\nz")]
        [TestCase(@"\x{000A}z", Result = "\nz")]
        public string should_unescape_group(string input)
        {
            return input.UnescapeGroup();
        }
    }
}
