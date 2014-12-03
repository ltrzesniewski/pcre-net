using System;
using System.Text;

namespace PCRE.Tests.Pcre
{
    public static class StringExtensions
    {
        public static string Escape(this string str)
        {
            if (str == null)
                return null;

            var sb = new StringBuilder();

            foreach (var c in str)
            {
                if (c == '\\')
                    sb.Append(@"\\");
                else if (c < 32 || c > 126)
                    sb.AppendFormat(@"\x{0:X2}", (short)c);
                else
                    sb.Append(c);
            }

            return sb.ToString();
        }

        public static string UnescapeSubject(this string str)
        {
            if (str == null)
                return null;

            var sb = new StringBuilder();

            for (var i = 0; i < str.Length; ++i)
            {
                if (str[i] == '\\')
                {
                    ++i;
                    if (i >= str.Length)
                        break;

                    switch (str[i])
                    {
                        case '\\':
                            sb.Append('\\');
                            break;
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
                        case 'e':
                            sb.Append('\x1B');
                            break;

                        case 'x':
                            UnescapeHex(str, sb, ref i);
                            break;

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

        public static string UnescapeGroup(this string str)
        {
            if (str == null)
                return null;

            var sb = new StringBuilder();

            for (var i = 0; i < str.Length; ++i)
            {
                if (str[i] == '\\')
                {
                    ++i;
                    if (i >= str.Length)
                    {
                        sb.Append('\\');
                        break;
                    }

                    if (str[i] == 'x')
                    {
                        UnescapeHex(str, sb, ref i);
                        continue;
                    }

                    sb.Append('\\');
                }

                sb.Append(str[i]);
            }

            return sb.ToString();
        }

        private static void UnescapeHex(string str, StringBuilder sb, ref int i)
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
                return;
            }

            if (i + 2 < str.Length)
            {
                var c = str[i + 2];

                if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'))
                {
                    sb.Append((char)Convert.ToUInt16(str.Substring(i + 1, 2), 16));
                    i += 2;
                    return;
                }
            }
            sb.Append((char)Convert.ToUInt16(str.Substring(i + 1, 1), 16));
            ++i;
        }
    }
}
