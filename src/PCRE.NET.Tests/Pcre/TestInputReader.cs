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
                if (pattern is null)
                    yield break;

                var testCase = new TestCase(pattern);

                while (true)
                {
                    var line = ReadLine();

                    if (string.IsNullOrWhiteSpace(line))
                        break;

                    if (line.StartsWith("\\="))
                        continue;

                    line = line.Trim();
                    testCase.SubjectLines.Add(line);
                }

                yield return testCase;
            }
        }
    }
}
