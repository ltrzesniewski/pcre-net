using NUnit.Framework;
using PCRE.Internal;

namespace PCRE.Tests.PcreNet.Support
{
    [TestFixture]
    public class RegexKeyTests
    {
        [Test]
        public void should_freeze_settings()
        {
            var settings = new PcreRegexSettings();
            Assert.That(settings.ReadOnlySettings, Is.False);

            var key = new RegexKey("test", settings);
            Assert.That(key.Settings.ReadOnlySettings, Is.True);
            Assert.That(settings.ReadOnlySettings, Is.False);
        }

        [Test]
        public void should_compare_equal()
        {
            var implicitDefaults = new PcreRegexSettings();
            var explicitDefaults = new PcreRegexSettings
            {
                NewLine = PcreBuildInfo.NewLine,
                BackslashR = PcreBuildInfo.BackslashR,
                ParensLimit = PcreBuildInfo.ParensLimit,
                MaxPatternLength = null
            };

            var keyA = new RegexKey("test", implicitDefaults);
            var keyB = new RegexKey("test", explicitDefaults);

            Assert.That(keyA, Is.EqualTo(keyB));
        }

        [Test]
        public void should_not_compare_equal()
        {
            var defaults = new PcreRegexSettings();
            var other = new PcreRegexSettings
            {
                ParensLimit = 42
            };

            var keyA = new RegexKey("test", defaults);
            var keyB = new RegexKey("test", other);

            Assert.That(keyA, Is.Not.EqualTo(keyB));
        }
    }
}
