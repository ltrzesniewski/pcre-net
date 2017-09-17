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

                if (line.StartsWith("\\="))
                    continue;

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
                var args = part.Split('=');

                var name = args[0];
                var value = args.Length == 2 ? args[1] : null;

                switch (name)
                {
                    case "aftertext":
                    case "allaftertext":
                        pattern.GetRemainingString = true;
                        break;

                    case "dupnames":
                        pattern.PatternOptions |= PcreOptions.DupNames;
                        break;

                    case "mark":
                        pattern.ExtractMarks = true;
                        break;

                    case "no_start_optimize":
                        pattern.PatternOptions |= PcreOptions.NoStartOptimize;
                        break;

                    case "hex":
                        pattern.HexEncoding = true;
                        break;

                    case "dollar_endonly":
                        pattern.PatternOptions |= PcreOptions.DollarEndOnly;
                        break;

                    case "anchored":
                        pattern.PatternOptions |= PcreOptions.Anchored;
                        break;

                    case "ungreedy":
                        pattern.PatternOptions |= PcreOptions.Ungreedy;
                        break;

                    case "global":
                    case "altglobal":
                        pattern.AllMatches = true;
                        break;

                    case "no_auto_possess":
                        pattern.PatternOptions |= PcreOptions.NoAutoPossess;
                        break;

                    case "no_auto_capture":
                        pattern.PatternOptions |= PcreOptions.ExplicitCapture;
                        break;

                    case "auto_callout":
                        pattern.PatternOptions |= PcreOptions.AutoCallout;
                        break;

                    case "firstline":
                        pattern.PatternOptions |= PcreOptions.FirstLine;
                        break;

                    case "alt_bsux":
                        pattern.PatternOptions |= PcreOptions.AltBsUX;
                        break;

                    case "allow_empty_class":
                        pattern.PatternOptions |= PcreOptions.AllowEmptyClass;
                        break;

                    case "match_unset_backref":
                        pattern.PatternOptions |= PcreOptions.MatchUnsetBackref;
                        break;

                    case "allcaptures":
                        break;

                    case "replace":
                        pattern.ReplaceWith = value;
                        break;

                    case "info":
                        pattern.IncludeInfo = true;
                        break;

                    case "no_dotstar_anchor":
                        pattern.PatternOptions |= PcreOptions.NoDotStarAnchor;
                        break;

                    case "dotall":
                        pattern.PatternOptions |= PcreOptions.Singleline;
                        break;

                    case "subject_literal":
                        pattern.SubjectLiteral = true;
                        break;

                    case "newline": // TODO
                    case "bsr":
                    case "startchar":
                    case "stackguard":
                    case "parens_nest_limit":
                        break;

                    case "jitstack": // TODO
                        pattern.NotSupported = true;
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
                                    if ((pattern.PatternOptions & PcreOptions.Extended) != 0)
                                        pattern.PatternOptions = pattern.PatternOptions & ~PcreOptions.Extended | PcreOptions.ExtendedMore;
                                    else
                                        pattern.PatternOptions |= PcreOptions.Extended;
                                    break;

                                case 'g':
                                    pattern.AllMatches = true;
                                    break;

                                case 'B':
                                    break;

                                case 'I':
                                    pattern.IncludeInfo = true;
                                    break;

                                case '\\':
                                    pattern.Pattern += "\\";
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
            return new InvalidOperationException($"{message} at line {_lineNumber}: {_lastReadLine}", innerException);
        }
    }
}
