using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace PCRE.Tests.Pcre
{
    [TestFixture]
    public class PcreTests
    {
        [Test]
        [TestCase("testinput1", "testoutput1")]
        public void should_pass_pcre_tests(string inputFile, string outputFile)
        {
            const string testCasesDir = @"Pcre\TestCases";

            using (var inputFs = File.OpenRead(Path.Combine(testCasesDir, inputFile)))
            using (var outputFs = File.OpenRead(Path.Combine(testCasesDir, outputFile)))
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
                    Test(test.testCase, test.expectedResult);
                }
            }
        }

        private static void Test(TestCase testCase, TestOutput expectedResult)
        {
            Console.WriteLine("TEST CASE: Input line {0}, output line {1}", testCase.Pattern.LineNumber, expectedResult.Pattern.LineNumber);
            Console.WriteLine(testCase.Pattern.FullString);

            if (testCase.Skip)
            {
                Console.WriteLine("SKIPPING");
                return;
            }

            Assert.That(expectedResult.Pattern, Is.EqualTo(testCase.Pattern));

            if (!testCase.Pattern.AllMatches)
                Assert.That(expectedResult.ExpectedResults.Count, Is.EqualTo(testCase.SubjectLines.Count));
        }
    }
}
