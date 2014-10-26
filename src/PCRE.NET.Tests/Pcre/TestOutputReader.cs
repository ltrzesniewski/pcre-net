using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace PCRE.Tests.Pcre
{
    public class TestOutputReader : TestFileReader
    {
        public TestOutputReader(Stream inputStream)
            : base(inputStream)
        {
        }

        public IEnumerable<TestOutput> ReadTestOutputs()
        {
            while (true)
            {
                var pattern = ReadNextPattern();

                if (pattern == null)
                    yield break;

                var testOutput = new TestOutput
                {
                    Pattern = pattern
                };

                ExpectedResult currentResult = null;
                ExpectedMatch currentMatch = null;

                while (true)
                {
                    var line = ReadLine();

                    if (string.IsNullOrWhiteSpace(line))
                        break;

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

                        // Remaining string
                        if (pattern.GetRemainingString)
                        {
                            match = Regex.Match(line, @"^\s*(\d+)\+ (.*)$");
                            if (match.Success && currentMatch != null)
                            {
                                // TODO
                                currentMatch.RemainingString = match.Groups[2].Value;
                                continue;
                            }
                        }

                        if (pattern.ExtractMarks)
                        {
                            match = Regex.Match(line, @"^\s*MK: (.*)$");
                            if (match.Success && currentMatch != null)
                            {
                                // TODO
                                currentMatch.MarkName = match.Groups[2].Value;
                                continue;
                            }
                        }
                    }

                    currentResult = new ExpectedResult
                    {
                        SubjectLine = line.Trim()
                    };

                    currentMatch = null;

                    testOutput.ExpectedResults.Add(currentResult);
                    continue;
                }

                yield return testOutput;
            }
        }
    }
}
