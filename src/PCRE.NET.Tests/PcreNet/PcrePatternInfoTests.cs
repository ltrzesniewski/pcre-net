using System.Text;
using NUnit.Framework;
using PCRE.Tests.Support;

namespace PCRE.Tests.PcreNet;

[TestFixture]
public class PcrePatternInfoTests
{
    [Test]
    public void should_return_pattern_and_options()
    {
        var re = new PcreRegex(@"foo\s+bar", PcreOptions.IgnoreCase);

        Assert.That(re.PatternInfo.PatternString, Is.EqualTo(@"foo\s+bar"));
        Assert.That(re.PatternInfo.Options, Is.EqualTo(PcreOptions.IgnoreCase | PcreOptions.Utf));
    }

    [Test]
    public void should_return_pattern_and_options_8bit()
    {
        var re = new PcreRegex8Bit(@"foo\s+bar".ToLatin1Bytes(), TestSupport.Latin1Encoding, PcreOptions.IgnoreCase);

        Assert.That(re.PatternInfo.PatternString, Is.EqualTo(@"foo\s+bar"));
        Assert.That(re.PatternInfo.Options, Is.EqualTo(PcreOptions.IgnoreCase));
    }

    [Test]
    public void should_return_pattern_and_options_utf8()
    {
        var re = new PcreRegexUtf8(@"foo\s+bar"u8, PcreOptions.IgnoreCase);

        Assert.That(re.PatternInfo.PatternString, Is.EqualTo(@"foo\s+bar"));
        Assert.That(re.PatternInfo.Options, Is.EqualTo(PcreOptions.IgnoreCase | PcreOptions.Utf));
    }

    [Test]
    public void should_return_strings_with_the_given_encoding()
    {
        var re = new PcreRegex8Bit("foo(?<bar>bär)".ToLatin1Bytes(), Encoding.ASCII);
        Assert.That(re.Encoding, Is.SameAs(Encoding.ASCII));
        Assert.That(re.PatternInfo.PatternString, Is.EqualTo("foo(?<bar>b?r)"));

        re = new PcreRegex8Bit("foo(?<bar>bär)".ToLatin1Bytes(), TestSupport.Latin1Encoding);
        Assert.That(re.Encoding, Is.SameAs(TestSupport.Latin1Encoding));
        Assert.That(re.PatternInfo.PatternString, Is.EqualTo("foo(?<bar>bär)"));
    }

    [Test]
    [TestCase(@"a", 0)]
    [TestCase(@"(a)(b)", 2)]
    [TestCase(@"(a)(b(c))", 3)]
    public void should_return_capture_count(string pattern, int expected)
    {
        var re = new PcreRegex(pattern);
        Assert.That(re.PatternInfo.CaptureCount, Is.EqualTo(expected));
    }

    [Test]
    [TestCase(@"a", 0)]
    [TestCase(@"(a)(b)", 2)]
    [TestCase(@"(a)(b(c))", 3)]
    public void should_return_capture_count_utf8(string pattern, int expected)
    {
        var re = new PcreRegexUtf8(pattern);
        Assert.That(re.PatternInfo.CaptureCount, Is.EqualTo(expected));
    }

    [Test]
    [TestCase(@"a", new string[0])]
    [TestCase(@"(a)", new string[0])]
    [TestCase(@"(?<foo>a)", new[] { "foo" })]
    [TestCase(@"(?<zzz>a)(?<aaa>b)", new[] { "zzz", "aaa" })]
    [TestCase(@"(?J)(?<foo>a)(?<foo>b)", new[] { "foo" })]
    [TestCase(@"(?|(?<foo>a)|(?<foo>b))", new[] { "foo" })]
    public void should_return_group_names(string pattern, string[] expected)
    {
        var re = new PcreRegex(pattern);
        Assert.That(re.PatternInfo.GroupNames, Is.EqualTo(expected));
    }

    [Test]
    [TestCase(@"a", new string[0])]
    [TestCase(@"(a)", new string[0])]
    [TestCase(@"(?<foo>a)", new[] { "foo" })]
    [TestCase(@"(?<zzz>a)(?<aaa>b)", new[] { "zzz", "aaa" })]
    [TestCase(@"(?J)(?<foo>a)(?<foo>b)", new[] { "foo" })]
    [TestCase(@"(?|(?<foo>a)|(?<foo>b))", new[] { "foo" })]
    public void should_return_group_names_utf8(string pattern, string[] expected)
    {
        var re = new PcreRegexUtf8(pattern);
        Assert.That(re.PatternInfo.GroupNames, Is.EqualTo(expected));
    }

    [Test]
    public void should_add_utf_mode_for_16bit()
    {
        var re = new PcreRegex("a");
        Assert.That(re.PatternInfo.Options.HasFlag(PcreOptions.Utf), Is.True);
        Assert.That(re.PatternInfo.Settings.Options.HasFlag(PcreOptions.Utf), Is.True);
        Assert.That(re.PatternInfo.ArgOptions.HasFlag(PcreOptions.Utf), Is.True);
        Assert.That(re.PatternInfo.AllOptions.HasFlag(PcreOptions.Utf), Is.True);
    }

    [Test]
    public void should_not_add_utf_mode_for_8bit()
    {
        var re = new PcreRegex8Bit("a".ToLatin1Bytes(), TestSupport.Latin1Encoding);
        Assert.That(re.PatternInfo.Options.HasFlag(PcreOptions.Utf), Is.False);
        Assert.That(re.PatternInfo.Settings.Options.HasFlag(PcreOptions.Utf), Is.False);
        Assert.That(re.PatternInfo.ArgOptions.HasFlag(PcreOptions.Utf), Is.False);
        Assert.That(re.PatternInfo.AllOptions.HasFlag(PcreOptions.Utf), Is.False);
    }

    [Test]
    public void should_add_utf_mode_for_utf8()
    {
        var re = new PcreRegexUtf8("a");
        Assert.That(re.PatternInfo.Options.HasFlag(PcreOptions.Utf), Is.True);
        Assert.That(re.PatternInfo.Settings.Options.HasFlag(PcreOptions.Utf), Is.True);
        Assert.That(re.PatternInfo.ArgOptions.HasFlag(PcreOptions.Utf), Is.True);
        Assert.That(re.PatternInfo.AllOptions.HasFlag(PcreOptions.Utf), Is.True);
    }

    [Test]
    [TestCase(@"a", 1)]
    [TestCase(@"(a)(b)", 2)]
    [TestCase(@"a{3,5}b", 4)]
    public void should_detect_min_subject_length(string pattern, int expected)
    {
        var re = new PcreRegex(pattern);
        Assert.That(re.PatternInfo.MinSubjectLength, Is.EqualTo(expected));
    }

    [Test]
    [TestCase(@"a", 1)]
    [TestCase(@"(a)(b)", 2)]
    [TestCase(@"a{3,5}b", 4)]
    public void should_detect_min_subject_length_utf8(string pattern, int expected)
    {
        var re = new PcreRegexUtf8(pattern);
        Assert.That(re.PatternInfo.MinSubjectLength, Is.EqualTo(expected));
    }

    [Test]
    [TestCase(@"a")]
    [TestCase(@"ab?ac?")]
    public void should_compile_pattern(string pattern)
    {
        var re = new PcreRegex(pattern, PcreOptions.Compiled);
        Assert.That(re.PatternInfo.IsCompiled, Is.True);
    }

    [Test]
    [TestCase(@"a")]
    [TestCase(@"ab?ac?")]
    public void should_compile_pattern_utf8(string pattern)
    {
        var re = new PcreRegexUtf8(pattern, PcreOptions.Compiled);
        Assert.That(re.PatternInfo.IsCompiled, Is.True);
    }

    [Test]
    public void should_enumerate_callouts()
    {
        var re = new PcreRegex(@"a(?C42)bb(?C{ foo })(?:ccc)");

        Assert.That(re.PatternInfo.Callouts.Count, Is.EqualTo(2));

        Assert.That(re.PatternInfo.Callouts[0].Number, Is.EqualTo(42));
        Assert.That(re.PatternInfo.Callouts[0].String, Is.Null);
        Assert.That(re.PatternInfo.Callouts[0].StringOffset, Is.EqualTo(0));
        Assert.That(re.PatternInfo.Callouts[0].PatternPosition, Is.EqualTo(7));
        Assert.That(re.PatternInfo.Callouts[0].NextPatternItemLength, Is.EqualTo(1));

        Assert.That(re.PatternInfo.Callouts[1].Number, Is.EqualTo(0));
        Assert.That(re.PatternInfo.Callouts[1].String, Is.EqualTo(" foo "));
        Assert.That(re.PatternInfo.Callouts[1].StringOffset, Is.EqualTo(13));
        Assert.That(re.PatternInfo.Callouts[1].PatternPosition, Is.EqualTo(20));
        Assert.That(re.PatternInfo.Callouts[1].NextPatternItemLength, Is.EqualTo(3));
    }

    [Test]
    public void should_enumerate_callouts_utf8()
    {
        var re = new PcreRegexUtf8(@"a(?C42)bb(?C{ foo })(?:ccc)"u8);

        Assert.That(re.PatternInfo.Callouts.Count, Is.EqualTo(2));

        Assert.That(re.PatternInfo.Callouts[0].Number, Is.EqualTo(42));
        Assert.That(re.PatternInfo.Callouts[0].String, Is.Null);
        Assert.That(re.PatternInfo.Callouts[0].StringOffset, Is.EqualTo(0));
        Assert.That(re.PatternInfo.Callouts[0].PatternPosition, Is.EqualTo(7));
        Assert.That(re.PatternInfo.Callouts[0].NextPatternItemLength, Is.EqualTo(1));

        Assert.That(re.PatternInfo.Callouts[1].Number, Is.EqualTo(0));
        Assert.That(re.PatternInfo.Callouts[1].String, Is.EqualTo(" foo "));
        Assert.That(re.PatternInfo.Callouts[1].StringOffset, Is.EqualTo(13));
        Assert.That(re.PatternInfo.Callouts[1].PatternPosition, Is.EqualTo(20));
        Assert.That(re.PatternInfo.Callouts[1].NextPatternItemLength, Is.EqualTo(3));
    }

    [Test]
    public void should_expose_jit_options()
    {
        var re = new PcreRegex(@"foo", new PcreRegexSettings { JitCompileOptions = PcreJitCompileOptions.PartialSoft });
        Assert.That(re.PatternInfo.JitOptions, Is.EqualTo(PcreJitCompileOptions.PartialSoft));
    }

    [Test]
    public void should_expose_jit_options_utf8()
    {
        var re = new PcreRegexUtf8(@"foo"u8, new PcreRegexSettings { JitCompileOptions = PcreJitCompileOptions.PartialSoft });
        Assert.That(re.PatternInfo.JitOptions, Is.EqualTo(PcreJitCompileOptions.PartialSoft));
    }

    [Test]
    public void should_convert_jit_options()
    {
        var compiled = new PcreRegex(@"foo", PcreOptions.Compiled);
        Assert.That(compiled.PatternInfo.JitOptions, Is.EqualTo(PcreJitCompileOptions.Complete));

        var partial = new PcreRegex(@"foo", PcreOptions.CompiledPartial);
        Assert.That(partial.PatternInfo.JitOptions, Is.EqualTo(PcreJitCompileOptions.PartialHard | PcreJitCompileOptions.PartialSoft));
    }

    [Test]
    public void should_convert_jit_options_utf8()
    {
        var compiled = new PcreRegexUtf8(@"foo"u8, PcreOptions.Compiled);
        Assert.That(compiled.PatternInfo.JitOptions, Is.EqualTo(PcreJitCompileOptions.Complete));

        var partial = new PcreRegexUtf8(@"foo"u8, PcreOptions.CompiledPartial);
        Assert.That(partial.PatternInfo.JitOptions, Is.EqualTo(PcreJitCompileOptions.PartialHard | PcreJitCompileOptions.PartialSoft));
    }
}
