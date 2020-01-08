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

        public IEnumerable<TestInput> ReadTestInputs()
        {
            while (true)
            {
                var pattern = ReadNextPattern();

                if (pattern == null)
                    yield break;

                var testCase = new TestInput
                {
                    Pattern = pattern,
                };

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
