using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace PCRE.Tests.Pcre
{
    [TestFixture]
    public class PcreTests
    {
        [Test]
        [TestCaseSource(typeof(PcreTestsSource))]
        public void should_pass_pcre_test_suite(TestCase testCase, TestOutput expectedResult)
        {
            Console.WriteLine("TEST CASE: Input line {0}, output line {1}", testCase.Pattern.LineNumber, expectedResult.Pattern.LineNumber);
            Console.WriteLine(testCase.Pattern.FullString);

            if (testCase.Skip)
                Assert.Inconclusive();

            Assert.That(expectedResult.Pattern, Is.EqualTo(testCase.Pattern));

            if (!testCase.Pattern.AllMatches)
                Assert.That(expectedResult.ExpectedResults.Count, Is.EqualTo(testCase.SubjectLines.Count));
        }

        private class PcreTestsSource : IEnumerable<ITestCaseData>
        {
            private static readonly string[,] InputFiles =
            {
                { "testinput1", "testoutput1" }
            };

            private IEnumerable<ITestCaseData> GetTestCases()
            {
                const string testCasesDir = @"Pcre\TestCases";

                for (var fileIndex = 0; fileIndex < InputFiles.GetLength(0); ++fileIndex)
                {
                    var testFileName = InputFiles[fileIndex, 0];

                    using (var inputFs = File.OpenRead(Path.Combine(testCasesDir, testFileName)))
                    using (var outputFs = File.OpenRead(Path.Combine(testCasesDir, InputFiles[fileIndex, 1])))
                    using (var inputReader = new TestInputReader(inputFs))
                    using (var outputReader = new TestOutputReader(outputFs))
                    {
                        var tests = inputReader.ReadTestCases().Zip(outputReader.ReadTestOutputs(), (i, o) => new
                        {
                            testCase = i,
                            expectedResult = o
                        });

                        foreach (var test in tests)
                        {
                            var testCase = new TestCaseData(test.testCase, test.expectedResult)
                                .SetCategory(testFileName)
                                .SetName(String.Format("{0} line {1:0000}", testFileName, test.testCase.Pattern.LineNumber))
                                .SetDescription(test.testCase.Pattern.Pattern);

                            yield return testCase;
                        }
                    }
                }
            }

            public IEnumerator<ITestCaseData> GetEnumerator()
            {
                return GetTestCases().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
