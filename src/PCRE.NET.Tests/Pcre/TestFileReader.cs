using System;
using System.IO;
using System.Text;

namespace PCRE.Tests.Pcre
{
    public abstract class TestFileReader : IDisposable
    {
        private readonly TextReader _reader;
        private int _lineNumber;
        private string _lastReadLine;

        protected TestFileReader(Stream inputStream)
        {
            _reader = new StreamReader(inputStream, Encoding.GetEncoding("ISO-8859-1"));
        }

        public void Dispose()
        {
            _reader.Dispose();
        }

        protected string ReadLine()
        {
            ++_lineNumber;
            _lastReadLine = _reader.ReadLine();
            return _lastReadLine;
        }

        protected TestPattern ReadNextPattern()
        {
            while (true)
            {
                var line = ReadLine();
                var lineNumber = _lineNumber;

                if (line == null)
                    return null;

                if (String.IsNullOrWhiteSpace(line))
                    continue;

                if (line.StartsWith("<"))
                    continue; // TODO : Interpret that

                line = line.TrimStart();

                if (line[0] <= ' ')
                    throw InvalidInputException("Unexpected input");

                var delimiter = line[0];

                var fullString = new StringBuilder();
                var pattern = new StringBuilder();

                var firstLine = true;

                while (true)
                {
                    fullString.AppendLine(line);

                    for (var i = 0; i < line.Length; ++i)
                    {
                        if (line[i] == delimiter)
                        {
                            if (firstLine)
                            {
                                firstLine = false;
                            }
                            else
                            {
                                var result = new TestPattern
                                {
                                    FullString = fullString.ToString(),
                                    Pattern = pattern.ToString(),
                                    OptionsString = line.Substring(i + 1),
                                    LineNumber = lineNumber
                                };

                                ParseOptions(result);

                                return result;
                            }
                        }

                        else if (line[i] == '\\')
                        {
                            pattern.Append('\\');
                            if (++i < line.Length)
                                pattern.Append(line[i]);
                        }
                        else
                        {
                            pattern.Append(line[i]);
                        }
                    }

                    pattern.Append('\n');

                    line = ReadLine();

                    if (line == null)
                        throw InvalidInputException("Unexpected end of pattern");
                }
            }
        }

        private void ParseOptions(TestPattern pattern)
        {
            var options = PcreOptions.None;

            foreach (var c in pattern.OptionsString)
            {
                switch (c)
                {
                    case 'i':
                        options |= PcreOptions.IgnoreCase;
                        break;

                    case 'm':
                        options |= PcreOptions.MultiLine;
                        break;

                    case 's':
                        options |= PcreOptions.Singleline;
                        break;

                    case 'x':
                        options |= PcreOptions.IgnorePatternWhitespace;
                        break;

                    case 'J':
                        options |= PcreOptions.DuplicateNames;
                        break;

                    case 'g':
                        pattern.AllMatches = true;
                        break;

                    case '+':
                        pattern.GetRemainingString = true;
                        break;

                    case 'K':
                        pattern.ExtractMarks = true;
                        break;

                    case 'S':
                        options |= PcreOptions.Studied;
                        break;

                    case 'Y':
                        options |= PcreOptions.NoStartOptimize;
                        break;

                    case ' ':
                        break;

                    default:
                        throw InvalidInputException("Unknown modifier: " + c);
                }
            }

            pattern.PatternOptions = options;
        }

        

        protected InvalidOperationException InvalidInputException(string message, Exception innerException = null)
        {
            return new InvalidOperationException(String.Format("{0} at line {1}: {2}", message, _lineNumber, _lastReadLine), innerException);
        }
    }
}
