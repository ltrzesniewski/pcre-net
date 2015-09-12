using NUnit.Framework;

namespace PCRE.Tests.PcreNet
{
    [TestFixture]
    public class PcrePatternInfoTests
    {
        [Test]
        public void should_return_pattern_and_options()
        {
            var re = new PcreRegex(@"foo\s+bar", PcreOptions.IgnoreCase);

            Assert.That(re.PaternInfo.PatternString, Is.EqualTo(@"foo\s+bar"));
            Assert.That(re.PaternInfo.Options, Is.EqualTo(PcreOptions.IgnoreCase));
        }

        [Test]
        [TestCase(@"a", 0)]
        [TestCase(@"(a)(b)", 2)]
        [TestCase(@"(a)(b(c))", 3)]
        public void should_return_capture_count(string pattern, int expected)
        {
            var re = new PcreRegex(pattern);
            Assert.That(re.PaternInfo.CaptureCount, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(@"a", 1)]
        [TestCase(@"(a)(b)", 2)]
        [TestCase(@"a{3,5}b", 4)]
        public void should_detect_min_subject_length(string pattern, int expected)
        {
            var re = new PcreRegex(pattern);
            Assert.That(re.PaternInfo.MinSubjectLength, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(@"a")]
        [TestCase(@"ab?ac?")]
        public void should_compile_pattern(string pattern)
        {
            var re = new PcreRegex(pattern, PcreOptions.Compiled);
            Assert.That(re.PaternInfo.IsCompiled, Is.True);
        }

        [Test]
        public void should_enumerate_callouts()
        {
            var re = new PcreRegex(@"a(?C42)bb(?C{ foo })(?:ccc)");

            Assert.That(re.PaternInfo.Callouts.Count, Is.EqualTo(2));

            Assert.That(re.PaternInfo.Callouts[0].Number, Is.EqualTo(42));
            Assert.That(re.PaternInfo.Callouts[0].String, Is.Null);
            Assert.That(re.PaternInfo.Callouts[0].StringOffset, Is.EqualTo(0));
            Assert.That(re.PaternInfo.Callouts[0].PatternPosition, Is.EqualTo(7));
            Assert.That(re.PaternInfo.Callouts[0].NextItemLength, Is.EqualTo(1));

            Assert.That(re.PaternInfo.Callouts[1].Number, Is.EqualTo(0));
            Assert.That(re.PaternInfo.Callouts[1].String, Is.EqualTo(" foo "));
            Assert.That(re.PaternInfo.Callouts[1].StringOffset, Is.EqualTo(13));
            Assert.That(re.PaternInfo.Callouts[1].PatternPosition, Is.EqualTo(20));
            Assert.That(re.PaternInfo.Callouts[1].NextItemLength, Is.EqualTo(7));
        }
    }
}
