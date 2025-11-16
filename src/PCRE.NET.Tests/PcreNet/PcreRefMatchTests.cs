using System;
using NUnit.Framework;

namespace PCRE.Tests.PcreNet;

[TestFixture]
public class PcreRefMatchTests
{
    [Test]
    public void should_return_matched_string()
    {
        var re = new PcreRegex(".");
        var match = re.Match("ab".AsSpan());

        Assert.That(match.ToString(), Is.EqualTo("a"));
    }

    [Test]
    public void should_return_matched_string_utf8()
    {
        var re = new PcreRegexUtf8("."u8);
        var match = re.Match("ab"u8);

        Assert.That(match.ToString(), Is.EqualTo("a"));
    }

    [Test]
    public void should_return_matched_string_from_group()
    {
        var re = new PcreRegex(".");
        var match = re.Match("ab".AsSpan());

        Assert.That(match[0].ToString(), Is.EqualTo("a"));
    }

    [Test]
    public void should_return_matched_string_from_group_utf8()
    {
        var re = new PcreRegexUtf8("."u8);
        var match = re.Match("ab"u8);

        Assert.That(match[0].ToString(), Is.EqualTo("a"));
    }

    [Test]
    public void should_cast_group_to_string()
    {
        var re = new PcreRegex(".");
        var match = re.Match("ab".AsSpan());

        Assert.That((string)match[0], Is.EqualTo("a"));
    }

    [Test]
    public void should_cast_group_to_string_utf8()
    {
        var re = new PcreRegexUtf8("."u8);
        var match = re.Match("ab"u8);

        Assert.That((string)match[0], Is.EqualTo("a"));
    }

    [Test]
    public void should_enumerate_groups()
    {
        var re = new PcreRegex("(.)(?<name>.)");
        var match = re.Match("ab".AsSpan());

        var enumerator = match.Groups.GetEnumerator();

        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current.Value.ToString(), Is.EqualTo("ab"));

        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current.Value.ToString(), Is.EqualTo("a"));

        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current.Value.ToString(), Is.EqualTo("b"));

        Assert.That(enumerator.MoveNext(), Is.False);
        Assert.That(enumerator.MoveNext(), Is.False);
    }

    [Test]
    public void should_enumerate_groups_utf8()
    {
        var re = new PcreRegexUtf8("(.)(?<name>.)"u8);
        var match = re.Match("ab"u8);

        var enumerator = match.Groups.GetEnumerator();

        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current.Value.SequenceEqual("ab"u8));

        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current.Value.SequenceEqual("a"u8));

        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current.Value.SequenceEqual("b"u8));

        Assert.That(enumerator.MoveNext(), Is.False);
        Assert.That(enumerator.MoveNext(), Is.False);
    }

    [Test]
    public void should_enumerate_groups_directly()
    {
        var re = new PcreRegex("(.)(?<name>.)");
        var match = re.Match("ab".AsSpan());

        var enumerator = match.GetEnumerator();

        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current.Value.ToString(), Is.EqualTo("ab"));

        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current.Value.ToString(), Is.EqualTo("a"));

        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current.Value.ToString(), Is.EqualTo("b"));

        Assert.That(enumerator.MoveNext(), Is.False);
        Assert.That(enumerator.MoveNext(), Is.False);
    }

    [Test]
    public void should_enumerate_groups_directly_utf8()
    {
        var re = new PcreRegexUtf8("(.)(?<name>.)"u8);
        var match = re.Match("ab"u8);

        var enumerator = match.GetEnumerator();

        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current.Value.SequenceEqual("ab"u8));

        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current.Value.SequenceEqual("a"u8));

        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current.Value.SequenceEqual("b"u8));

        Assert.That(enumerator.MoveNext(), Is.False);
        Assert.That(enumerator.MoveNext(), Is.False);
    }

    [Test]
    public void should_get_group_list()
    {
        var re = new PcreRegex("(.)(?<name>.)");
        var values = re.Match("ab".AsSpan()).Groups.ToList(i => i.Value.ToString());

        Assert.That(values, Is.EqualTo(["ab", "a", "b"]));
    }

    [Test]
    public void should_get_group_list_utf8()
    {
        var re = new PcreRegexUtf8("(.)(?<name>.)"u8);
        var values = re.Match("ab"u8).Groups.ToList(i => i.ToString());

        Assert.That(values, Is.EqualTo(["ab", "a", "b"]));
    }

    [Test]
    public void should_have_undefined_value_in_default_ref_match()
    {
        var match = default(PcreRefMatch);

        Assert.That(match.Success, Is.False);
        Assert.That(match.CaptureCount, Is.EqualTo(0));
        Assert.That(match.Value.ToString(), Is.SameAs(string.Empty));
        Assert.That(match.Index, Is.EqualTo(-1));
        Assert.That(match.EndIndex, Is.EqualTo(-1));
        Assert.That(match.Length, Is.EqualTo(0));
        Assert.That(match.IsPartialMatch, Is.False);
        Assert.That(match.Mark.Length, Is.EqualTo(0));

        Assert.That(match[0].Success, Is.False);
        Assert.That(match[0].IsDefined, Is.False);
        Assert.That(match[0].Value.ToString(), Is.SameAs(string.Empty));
        Assert.That(match[0].Index, Is.EqualTo(-1));
        Assert.That(match[0].EndIndex, Is.EqualTo(-1));
        Assert.That(match[0].Length, Is.EqualTo(0));
    }

    [Test]
    public void should_have_undefined_value_in_default_utf8_match()
    {
        var match = default(PcreRefMatchUtf8);

        Assert.That(match.Success, Is.False);
        Assert.That(match.CaptureCount, Is.EqualTo(0));
        Assert.That(match.Value.IsEmpty);
        Assert.That(match.Index, Is.EqualTo(-1));
        Assert.That(match.EndIndex, Is.EqualTo(-1));
        Assert.That(match.Length, Is.EqualTo(0));
        Assert.That(match.IsPartialMatch, Is.False);
        Assert.That(match.Mark.Length, Is.EqualTo(0));

        Assert.That(match[0].Success, Is.False);
        Assert.That(match[0].IsDefined, Is.False);
        Assert.That(match[0].Value.IsEmpty);
        Assert.That(match[0].Index, Is.EqualTo(-1));
        Assert.That(match[0].EndIndex, Is.EqualTo(-1));
        Assert.That(match[0].Length, Is.EqualTo(0));
    }

    [Test]
    public void should_copy_ref_match()
    {
        var re = new PcreRegex(".");
        var enumerator = re.Matches("ab".AsSpan()).GetEnumerator();

        Assert.That(enumerator.MoveNext(), Is.True);

        var copy = enumerator.Current;

        Assert.That(enumerator.Current.Value.ToString(), Is.EqualTo("a"));
        Assert.That(copy.Value.ToString(), Is.EqualTo("a"));

        Assert.That(enumerator.MoveNext(), Is.True);

        Assert.That(enumerator.Current.Value.ToString(), Is.EqualTo("b"));
        Assert.That(copy.Value.ToString(), Is.EqualTo("a"));
    }

    [Test]
    public void should_copy_utf8_match()
    {
        var re = new PcreRegexUtf8("."u8);
        var enumerator = re.Matches("ab"u8).GetEnumerator();

        Assert.That(enumerator.MoveNext(), Is.True);

        var copy = enumerator.Current;

        Assert.That(enumerator.Current.Value.SequenceEqual("a"u8));
        Assert.That(copy.Value.SequenceEqual("a"u8));

        Assert.That(enumerator.MoveNext(), Is.True);

        Assert.That(enumerator.Current.Value.SequenceEqual("b"u8));
        Assert.That(copy.Value.SequenceEqual("a"u8));
    }

    [Test]
    public void should_support_move_next_after_all_matches_are_exhausted()
    {
        var re = new PcreRegex(".");
        var enumerator = re.Matches("ab".AsSpan()).GetEnumerator();

        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.MoveNext(), Is.False);
        Assert.That(enumerator.MoveNext(), Is.False);
    }

    [Test]
    public void should_support_move_next_after_all_matches_are_exhausted_utf8()
    {
        var re = new PcreRegexUtf8("."u8);
        var enumerator = re.Matches("ab"u8).GetEnumerator();

        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.MoveNext(), Is.False);
        Assert.That(enumerator.MoveNext(), Is.False);
    }

    [Test]
    public void should_support_indexed_groups_on_default_match()
    {
        var match = default(PcreRefMatch);
        var groupA = match[0];
        var groupB = match.Groups[0];

        Assert.That(groupA.IsDefined, Is.False);
        Assert.That(groupB.IsDefined, Is.False);
    }

    [Test]
    public void should_support_indexed_groups_on_default_match_utf8()
    {
        var match = default(PcreRefMatchUtf8);
        var groupA = match[0];
        var groupB = match.Groups[0];

        Assert.That(groupA.IsDefined, Is.False);
        Assert.That(groupB.IsDefined, Is.False);
    }

    [Test]
    public void should_support_named_groups_on_default_match()
    {
        var match = default(PcreRefMatch);
        var groupA = match["foo"];
        var groupB = match.Groups["foo"];

        Assert.That(groupA.IsDefined, Is.False);
        Assert.That(groupB.IsDefined, Is.False);
    }

    [Test]
    public void should_support_named_groups_on_default_match_utf8()
    {
        var match = default(PcreRefMatchUtf8);
        var groupA = match["foo"];
        var groupB = match.Groups["foo"];

        Assert.That(groupA.IsDefined, Is.False);
        Assert.That(groupB.IsDefined, Is.False);
    }

    [Test]
    public void should_support_duplicated_named_groups_on_default_match()
    {
        var match = default(PcreRefMatch);
        var enumerator = match.GetDuplicateNamedGroups("foo").GetEnumerator();

        Assert.That(enumerator.MoveNext(), Is.False);
        Assert.That(enumerator.MoveNext(), Is.False);
    }

    [Test]
    public void should_support_duplicated_named_groups_on_default_match_utf8()
    {
        var match = default(PcreRefMatchUtf8);
        var enumerator = match.GetDuplicateNamedGroups("foo").GetEnumerator();

        Assert.That(enumerator.MoveNext(), Is.False);
        Assert.That(enumerator.MoveNext(), Is.False);
    }

    [Test]
    public void should_support_group_enumeration_on_default_match()
    {
        var match = default(PcreRefMatch);
        var enumerator = match.GetEnumerator();

        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current.Success, Is.False);

        Assert.That(enumerator.MoveNext(), Is.False);
        Assert.That(enumerator.MoveNext(), Is.False);
    }

    [Test]
    public void should_support_group_enumeration_on_default_match_utf8()
    {
        var match = default(PcreRefMatchUtf8);
        var enumerator = match.GetEnumerator();

        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current.Success, Is.False);

        Assert.That(enumerator.MoveNext(), Is.False);
        Assert.That(enumerator.MoveNext(), Is.False);
    }
}
