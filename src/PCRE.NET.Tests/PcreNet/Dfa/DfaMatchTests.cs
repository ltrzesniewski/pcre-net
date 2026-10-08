using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NUnit.Framework;
using PCRE.Dfa;
using Shouldly;

namespace PCRE.Tests.PcreNet.Dfa;

[TestFixture]
public class DfaMatchTests
{
    [Test]
    public void should_match_with_dfa()
    {
        var re = new PcreRegex(@"<.*>");
        var result = re.Dfa.Match("This is <something> <something else> <something further> no more");

        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();

        result.ShouldHaveCount(3);
        result.Index.ShouldBe(8);

        result.LongestMatch.ShouldNotBeNull();
        result.LongestMatch.Value.ShouldBe("<something> <something else> <something further>");
        result.LongestMatch.ValueSpan.ShouldBe("<something> <something else> <something further>");

        result.LongestMatch.ShouldBeSameAs(result[0]);
        result.ShortestMatch.ShouldBeSameAs(result[2]);

        result.ShortestMatch.ShouldNotBeNull();
        result.ShortestMatch.Value.ShouldBe("<something>");
        result.ShortestMatch.ValueSpan.ShouldBe("<something>");

        result[1].ShouldNotBeNull();
        result[1].Value.ShouldBe("<something> <something else>");
        result[1].ValueSpan.ShouldBe("<something> <something else>");

        result[3].ShouldNotBeNull();
        result[3].Value.ShouldBeSameAs(string.Empty);
        result[3].ValueSpan.IsEmpty.ShouldBeTrue();
        result[3].Index.ShouldBe(-1);
        result[3].Length.ShouldBe(0);
    }

    [Test]
    public void should_get_shortest_match()
    {
        var re = new PcreRegex(@"<.*>");
        var result = re.Dfa.Match("This is <something> <something else> <something further> no more", PcreDfaMatchOptions.DfaShortest);

        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();

        result.ShouldHaveSingleItem();
        result.Index.ShouldBe(8);

        result.ShortestMatch.ShouldNotBeNull();
        result.ShortestMatch.Value.ShouldBe("<something>");
        result.ShortestMatch.ValueSpan.ShouldBe("<something>");
    }

    [Test]
    public void should_get_max_matches()
    {
        var re = new PcreRegex(@"<.*>");
        var result = re.Dfa.Match("This is <something> <something else> <something further> no more", new PcreDfaMatchSettings
        {
            MaxResults = 2
        });

        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();

        result.ShouldHaveCount(2);
        result.Index.ShouldBe(8);

        result.LongestMatch.ShouldNotBeNull();
        result.LongestMatch.Value.ShouldBe("<something> <something else> <something further>");
        result.LongestMatch.ValueSpan.ShouldBe("<something> <something else> <something further>");

        result.ShortestMatch.ShouldNotBeNull();
        result.ShortestMatch.Value.ShouldBe("<something> <something else>");
        result.ShortestMatch.ValueSpan.ShouldBe("<something> <something else>");
    }

    [Test]
    public void should_start_at_given_index()
    {
        var re = new PcreRegex(@"<.*>");
        var result = re.Dfa.Match("This is <something> <something else> <something further> no more", 10);

        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();

        result.ShouldHaveCount(2);
        result.Index.ShouldBe(20);

        result.LongestMatch.ShouldNotBeNull();
        result.LongestMatch.Value.ShouldBe("<something else> <something further>");
        result.LongestMatch.ValueSpan.ShouldBe("<something else> <something further>");

        result.ShortestMatch.ShouldNotBeNull();
        result.ShortestMatch.Value.ShouldBe("<something else>");
        result.ShortestMatch.ValueSpan.ShouldBe("<something else>");
    }

    [Test]
    public void should_execute_callouts()
    {
        var re = new PcreRegex(@"<.*(?C1)>");
        var settings = new PcreDfaMatchSettings();
        settings.OnCallout += callout => callout.Match.Subject[callout.CurrentOffset - 1] == 'e' ? PcreCalloutResult.Fail : PcreCalloutResult.Pass;

        var result = re.Dfa.Match("This is <something> <something else> <something further> no more", settings);

        result.ShouldHaveCount(2);
        result.LongestMatch.Value.ShouldBe("<something> <something else> <something further>");
        result.ShortestMatch.Value.ShouldBe("<something>");
    }

    [Test]
    public void should_return_value_span_from_subject_string()
    {
        var subject = string.Concat("foo", "bar");
        var re = new PcreRegex(@"b(a)(r)");

        var result = re.Dfa.Match(subject);

        ref var subjectRef = ref MemoryMarshal.GetReference(subject.AsSpan(3));
        ref var valueRef = ref MemoryMarshal.GetReference(result.LongestMatch.ValueSpan);
        Unsafe.AreSame(ref valueRef, ref subjectRef).ShouldBeTrue();

        _ = result.LongestMatch.Value; // Reading the string value shouldn't change the span target

        valueRef = ref MemoryMarshal.GetReference(result.LongestMatch.ValueSpan);
        Unsafe.AreSame(ref valueRef, ref subjectRef).ShouldBeTrue();
    }

    [Test]
    [TestCase(PcreDfaMatchOptions.PartialSoft)]
    [TestCase(PcreDfaMatchOptions.PartialHard)]
    public void should_match_partially(PcreDfaMatchOptions options)
    {
        var re = new PcreRegex(@"123");

        var match = re.Dfa.Match("abc12", options);

        match.Success.ShouldBeFalse();
        match.Index.ShouldBe(-1);
        match.ShouldBeEmpty();
        match.LongestMatch.Success.ShouldBeFalse();
        match.ShortestMatch.Success.ShouldBeFalse();

        match.PartialMatch.Success.ShouldBeTrue();
        match.PartialMatch.Index.ShouldBe(3);
        match.PartialMatch.EndIndex.ShouldBe(5);
        match.PartialMatch.Length.ShouldBe(2);
        match.PartialMatch.Value.ShouldBe("12");
    }

    [Test]
    public void should_differentiate_soft_and_hard_partial_matching()
    {
        var re = new PcreRegex(@"dog(sbody)?");

        var softMatch = re.Dfa.Match("dog", PcreDfaMatchOptions.PartialSoft);
        var hardMatch = re.Dfa.Match("dog", PcreDfaMatchOptions.PartialHard);

        softMatch.Success.ShouldBeTrue();
        softMatch.PartialMatch.Success.ShouldBeFalse();
        softMatch.PartialMatch.Index.ShouldBe(-1);

        hardMatch.Success.ShouldBeFalse();
        hardMatch.PartialMatch.Success.ShouldBeTrue();
        hardMatch.PartialMatch.Index.ShouldBe(0);
    }

    [Test]
    public void should_handle_unsuccessful_partial_matches()
    {
        var re = new PcreRegex(@"dog");

        var match = re.Dfa.Match("cat", PcreDfaMatchOptions.PartialSoft);

        match.Success.ShouldBeFalse();
        match.PartialMatch.Success.ShouldBeFalse();
        match.PartialMatch.Index.ShouldBe(-1);
        match.PartialMatch.EndIndex.ShouldBe(-1);
        match.PartialMatch.Length.ShouldBe(0);
    }

    [Test]
    public void should_cache_partial_match()
    {
        var re = new PcreRegex(@"123");

        var match = re.Dfa.Match("abc12", PcreDfaMatchOptions.PartialSoft);

        match.PartialMatch.ShouldBeSameAs(match.PartialMatch);
    }
}
