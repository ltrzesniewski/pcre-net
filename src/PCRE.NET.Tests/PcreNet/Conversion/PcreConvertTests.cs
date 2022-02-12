using NUnit.Framework;
using PCRE.Conversion;

namespace PCRE.Tests.PcreNet.Conversion;

[TestFixture]
public class PcreConvertTests
{
    [Test]
    public void should_convert_globs()
    {
        var result = PcreConvert.FromGlob(@"foo\*.bar", PcreGlobConversionOptions.DefaultWindows());
        Assert.That(result, Is.EqualTo(@"(?s)\Afoo\\(*COMMIT)[^\\]*?\.bar\z"));

        var options = PcreGlobConversionOptions.DefaultUnix();
        result = PcreConvert.FromGlob(@"foo/*.bar", options);
        Assert.That(result, Is.EqualTo(@"(?s)\Afoo/(*COMMIT)[^/]*?\.bar\z"));

        result = PcreConvert.FromGlob(@"foo/**.bar", options);
        Assert.That(result, Is.EqualTo(@"(?s)\Afoo/(*COMMIT).*?\.bar\z"));

        options.NoStarStar = true;
        result = PcreConvert.FromGlob(@"foo/**.bar", options);
        Assert.That(result, Is.EqualTo(@"(?s)\Afoo/(*COMMIT)[^/]*?\.bar\z"));

        options.NoWildcardSeparator = true;
        result = PcreConvert.FromGlob(@"foo/*.bar", options);
        Assert.That(result, Is.EqualTo(@"(?s)\Afoo/(*COMMIT).*?\.bar\z"));
    }

    [Test]
    public void should_convert_posix_basic()
    {
        var result = PcreConvert.FromPosixBasic(@"a|b");
        Assert.That(result, Is.EqualTo(@"(*NUL)a\|b"));
    }

    [Test]
    public void should_convert_posix_extended()
    {
        var result = PcreConvert.FromPosixExtended(@"a|b");
        Assert.That(result, Is.EqualTo(@"(*NUL)a|b"));
    }

    [Test]
    public void should_throw_on_syntax_error()
    {
        Assert.Throws<PcreException>(() => PcreConvert.FromPosixBasic(@"[err"));
    }
}
