using System;
using System.Collections.Generic;
using System.Text;

namespace PCRE.Support
{
    internal static class ReplacementPattern
    {
        public static Func<PcreMatch, string> Parse(PcreRegex regex, string replacementPattern)
        {
            // TODO : Refactor & improve

            var placeholderIndex = replacementPattern.IndexOf('$');

            if (placeholderIndex < 0)
                return match => replacementPattern;

            var replacementParts = new List<object>();
            var currentIndex = 0;

            while (true)
            {
                if (placeholderIndex > currentIndex)
                    replacementParts.Add(replacementPattern.Substring(currentIndex, placeholderIndex - currentIndex));

                currentIndex = placeholderIndex + 1;
                var endIntIndex = currentIndex;

                while (endIntIndex < replacementPattern.Length)
                {
                    var c = replacementPattern[endIntIndex];
                    if (c < '0' || c > '9')
                        break;
                    ++endIntIndex;
                }

                if (endIntIndex == currentIndex)
                {
                    replacementParts.Add("$");
                }
                else
                {
                    var groupIndex = Int32.Parse(replacementPattern.Substring(currentIndex, endIntIndex - currentIndex));

                    if (groupIndex <= regex.CaptureCount)
                        replacementParts.Add(groupIndex);
                    else
                        replacementParts.Add(replacementPattern.Substring(currentIndex - 1, endIntIndex - currentIndex + 1));

                    currentIndex = endIntIndex;
                }

                placeholderIndex = replacementPattern.IndexOf('$', currentIndex);
                if (placeholderIndex < 0)
                    break;
            }

            if (currentIndex < replacementPattern.Length)
                replacementParts.Add(replacementPattern.Substring(currentIndex));

            return match =>
            {
                var sb = new StringBuilder();

                foreach (var item in replacementParts)
                {
                    var str = item as string;
                    if (str != null)
                    {
                        sb.Append(str);
                        continue;
                    }

                    var groupIndex = (int)item;
                    sb.Append(match[groupIndex].Value);
                }

                return sb.ToString();
            };
        }
    }
}
