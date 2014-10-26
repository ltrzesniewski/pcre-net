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
            _reader = new StreamReader(inputStream);
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
            var options = PcreOptions.ASCII;

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
                        options |= PcreOptions.AllowDuplicateNames;
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
                        options |= (PcreOptions)0x04000000; // PCRE_NO_START_OPTIMIZE
                        break;

                    case ' ':
                        break;

                    default:
                        throw InvalidInputException("Unknown modifier: " + c);
                }
            }

            pattern.PatternOptions = options;
        }

        public static string Unescape(string str)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < str.Length; ++i)
            {
                if (str[i] == '\\')
                {
                    ++i;
                    if (i >= str.Length)
                    {
                        sb.Append('\n');
                        break;
                    }

                    switch (str[i])
                    {
                        case 't':
                            sb.Append('\t');
                            break;
                        case 'n':
                            sb.Append('\n');
                            break;
                        case 'r':
                            sb.Append('\r');
                            break;
                        case 'f':
                            sb.Append('\f');
                            break;
                        case 'a':
                            sb.Append('\a');
                            break;
                        case 'e': // TODO handle escape codes
                            sb.Append('\x1B');
                            break;

                        case 'x':
                        {
                            if (i + 1 >= str.Length)
                                throw new InvalidOperationException("Invalid hex escape: " + str);

                            if (str[i + 1] == '{')
                            {
                                var endIndex = i;
                                while (endIndex < str.Length && str[endIndex] != '}')
                                    ++endIndex;

                                sb.Append((char)Convert.ToUInt16(str.Substring(i + 2, endIndex - i - 2), 16));
                                i = endIndex;
                                break;
                            }

                            if (i + 2 < str.Length)
                            {
                                var c = str[i + 2];

                                if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'))
                                {
                                    sb.Append((char)Convert.ToUInt16(str.Substring(i + 1, 2), 16));
                                    i += 2;
                                    break;
                                }
                            }
                            sb.Append((char)Convert.ToUInt16(str.Substring(i + 1, 1), 16));
                            ++i;
                            break;
                        }

                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        {
                            var endIndex = i;
                            while (endIndex < str.Length && endIndex - i < 3 && str[endIndex] >= '0' && str[endIndex] <= '7')
                                ++endIndex;

                            sb.Append((char)Convert.ToUInt16(str.Substring(i, endIndex - i), 8));
                            i += endIndex - i - 1;
                            break;
                        }

                        default:
                            sb.Append(str[i]);
                            break;
                    }
                }
                else
                {
                    sb.Append(str[i]);
                }
            }

            return sb.ToString();
        }

        protected InvalidOperationException InvalidInputException(string message, Exception innerException = null)
        {
            return new InvalidOperationException(String.Format("{0} at line {1}: {2}", message, _lineNumber, _lastReadLine), innerException);
        }
    }
}
