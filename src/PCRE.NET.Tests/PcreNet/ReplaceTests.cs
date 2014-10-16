using System.Globalization;
using NUnit.Framework;

namespace PCRE.Tests.PcreNet
{
    [TestFixture]
    public class ReplaceTests
    {
        [Test]
        public void should_replace_matches_with_callback()
        {
            var re = new PcreRegex(@"a+", PcreOptions.IgnoreCase);
            var result = re.Replace("foo aaa bar aAAa baz", match => match.Length.ToString(CultureInfo.InvariantCulture));

            Assert.That(result, Is.EqualTo("foo 3 b1r 4 b1z"));
        }

        [Test]
        public void should_replace_matches_with_pattern()
        {
            var re = new PcreRegex(@"a+(b+)", PcreOptions.IgnoreCase);
            var result = re.Replace("foo aabb bar aaabbab baz", "<$0><$1>");

            Assert.That(result, Is.EqualTo("foo <aabb><bb> bar <aaabb><bb><ab><b> baz"));
        }

        [Test]
        public void should_not_throw_on_invalid_replacement_patterns()
        {
            var re = new PcreRegex(@"a+(b+)", PcreOptions.IgnoreCase);
            var result = re.Replace("foo aabb bar aaabbab baz", "<$2$$1$>");

            Assert.That(result, Is.EqualTo("foo <$2$bb$> bar <$2$bb$><$2$b$> baz"));
        }
    }
}
