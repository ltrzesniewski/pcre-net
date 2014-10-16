using System;
using System.Collections.Generic;
using System.Text;
using PCRE.Support;
using PCRE.Wrapper;

namespace PCRE
{
    public sealed class PcreRegex
    {
        private readonly PcrePattern _re;

        public PcrePatternInfo PaternInfo { get; private set; }

        internal int CaptureCount
        {
            get { return _re.CaptureCount; }
        }

        public PcreRegex(string pattern, PcreOptions options = PcreOptions.None)
        {
            if (pattern == null)
                throw new ArgumentNullException("pattern");

            _re = new PcrePattern(pattern, options.ToPatternOptions(), options.ToStudyOptions());
            PaternInfo = new PcrePatternInfo(_re);
        }

        public bool IsMatch(string subject)
        {
            if (subject == null)
                throw new ArgumentNullException("subject");

            return _re.IsMatch(subject);
        }

        public PcreMatch Match(string subject)
        {
            if (subject == null)
                throw new ArgumentNullException("subject");

            var offsets = _re.FirstMatch(subject);
            return offsets.IsMatch
                ? new PcreMatch(this, subject, offsets)
                : null;
        }

        public IEnumerable<PcreMatch> Matches(string subject)
        {
            if (subject == null)
                throw new ArgumentNullException("subject");

            return MatchesIterator(subject);
        }

        private IEnumerable<PcreMatch> MatchesIterator(string subject)
        {
            var offsets = _re.FirstMatch(subject);

            if (!offsets.IsMatch)
                yield break;

            var match = new PcreMatch(this, subject, offsets);
            yield return match;

            while (true)
            {
                var nextOffset = match.Index + match.Length;
                offsets = _re.NextMatch(subject, nextOffset);

                if (!offsets.IsMatch)
                    yield break;

                match = new PcreMatch(this, subject, offsets);
                yield return match;
            }
        }

        public string Replace(string subject, string replacement)
        {
            if (replacement == null)
                throw new ArgumentNullException("replacement");

            return Replace(subject, BuildReplacementFunc(replacement));
        }

        public string Replace(string subject, Func<PcreMatch, string> replacementFunc)
        {
            return Replace(subject, replacementFunc, -1);
        }

        public string Replace(string subject, Func<PcreMatch, string> replacementFunc, int count)
        {
            if (subject == null)
                throw new ArgumentNullException("subject");
            if (replacementFunc == null)
                throw new ArgumentNullException("replacementFunc");

            if (count == 0)
                return subject;

            var sb = new StringBuilder((int)(subject.Length * 1.2));
            var position = 0;

            foreach (var match in Matches(subject))
            {
                sb.Append(subject, position, match.Index - position);
                sb.Append(replacementFunc(match));
                position = match.Index + match.Length;

                if (--count == 0)
                    break;
            }

            sb.Append(subject, position, subject.Length - position);
            return sb.ToString();
        }

        private static Func<PcreMatch, string> BuildReplacementFunc(string replacementPattern)
        {
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
                    replacementParts.Add(groupIndex);
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
