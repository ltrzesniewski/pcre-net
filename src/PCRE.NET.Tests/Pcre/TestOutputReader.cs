using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace PCRE.Tests.Pcre
{
    public class TestOutputReader : TestFileReader
    {
        private const string _separator = "------------------------------------------------------------------";

        public TestOutputReader(Stream inputStream)
            : base(inputStream)
        {
        }

        public IEnumerable<TestOutput> ReadTestOutputs()
        {
            while (true)
            {
                var pattern = ReadNextPattern();
                if (pattern is null)
                    yield break;

                var testOutput = new TestOutput(pattern);

                ExpectedResult? currentResult = null;
                ExpectedMatch? currentMatch = null;

                while (true)
                {
                    var line = ReadLine();

                    if (line == _separator)
                    {
                        do
                        {
                            line = ReadLine();
                        } while (line != _separator && line != null);
                        line = ReadLine();
                    }

                    if (string.IsNullOrWhiteSpace(line))
                        break;

                    if (line.StartsWith("\\="))
                        continue;

                    if (pattern.ReplaceWith != null) // Not supported yet
                    {
                        while (!string.IsNullOrWhiteSpace(line))
                            line = ReadLine();
                        break;
                    }

                    if (currentResult != null)
                    {
                        if (line.StartsWith("No match"))
                        {
                            currentResult = null;
                            currentMatch = null;
                            continue;
                        }

                        // Capture
                        var match = Regex.Match(line, @"^\s*(\d+): (.*)$");
                        if (match.Success)
                        {
                            var groupIndex = int.Parse(match.Groups[1].Value);

                            if (groupIndex == 0)
                            {
                                currentMatch = new ExpectedMatch();
                                currentResult.Matches.Add(currentMatch);
                            }

                            if (currentMatch == null)
                                throw InvalidInputException("Group outside of match");

                            if (groupIndex != currentMatch.Groups.Count)
                                throw InvalidInputException("Invalid group index");

                            currentMatch.Groups.Add(match.Groups[2].Value == "<unset>"
                                ? ExpectedGroup.Unset
                                : new ExpectedGroup(match.Groups[2].Value));

                            continue;
                        }

                        if (pattern.GetRemainingString)
                        {
                            match = Regex.Match(line, @"^\s*(\d+)\+ (.*)$");
                            if (match.Success && currentMatch != null)
                            {
                                currentMatch.RemainingString = match.Groups[2].Value;
                                continue;
                            }
                        }

                        if (pattern.ExtractMarks)
                        {
                            match = Regex.Match(line, @"^\s*MK: (.*)$");
                            if (match.Success && currentMatch != null)
                            {
                                currentMatch.Mark = match.Groups[1].Value;
                                continue;
                            }
                        }
                    }

                    currentResult = new ExpectedResult(line.Trim());
                    currentMatch = null;

                    testOutput.ExpectedResults.Add(currentResult);
                }

                yield return testOutput;
            }
        }
    }
}
