using System.Text;
using NUnit.Framework;
using PCRE.Tests.Support;
using Shouldly;
using Shouldly.ShouldlyExtensionMethods;

namespace PCRE.Tests.PcreNet;

[TestFixture]
public class PcrePatternInfoTests
{
    [Test]
    public void should_return_pattern_and_options()
    {
        var re = new PcreRegex(@"foo\s+bar", PcreOptions.IgnoreCase);

        re.PatternInfo.PatternString.ShouldBe(@"foo\s+bar");
        re.PatternInfo.Options.ShouldBe(PcreOptions.IgnoreCase | PcreOptions.Utf);
    }

    [Test]
    public void should_return_pattern_and_options_8bit()
    {
        var re = new PcreRegex8Bit(@"foo\s+bar".ToLatin1Bytes(), TestSupport.Latin1Encoding, PcreOptions.IgnoreCase);

        re.PatternInfo.PatternString.ShouldBe(@"foo\s+bar");
        re.PatternInfo.Options.ShouldBe(PcreOptions.IgnoreCase);
    }

    [Test]
    public void should_return_pattern_and_options_utf8()
    {
        var re = new PcreRegexUtf8(@"foo\s+bar"u8, PcreOptions.IgnoreCase);

        re.PatternInfo.PatternString.ShouldBe(@"foo\s+bar");
        re.PatternInfo.Options.ShouldBe(PcreOptions.IgnoreCase | PcreOptions.Utf);
    }

    [Test]
    public void should_return_strings_with_the_given_encoding()
    {
        var re = new PcreRegex8Bit("foo(?<bar>bär)".ToLatin1Bytes(), Encoding.ASCII);
        re.Encoding.ShouldBeSameAs(Encoding.ASCII);
        re.PatternInfo.PatternString.ShouldBe("foo(?<bar>b?r)");

        re = new PcreRegex8Bit("foo(?<bar>bär)".ToLatin1Bytes(), TestSupport.Latin1Encoding);
        re.Encoding.ShouldBeSameAs(TestSupport.Latin1Encoding);
        re.PatternInfo.PatternString.ShouldBe("foo(?<bar>bär)");
    }

    [Test]
    [TestCase(@"a", 0)]
    [TestCase(@"(a)(b)", 2)]
    [TestCase(@"(a)(b(c))", 3)]
    public void should_return_capture_count(string pattern, int expected)
    {
        var re = new PcreRegex(pattern);
        re.PatternInfo.CaptureCount.ShouldBe(expected);
    }

    [Test]
    [TestCase(@"a", 0)]
    [TestCase(@"(a)(b)", 2)]
    [TestCase(@"(a)(b(c))", 3)]
    public void should_return_capture_count_utf8(string pattern, int expected)
    {
        var re = new PcreRegexUtf8(pattern);
        re.PatternInfo.CaptureCount.ShouldBe(expected);
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
        re.PatternInfo.GroupNames.ShouldBe(expected);
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
        re.PatternInfo.GroupNames.ShouldBe(expected);
    }

    [Test]
    public void should_add_utf_mode_for_16bit()
    {
        var re = new PcreRegex("a");
        re.PatternInfo.Options.ShouldHaveFlag(PcreOptions.Utf);
        re.PatternInfo.Settings.Options.ShouldHaveFlag(PcreOptions.Utf);
        re.PatternInfo.ArgOptions.ShouldHaveFlag(PcreOptions.Utf);
        re.PatternInfo.AllOptions.ShouldHaveFlag(PcreOptions.Utf);
    }

    [Test]
    public void should_not_add_utf_mode_for_8bit()
    {
        var re = new PcreRegex8Bit("a".ToLatin1Bytes(), TestSupport.Latin1Encoding);
        re.PatternInfo.Options.ShouldNotHaveFlag(PcreOptions.Utf);
        re.PatternInfo.Settings.Options.ShouldNotHaveFlag(PcreOptions.Utf);
        re.PatternInfo.ArgOptions.ShouldNotHaveFlag(PcreOptions.Utf);
        re.PatternInfo.AllOptions.ShouldNotHaveFlag(PcreOptions.Utf);
    }

    [Test]
    public void should_add_utf_mode_for_utf8()
    {
        var re = new PcreRegexUtf8("a");
        re.PatternInfo.Options.ShouldHaveFlag(PcreOptions.Utf);
        re.PatternInfo.Settings.Options.ShouldHaveFlag(PcreOptions.Utf);
        re.PatternInfo.ArgOptions.ShouldHaveFlag(PcreOptions.Utf);
        re.PatternInfo.AllOptions.ShouldHaveFlag(PcreOptions.Utf);
    }

    [Test]
    [TestCase(@"a", 1)]
    [TestCase(@"(a)(b)", 2)]
    [TestCase(@"a{3,5}b", 4)]
    public void should_detect_min_subject_length(string pattern, int expected)
    {
        var re = new PcreRegex(pattern);
        re.PatternInfo.MinSubjectLength.ShouldBe((uint)expected);
    }

    [Test]
    [TestCase(@"a", 1)]
    [TestCase(@"(a)(b)", 2)]
    [TestCase(@"a{3,5}b", 4)]
    public void should_detect_min_subject_length_utf8(string pattern, int expected)
    {
        var re = new PcreRegexUtf8(pattern);
        re.PatternInfo.MinSubjectLength.ShouldBe((uint)expected);
    }

    [Test]
    [TestCase(@"a")]
    [TestCase(@"ab?ac?")]
    public void should_compile_pattern(string pattern)
    {
        var re = new PcreRegex(pattern, PcreOptions.Compiled);
        re.PatternInfo.IsCompiled.ShouldBeTrue();
    }

    [Test]
    [TestCase(@"a")]
    [TestCase(@"ab?ac?")]
    public void should_compile_pattern_utf8(string pattern)
    {
        var re = new PcreRegexUtf8(pattern, PcreOptions.Compiled);
        re.PatternInfo.IsCompiled.ShouldBeTrue();
    }

    [Test]
    public void should_enumerate_callouts()
    {
        var re = new PcreRegex(@"a(?C42)bb(?C{ foo })(?:ccc)");

        re.PatternInfo.Callouts.Count.ShouldBe(2);

        re.PatternInfo.Callouts[0].Number.ShouldBe(42);
        re.PatternInfo.Callouts[0].String.ShouldBeNull();
        re.PatternInfo.Callouts[0].StringOffset.ShouldBe(0);
        re.PatternInfo.Callouts[0].PatternPosition.ShouldBe(7);
        re.PatternInfo.Callouts[0].NextPatternItemLength.ShouldBe(1);

        re.PatternInfo.Callouts[1].Number.ShouldBe(0);
        re.PatternInfo.Callouts[1].String.ShouldBe(" foo ");
        re.PatternInfo.Callouts[1].StringOffset.ShouldBe(13);
        re.PatternInfo.Callouts[1].PatternPosition.ShouldBe(20);
        re.PatternInfo.Callouts[1].NextPatternItemLength.ShouldBe(3);
    }

    [Test]
    public void should_enumerate_callouts_utf8()
    {
        var re = new PcreRegexUtf8(@"a(?C42)bb(?C{ foo })(?:ccc)"u8);

        re.PatternInfo.Callouts.Count.ShouldBe(2);

        re.PatternInfo.Callouts[0].Number.ShouldBe(42);
        re.PatternInfo.Callouts[0].String.ShouldBeNull();
        re.PatternInfo.Callouts[0].StringOffset.ShouldBe(0);
        re.PatternInfo.Callouts[0].PatternPosition.ShouldBe(7);
        re.PatternInfo.Callouts[0].NextPatternItemLength.ShouldBe(1);

        re.PatternInfo.Callouts[1].Number.ShouldBe(0);
        re.PatternInfo.Callouts[1].String.ShouldBe(" foo ");
        re.PatternInfo.Callouts[1].StringOffset.ShouldBe(13);
        re.PatternInfo.Callouts[1].PatternPosition.ShouldBe(20);
        re.PatternInfo.Callouts[1].NextPatternItemLength.ShouldBe(3);
    }

    [Test]
    public void should_expose_jit_options()
    {
        var re = new PcreRegex(@"foo", new PcreRegexSettings { JitCompileOptions = PcreJitCompileOptions.PartialSoft });
        re.PatternInfo.JitOptions.ShouldBe(PcreJitCompileOptions.PartialSoft);
    }

    [Test]
    public void should_expose_jit_options_utf8()
    {
        var re = new PcreRegexUtf8(@"foo"u8, new PcreRegexSettings { JitCompileOptions = PcreJitCompileOptions.PartialSoft });
        re.PatternInfo.JitOptions.ShouldBe(PcreJitCompileOptions.PartialSoft);
    }

    [Test]
    public void should_convert_jit_options()
    {
        var compiled = new PcreRegex(@"foo", PcreOptions.Compiled);
        compiled.PatternInfo.JitOptions.ShouldBe(PcreJitCompileOptions.Complete);

        var partial = new PcreRegex(@"foo", PcreOptions.CompiledPartial);
        partial.PatternInfo.JitOptions.ShouldBe(PcreJitCompileOptions.PartialHard | PcreJitCompileOptions.PartialSoft);
    }

    [Test]
    public void should_convert_jit_options_utf8()
    {
        var compiled = new PcreRegexUtf8(@"foo"u8, PcreOptions.Compiled);
        compiled.PatternInfo.JitOptions.ShouldBe(PcreJitCompileOptions.Complete);

        var partial = new PcreRegexUtf8(@"foo"u8, PcreOptions.CompiledPartial);
        partial.PatternInfo.JitOptions.ShouldBe(PcreJitCompileOptions.PartialHard | PcreJitCompileOptions.PartialSoft);
    }
}
