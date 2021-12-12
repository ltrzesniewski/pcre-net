using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace PCRE.Tests.Pcre
{
    [TestFixture]
    public class PcreTests
    {
        [Test]
        [TestCaseSource(typeof(PcreTestsSource))]
        [Parallelizable(ParallelScope.All)]
        public void should_pass_pcre_test_suite(TestCase testCase)
        {
            Assert.That(testCase.ExpectedResult.Pattern, Is.EqualTo(testCase.Input.Pattern));

            var options = testCase.Jit
                ? PcreOptions.Compiled | PcreOptions.CompiledPartial
                : PcreOptions.None;

            try
            {
                RunTest(testCase.Input, testCase.ExpectedResult, options, testCase.ApiKind);
            }
            catch
            {
                Console.WriteLine($"PATTERN: {testCase.TestFile}:line {testCase.Input.Pattern.LineNumber}");
                throw;
            }
        }

        private static void RunTest(TestInput testInput, TestOutput expectedResult, PcreOptions options, ApiKind apiKind)
        {
            var pattern = testInput.Pattern;

            if (pattern.NotSupported)
                Assert.Inconclusive("Feature not supported");

            options = (options | pattern.PatternOptions) & ~pattern.ResetOptionBits;

            PcreRegex regex;
            try
            {
                regex = new PcreRegex(pattern.Pattern, options);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains(@"\C is not allowed in a lookbehind assertion"))
                    Assert.Inconclusive(ex.Message);

                throw;
            }

            var jitStack = pattern.JitStack != 0 && (options & PcreOptions.Compiled) != 0
                ? new PcreJitStack(1, pattern.JitStack)
                : null;

            using (jitStack)
            {
                for (var line = 0; line < testInput.SubjectLines.Count; ++line)
                {
                    var subject = testInput.SubjectLines[line];
                    var expected = expectedResult.ExpectedResults[line];

                    Assert.That(expected.SubjectLine, Is.EqualTo(subject));

                    if (!pattern.SubjectLiteral)
                    {
                        subject = pattern.HexEncoding
                            ? subject.UnescapeBinarySubject()
                            : subject.UnescapeSubject();
                    }

                    var matchSettings = new PcreMatchSettings
                    {
                        JitStack = jitStack
                    };

                    switch (apiKind)
                    {
                        case ApiKind.String:
                        {
                            var matches = regex.Matches(subject, 0, PcreMatchOptions.None, null, matchSettings)
                                               .Take(pattern.AllMatches ? int.MaxValue : 1)
                                               .ToList();

                            Assert.That(matches.Count, Is.EqualTo(expected.Matches.Count));

                            for (var matchIndex = 0; matchIndex < matches.Count; ++matchIndex)
                            {
                                var actualMatch = matches[matchIndex];
                                var expectedMatch = expected.Matches[matchIndex];

                                CompareGroups(pattern, actualMatch, expectedMatch);

                                if (pattern.ExtractMarks)
                                    CompareMark(actualMatch, expectedMatch);

                                if (pattern.GetRemainingString)
                                    CompareRemainingString(actualMatch, expectedMatch);
                            }

                            break;
                        }

                        case ApiKind.Span:
                        {
                            var matchCount = 0;

                            foreach (var actualMatch in regex.Matches(subject.AsSpan(), 0, PcreMatchOptions.None, null, matchSettings))
                            {
                                Assert.That(matchCount, Is.LessThan(expected.Matches.Count));

                                var expectedMatch = expected.Matches[matchCount];
                                ++matchCount;

                                CompareGroups(pattern, actualMatch, expectedMatch);

                                if (pattern.ExtractMarks)
                                    CompareMark(actualMatch, expectedMatch);

                                if (pattern.GetRemainingString)
                                    CompareRemainingString(actualMatch, expectedMatch);

                                if (!pattern.AllMatches)
                                    break;
                            }

                            Assert.That(matchCount, Is.EqualTo(expected.Matches.Count));
                            break;
                        }

                        case ApiKind.MatchBuffer:
                        {
                            var matchCount = 0;
                            using var buffer = regex.CreateMatchBuffer(matchSettings);

                            foreach (var actualMatch in buffer.Matches(subject.AsSpan()))
                            {
                                Assert.That(matchCount, Is.LessThan(expected.Matches.Count));

                                var expectedMatch = expected.Matches[matchCount];
                                ++matchCount;

                                CompareGroups(pattern, actualMatch, expectedMatch);

                                if (pattern.ExtractMarks)
                                    CompareMark(actualMatch, expectedMatch);

                                if (pattern.GetRemainingString)
                                    CompareRemainingString(actualMatch, expectedMatch);

                                if (!pattern.AllMatches)
                                    break;
                            }

                            Assert.That(matchCount, Is.EqualTo(expected.Matches.Count));
                            break;
                        }
                    }
                }
            }
        }

        private static void CompareGroups(TestPattern pattern, PcreMatch actualMatch, ExpectedMatch expectedMatch)
        {
            var actualGroups = actualMatch.ToList();
            var expectedGroups = expectedMatch.Groups.ToList();

            Assert.That(actualGroups.Count, Is.GreaterThanOrEqualTo(expectedGroups.Count));

            for (var groupIndex = 0; groupIndex < actualGroups.Count; ++groupIndex)
            {
                var actualGroup = actualGroups[groupIndex];
                var expectedGroup = groupIndex < expectedGroups.Count
                    ? expectedGroups[groupIndex]
                    : ExpectedGroup.Unset;

                Assert.That(actualGroup.Success, Is.EqualTo(expectedGroup.IsMatch));

                if (expectedGroup.IsMatch)
                {
                    var expectedValue = pattern.SubjectLiteral
                        ? expectedGroup.Value
                        : expectedGroup.Value.UnescapeGroup();

                    Assert.That(actualGroup.Value, Is.EqualTo(expectedValue));
                    Assert.That(actualGroup.ValueSpan.ToString(), Is.EqualTo(expectedValue));
                }
            }
        }

        private static void CompareGroups(TestPattern pattern, PcreRefMatch actualMatch, ExpectedMatch expectedMatch)
        {
            var expectedGroups = expectedMatch.Groups.ToList();

            Assert.That(actualMatch.Groups.Count, Is.GreaterThanOrEqualTo(expectedGroups.Count));

            for (var groupIndex = 0; groupIndex < actualMatch.Groups.Count; ++groupIndex)
            {
                var actualGroup = actualMatch.Groups[groupIndex];
                var expectedGroup = groupIndex < expectedGroups.Count
                    ? expectedGroups[groupIndex]
                    : ExpectedGroup.Unset;

                Assert.That(actualGroup.Success, Is.EqualTo(expectedGroup.IsMatch));

                if (expectedGroup.IsMatch)
                {
                    var expectedValue = pattern.SubjectLiteral
                        ? expectedGroup.Value
                        : expectedGroup.Value.UnescapeGroup();

                    Assert.That(actualGroup.Value.ToString(), Is.EqualTo(expectedValue));
                }
            }
        }

        private static void CompareMark(PcreMatch actualMatch, ExpectedMatch expectedMatch)
            => Assert.That(actualMatch.Mark, Is.EqualTo(expectedMatch.Mark?.UnescapeGroup()));

        private static void CompareMark(PcreRefMatch actualMatch, ExpectedMatch expectedMatch)
            => Assert.That(actualMatch.Mark.ToString(), Is.EqualTo(expectedMatch.Mark?.UnescapeGroup() ?? string.Empty));

        private static void CompareRemainingString(PcreMatch actualMatch, ExpectedMatch expectedMatch)
            => Assert.That(actualMatch.Subject.Substring(actualMatch.Index + actualMatch.Length), Is.EqualTo(expectedMatch.RemainingString?.UnescapeGroup()));

        private static void CompareRemainingString(PcreRefMatch actualMatch, ExpectedMatch expectedMatch)
            => Assert.That(actualMatch.Subject.Slice(actualMatch.Index + actualMatch.Length).ToString(), Is.EqualTo(expectedMatch.RemainingString?.UnescapeGroup()));

        private class PcreTestsSource : IEnumerable<ITestCaseData>
        {
            private static string[,] InputFiles { get; } =
            {
                { "testinput1", "testoutput1" }
                //{ "testinput2", "testoutput2" }
            };

            private static IEnumerable<ITestCaseData> GetTestCases()
            {
                var testCasesDir = Path.Combine(Path.GetDirectoryName(typeof(PcreTests).Assembly.Location) ?? throw new InvalidOperationException(), @"Pcre", "TestCases");

                for (var fileIndex = 0; fileIndex < InputFiles.GetLength(0); ++fileIndex)
                {
                    var testFileName = InputFiles[fileIndex, 0];
                    var testFilePath = Path.Combine(testCasesDir, testFileName);

                    using (var inputFs = File.OpenRead(testFilePath))
                    using (var outputFs = File.OpenRead(Path.Combine(testCasesDir, InputFiles[fileIndex, 1])))
                    using (var inputReader = new TestInputReader(inputFs))
                    using (var outputReader = new TestOutputReader(outputFs))
                    {
                        var tests = inputReader.ReadTestInputs().Zip(outputReader.ReadTestOutputs(), (i, o) => new
                        {
                            input = i,
                            expectedResult = o
                        });

                        var testCases =
                            from test in tests
                            from jit in new[] { false, true }
                            from apiKind in new[] { ApiKind.String, ApiKind.Span, ApiKind.MatchBuffer }
                            let testCase = new TestCase(testFilePath, test.input, test.expectedResult, jit, apiKind)
                            select new TestCaseData(testCase)
                                   .SetCategory(testFileName)
                                   .SetName($"PCRE {testFileName}, Line {testCase.Input.Pattern.LineNumber:0000}, {(jit ? "JIT" : "Interpreted")}, {apiKind}")
                                   .SetDescription(testCase.Input.Pattern.Pattern);

                        foreach (var testCase in testCases)
                            yield return testCase;
                    }
                }
            }

            public IEnumerator<ITestCaseData> GetEnumerator() => GetTestCases().GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
