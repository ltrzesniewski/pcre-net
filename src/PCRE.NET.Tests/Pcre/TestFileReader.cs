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

                if (line.StartsWith("#"))
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
            pattern.PatternOptions = PcreOptions.None;
            pattern.ResetOptionBits = PcreOptions.None;

            foreach (var part in pattern.OptionsString.Split(','))
            {
                switch (part)
                {
                    case "aftertext":
                        pattern.GetRemainingString = true;
                        break;

                    case "dupnames":
                        pattern.PatternOptions |= PcreOptions.DuplicateNames;
                        break;

                    case "mark":
                        pattern.ExtractMarks = true;
                        break;

                    case "no_start_optimize":
                        pattern.PatternOptions |= PcreOptions.NoStartOptimize;
                        break;

                    default:
                        foreach (var c in part)
                        {
                            switch (c)
                            {
                                case 'i':
                                    pattern.PatternOptions |= PcreOptions.IgnoreCase;
                                    break;

                                case 'm':
                                    pattern.PatternOptions |= PcreOptions.MultiLine;
                                    break;

                                case 's':
                                    pattern.PatternOptions |= PcreOptions.Singleline;
                                    break;

                                case 'x':
                                    pattern.PatternOptions |= PcreOptions.IgnorePatternWhitespace;
                                    break;

                                case 'g':
                                    pattern.AllMatches = true;
                                    break;

                                case ' ':
                                    break;

                                default:
                                    throw InvalidInputException("Unknown option: " + part);
                            }
                        }
                        break;
                }
            }
        }

        protected InvalidOperationException InvalidInputException(string message, Exception innerException = null)
        {
            return new InvalidOperationException(String.Format("{0} at line {1}: {2}", message, _lineNumber, _lastReadLine), innerException);
        }
    }
}
