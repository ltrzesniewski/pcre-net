using NUnit.Framework;
using PCRE.Internal;
using Shouldly;

namespace PCRE.Tests.PcreNet.Support;

[TestFixture]
public class RegexKeyTests
{
    [Test]
    public void should_freeze_settings()
    {
        var settings = new PcreRegexSettings();
        settings.ReadOnlySettings.ShouldBeFalse();

        var key = new RegexKey("test", settings);
        key.Settings.ReadOnlySettings.ShouldBeTrue();
        settings.ReadOnlySettings.ShouldBeFalse();
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
            MaxPatternLength = null,
            MaxPatternCompiledLength = null,
            MaxVarLookbehind = 255
        };

        var keyA = new RegexKey("test", implicitDefaults);
        var keyB = new RegexKey("test", explicitDefaults);

        keyA.ShouldBe(keyB);
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

        keyA.ShouldNotBe(keyB);
    }
}
