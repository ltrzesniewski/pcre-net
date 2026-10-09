using System;
using NUnit.Framework;
using PCRE.Tests.Support;
using Shouldly;

namespace PCRE.Tests.PcreNet;

[TestFixture]
public class PcreRefMatchTests
{
    [Test]
    public void should_return_matched_string()
    {
        var re = new PcreRegex(".");
        var match = re.Match("ab".AsSpan());

        match.ToString().ShouldBe("a");
    }

    [Test]
    public void should_return_matched_string_utf8()
    {
        var re = new PcreRegexUtf8("."u8);
        var match = re.Match("ab"u8);

        match.ToString().ShouldBe("a");
    }

    [Test]
    public void should_return_matched_string_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(".".ToLatin1Bytes());
        var match = re.Match("ab".ToLatin1Bytes());

        match.ToString().ShouldBe("a");
    }

    [Test]
    public void should_return_matched_string_from_group()
    {
        var re = new PcreRegex(".");
        var match = re.Match("ab".AsSpan());

        match[0].ToString().ShouldBe("a");
    }

    [Test]
    public void should_return_matched_string_from_group_utf8()
    {
        var re = new PcreRegexUtf8("."u8);
        var match = re.Match("ab"u8);

        match[0].ToString().ShouldBe("a");
    }

    [Test]
    public void should_return_matched_string_from_group_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(".".ToLatin1Bytes());
        var match = re.Match("ab".ToLatin1Bytes());

        match[0].ToString().ShouldBe("a");
    }

    [Test]
    public void should_cast_group_to_string()
    {
        var re = new PcreRegex(".");
        var match = re.Match("ab".AsSpan());

        ((string)match[0]).ShouldBe("a");
    }

    [Test]
    public void should_cast_group_to_string_utf8()
    {
        var re = new PcreRegexUtf8("."u8);
        var match = re.Match("ab"u8);

        ((string)match[0]).ShouldBe("a");
    }

    [Test]
    public void should_cast_group_to_string_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(".".ToLatin1Bytes());
        var match = re.Match("ab".ToLatin1Bytes());

        ((string)match[0]).ShouldBe("a");
    }

    [Test]
    public void should_enumerate_groups()
    {
        var re = new PcreRegex("(.)(?<name>.)");
        var match = re.Match("ab".AsSpan());

        var enumerator = match.Groups.GetEnumerator();

        enumerator.MoveNext().ShouldBeTrue();
        enumerator.Current.Value.ShouldBe("ab");

        enumerator.MoveNext().ShouldBeTrue();
        enumerator.Current.Value.ShouldBe("a");

        enumerator.MoveNext().ShouldBeTrue();
        enumerator.Current.Value.ShouldBe("b");

        enumerator.MoveNext().ShouldBeFalse();
        enumerator.MoveNext().ShouldBeFalse();
    }

    [Test]
    public void should_enumerate_groups_utf8()
    {
        var re = new PcreRegexUtf8("(.)(?<name>.)"u8);
        var match = re.Match("ab"u8);

        var enumerator = match.Groups.GetEnumerator();

        enumerator.MoveNext().ShouldBeTrue();
        enumerator.Current.Value.ShouldBe("ab"u8);

        enumerator.MoveNext().ShouldBeTrue();
        enumerator.Current.Value.ShouldBe("a"u8);

        enumerator.MoveNext().ShouldBeTrue();
        enumerator.Current.Value.ShouldBe("b"u8);

        enumerator.MoveNext().ShouldBeFalse();
        enumerator.MoveNext().ShouldBeFalse();
    }

    [Test]
    public void should_enumerate_groups_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit("(.)(?<name>.)".ToLatin1Bytes());
        var match = re.Match("ab".ToLatin1Bytes());

        var enumerator = match.Groups.GetEnumerator();

        enumerator.MoveNext().ShouldBeTrue();
        enumerator.Current.Value.ShouldBe("ab".ToLatin1Bytes());

        enumerator.MoveNext().ShouldBeTrue();
        enumerator.Current.Value.ShouldBe("a".ToLatin1Bytes());

        enumerator.MoveNext().ShouldBeTrue();
        enumerator.Current.Value.ShouldBe("b".ToLatin1Bytes());

        enumerator.MoveNext().ShouldBeFalse();
        enumerator.MoveNext().ShouldBeFalse();
    }

    [Test]
    public void should_enumerate_groups_directly()
    {
        var re = new PcreRegex("(.)(?<name>.)");
        var match = re.Match("ab".AsSpan());

        var enumerator = match.GetEnumerator();

        enumerator.MoveNext().ShouldBeTrue();
        enumerator.Current.Value.ShouldBe("ab");

        enumerator.MoveNext().ShouldBeTrue();
        enumerator.Current.Value.ShouldBe("a");

        enumerator.MoveNext().ShouldBeTrue();
        enumerator.Current.Value.ShouldBe("b");

        enumerator.MoveNext().ShouldBeFalse();
        enumerator.MoveNext().ShouldBeFalse();
    }

    [Test]
    public void should_enumerate_groups_directly_utf8()
    {
        var re = new PcreRegexUtf8("(.)(?<name>.)"u8);
        var match = re.Match("ab"u8);

        var enumerator = match.GetEnumerator();

        enumerator.MoveNext().ShouldBeTrue();
        enumerator.Current.Value.ShouldBe("ab"u8);

        enumerator.MoveNext().ShouldBeTrue();
        enumerator.Current.Value.ShouldBe("a"u8);

        enumerator.MoveNext().ShouldBeTrue();
        enumerator.Current.Value.ShouldBe("b"u8);

        enumerator.MoveNext().ShouldBeFalse();
        enumerator.MoveNext().ShouldBeFalse();
    }

    [Test]
    public void should_enumerate_groups_directly_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit("(.)(?<name>.)".ToLatin1Bytes());
        var match = re.Match("ab".ToLatin1Bytes());

        var enumerator = match.GetEnumerator();

        enumerator.MoveNext().ShouldBeTrue();
        enumerator.Current.Value.ShouldBe("ab".ToLatin1Bytes());

        enumerator.MoveNext().ShouldBeTrue();
        enumerator.Current.Value.ShouldBe("a".ToLatin1Bytes());

        enumerator.MoveNext().ShouldBeTrue();
        enumerator.Current.Value.ShouldBe("b".ToLatin1Bytes());

        enumerator.MoveNext().ShouldBeFalse();
        enumerator.MoveNext().ShouldBeFalse();
    }

    [Test]
    public void should_get_group_list()
    {
        var re = new PcreRegex("(.)(?<name>.)");
        var values = re.Match("ab".AsSpan()).Groups.ToList(i => i.Value.ToString());

        values.ShouldBe(["ab", "a", "b"]);
    }

    [Test]
    public void should_get_group_list_utf8()
    {
        var re = new PcreRegexUtf8("(.)(?<name>.)"u8);
        var values = re.Match("ab"u8).Groups.ToList(i => i.ToString());

        values.ShouldBe(["ab", "a", "b"]);
    }

    [Test]
    public void should_get_group_list_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit("(.)(?<name>.)".ToLatin1Bytes());
        var values = re.Match("ab".ToLatin1Bytes()).Groups.ToList(i => i.ToString());

        values.ShouldBe(["ab", "a", "b"]);
    }

    [Test]
    public void should_have_undefined_value_in_default_ref_match()
    {
        var match = default(PcreRefMatch);

        match.Success.ShouldBeFalse();
        match.CaptureCount.ShouldBe(0);
        match.Value.ToString().ShouldBeSameAs(string.Empty);
        match.Index.ShouldBe(-1);
        match.EndIndex.ShouldBe(-1);
        match.Length.ShouldBe(0);
        match.IsPartialMatch.ShouldBeFalse();
        match.Mark.IsEmpty.ShouldBeTrue();

        match[0].Success.ShouldBeFalse();
        match[0].IsDefined.ShouldBeFalse();
        match[0].Value.ToString().ShouldBeSameAs(string.Empty);
        match[0].Index.ShouldBe(-1);
        match[0].EndIndex.ShouldBe(-1);
        match[0].Length.ShouldBe(0);
    }

    [Test]
    public void should_have_undefined_value_in_default_8bit_match()
    {
        var match = default(PcreRefMatch8Bit);

        match.Success.ShouldBeFalse();
        match.CaptureCount.ShouldBe(0);
        match.Value.IsEmpty.ShouldBeTrue();
        match.Index.ShouldBe(-1);
        match.EndIndex.ShouldBe(-1);
        match.Length.ShouldBe(0);
        match.IsPartialMatch.ShouldBeFalse();
        match.Mark.IsEmpty.ShouldBeTrue();

        match[0].Success.ShouldBeFalse();
        match[0].IsDefined.ShouldBeFalse();
        match[0].Value.IsEmpty.ShouldBeTrue();
        match[0].Index.ShouldBe(-1);
        match[0].EndIndex.ShouldBe(-1);
        match[0].Length.ShouldBe(0);
    }

    [Test]
    public void should_copy_ref_match()
    {
        var re = new PcreRegex(".");
        var enumerator = re.Matches("ab".AsSpan()).GetEnumerator();

        enumerator.MoveNext().ShouldBeTrue();

        var copy = enumerator.Current;

        enumerator.Current.Value.ShouldBe("a");
        copy.Value.ShouldBe("a");

        enumerator.MoveNext().ShouldBeTrue();

        enumerator.Current.Value.ShouldBe("b");
        copy.Value.ShouldBe("a");
    }

    [Test]
    public void should_copy_utf8_match()
    {
        var re = new PcreRegexUtf8("."u8);
        var enumerator = re.Matches("ab"u8).GetEnumerator();

        enumerator.MoveNext().ShouldBeTrue();

        var copy = enumerator.Current;

        enumerator.Current.Value.ShouldBe("a"u8);
        copy.Value.ShouldBe("a"u8);

        enumerator.MoveNext().ShouldBeTrue();

        enumerator.Current.Value.ShouldBe("b"u8);
        copy.Value.ShouldBe("a"u8);
    }

    [Test]
    public void should_copy_8bit_match()
    {
        var re = TestSupport.CreatePcreRegex8Bit(".".ToLatin1Bytes());
        var enumerator = re.Matches("ab".ToLatin1Bytes()).GetEnumerator();

        enumerator.MoveNext().ShouldBeTrue();

        var copy = enumerator.Current;

        enumerator.Current.Value.ShouldBe("a".ToLatin1Bytes());
        copy.Value.ShouldBe("a".ToLatin1Bytes());

        enumerator.MoveNext().ShouldBeTrue();

        enumerator.Current.Value.ShouldBe("b".ToLatin1Bytes());
        copy.Value.ShouldBe("a".ToLatin1Bytes());
    }

    [Test]
    public void should_support_move_next_after_all_matches_are_exhausted()
    {
        var re = new PcreRegex(".");
        var enumerator = re.Matches("ab".AsSpan()).GetEnumerator();

        enumerator.MoveNext().ShouldBeTrue();
        enumerator.MoveNext().ShouldBeTrue();
        enumerator.MoveNext().ShouldBeFalse();
        enumerator.MoveNext().ShouldBeFalse();
    }

    [Test]
    public void should_support_move_next_after_all_matches_are_exhausted_utf8()
    {
        var re = new PcreRegexUtf8("."u8);
        var enumerator = re.Matches("ab"u8).GetEnumerator();

        enumerator.MoveNext().ShouldBeTrue();
        enumerator.MoveNext().ShouldBeTrue();
        enumerator.MoveNext().ShouldBeFalse();
        enumerator.MoveNext().ShouldBeFalse();
    }

    [Test]
    public void should_support_move_next_after_all_matches_are_exhausted_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(".".ToLatin1Bytes());
        var enumerator = re.Matches("ab".ToLatin1Bytes()).GetEnumerator();

        enumerator.MoveNext().ShouldBeTrue();
        enumerator.MoveNext().ShouldBeTrue();
        enumerator.MoveNext().ShouldBeFalse();
        enumerator.MoveNext().ShouldBeFalse();
    }

    [Test]
    public void should_support_indexed_groups_on_default_match()
    {
        var match = default(PcreRefMatch);
        var groupA = match[0];
        var groupB = match.Groups[0];

        groupA.IsDefined.ShouldBeFalse();
        groupB.IsDefined.ShouldBeFalse();
    }

    [Test]
    public void should_support_indexed_groups_on_default_match_8bit()
    {
        var match = default(PcreRefMatch8Bit);
        var groupA = match[0];
        var groupB = match.Groups[0];

        groupA.IsDefined.ShouldBeFalse();
        groupB.IsDefined.ShouldBeFalse();
    }

    [Test]
    public void should_support_named_groups_on_default_match()
    {
        var match = default(PcreRefMatch);
        var groupA = match["foo"];
        var groupB = match.Groups["foo"];

        groupA.IsDefined.ShouldBeFalse();
        groupB.IsDefined.ShouldBeFalse();
    }

    [Test]
    public void should_support_named_groups_on_default_match_8bit()
    {
        var match = default(PcreRefMatch8Bit);
        var groupA = match["foo"];
        var groupB = match.Groups["foo"];

        groupA.IsDefined.ShouldBeFalse();
        groupB.IsDefined.ShouldBeFalse();
    }

    [Test]
    public void should_support_duplicated_named_groups_on_default_match()
    {
        var match = default(PcreRefMatch);
        var enumerator = match.GetDuplicateNamedGroups("foo").GetEnumerator();

        enumerator.MoveNext().ShouldBeFalse();
        enumerator.MoveNext().ShouldBeFalse();
    }

    [Test]
    public void should_support_duplicated_named_groups_on_default_match_8bit()
    {
        var match = default(PcreRefMatch8Bit);
        var enumerator = match.GetDuplicateNamedGroups("foo").GetEnumerator();

        enumerator.MoveNext().ShouldBeFalse();
        enumerator.MoveNext().ShouldBeFalse();
    }

    [Test]
    public void should_support_group_enumeration_on_default_match()
    {
        var match = default(PcreRefMatch);
        var enumerator = match.GetEnumerator();

        enumerator.MoveNext().ShouldBeTrue();
        enumerator.Current.Success.ShouldBeFalse();

        enumerator.MoveNext().ShouldBeFalse();
        enumerator.MoveNext().ShouldBeFalse();
    }

    [Test]
    public void should_support_group_enumeration_on_default_match_8bit()
    {
        var match = default(PcreRefMatch8Bit);
        var enumerator = match.GetEnumerator();

        enumerator.MoveNext().ShouldBeTrue();
        enumerator.Current.Success.ShouldBeFalse();

        enumerator.MoveNext().ShouldBeFalse();
        enumerator.MoveNext().ShouldBeFalse();
    }
}
