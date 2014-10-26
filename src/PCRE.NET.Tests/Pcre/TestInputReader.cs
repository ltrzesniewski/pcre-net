using System;
using System.Collections.Generic;
using System.IO;

namespace PCRE.Tests.Pcre
{
    public class TestInputReader : TestFileReader
    {
        public TestInputReader(Stream inputStream)
            : base(inputStream)
        {
        }

        public IEnumerable<TestCase> ReadTestCases()
        {
            while (true)
            {
                var pattern = ReadNextPattern();

                if (pattern == null)
                    yield break;

                var testCase = new TestCase
                {
                    Pattern = pattern,
                };

                try
                {
                    // TODO : DUPNAMES is broken
                    if ((pattern.PatternOptions & PcreOptions.AllowDuplicateNames) != 0)
                        testCase.Skip = true;

                    testCase.Regex = new PcreRegex(pattern.Pattern, pattern.PatternOptions);
                }
                catch (ArgumentException ex)
                {
                    if (ex.Message.Contains(@"\C not allowed in lookbehind assertion")) // Not supported
                        testCase.Skip = true;

                    if (!testCase.Skip)
                        throw InvalidInputException("Invalid pattern", ex);
                }

                while (true)
                {
                    var line = ReadLine();

                    if (string.IsNullOrWhiteSpace(line))
                        break;

                    line = line.Trim();
                    testCase.SubjectLines.Add(line);
                }

                yield return testCase;
            }
        }
    }
}
