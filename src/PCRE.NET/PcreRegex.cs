using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using PCRE.Support;
using PCRE.Wrapper;

namespace PCRE
{
    public sealed class PcreRegex
    {
        // ReSharper disable IntroduceOptionalParameters.Global, MemberCanBePrivate.Global, UnusedMember.Global

        private readonly PcrePattern _re;

        public PcrePatternInfo PaternInfo { get; private set; }

        internal int CaptureCount
        {
            get { return _re.CaptureCount; }
        }

        internal Dictionary<string, int> CaptureNameMap
        {
            get { return _re.CaptureNames; }
        }

        public PcreRegex(string pattern, PcreOptions options = PcreOptions.None)
        {
            if (pattern == null)
                throw new ArgumentNullException("pattern");

            _re = new PcrePattern(pattern, options.ToPatternOptions(), options.ToStudyOptions());
            PaternInfo = new PcrePatternInfo(_re, pattern, options);
        }

        [Pure]
        public bool IsMatch(string subject)
        {
            return IsMatch(subject, 0);
        }

        [Pure]
        public bool IsMatch(string subject, int startIndex)
        {
            if (subject == null)
                throw new ArgumentNullException("subject");

            if (startIndex < 0 || startIndex > subject.Length)
                throw new ArgumentOutOfRangeException("startIndex");

            return _re.IsMatch(subject, startIndex);
        }

        [Pure]
        public PcreMatch Match(string subject)
        {
            return Match(subject, 0);
        }

        [Pure]
        public PcreMatch Match(string subject, int startIndex)
        {
            if (subject == null)
                throw new ArgumentNullException("subject");

            if (startIndex < 0 || startIndex > subject.Length)
                throw new ArgumentOutOfRangeException("startIndex");

            var offsets = _re.Match(subject, startIndex, PcrePatternOptions.None);
            return offsets.IsMatch
                ? new PcreMatch(this, subject, offsets)
                : null;
        }

        [Pure]
        public IEnumerable<PcreMatch> Matches(string subject)
        {
            return Matches(subject, 0);
        }

        [Pure]
        public IEnumerable<PcreMatch> Matches(string subject, int startIndex)
        {
            if (subject == null)
                throw new ArgumentNullException("subject");

            if (startIndex < 0 || startIndex > subject.Length)
                throw new ArgumentOutOfRangeException("startIndex");

            return MatchesIterator(subject, startIndex);
        }

        private IEnumerable<PcreMatch> MatchesIterator(string subject, int startIndex)
        {
            var offsets = _re.Match(subject, startIndex, PcrePatternOptions.None);

            if (!offsets.IsMatch)
                yield break;

            var match = new PcreMatch(this, subject, offsets);
            yield return match;

            while (true)
            {
                var nextOffset = match.Index + match.Length;
                offsets = _re.Match(subject, nextOffset, match.Length == 0 ? PcrePatternOptions.NotEmptyAtStart : PcrePatternOptions.None);

                if (!offsets.IsMatch)
                    yield break;

                match = new PcreMatch(this, subject, offsets);
                yield return match;
            }
        }

        [Pure]
        public string Replace(string subject, string replacement)
        {
            return Replace(subject, replacement, -1, 0);
        }

        [Pure]
        public string Replace(string subject, string replacement, int count)
        {
            return Replace(subject, replacement, count, 0);
        }

        [Pure]
        public string Replace(string subject, string replacement, int count, int startIndex)
        {
            if (replacement == null)
                throw new ArgumentNullException("replacement");

            return Replace(subject, ReplacementPattern.Parse(this, replacement), count, startIndex);
        }

        public string Replace(string subject, Func<PcreMatch, string> replacementFunc)
        {
            return Replace(subject, replacementFunc, -1, 0);
        }

        public string Replace(string subject, Func<PcreMatch, string> replacementFunc, int count)
        {
            return Replace(subject, replacementFunc, count, 0);
        }

        public string Replace(string subject, Func<PcreMatch, string> replacementFunc, int count, int startIndex)
        {
            if (subject == null)
                throw new ArgumentNullException("subject");
            if (replacementFunc == null)
                throw new ArgumentNullException("replacementFunc");

            if (count == 0)
                return subject;

            var sb = new StringBuilder((int)(subject.Length * 1.2));
            var position = 0;

            foreach (var match in Matches(subject, startIndex))
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

        [Pure]
        public IEnumerable<string> Split(string subject)
        {
            return Split(subject, -1, 0);
        }

        [Pure]
        public IEnumerable<string> Split(string subject, int count)
        {
            return Split(subject, count, 0);
        }

        [Pure]
        public IEnumerable<string> Split(string subject, int count, int startIndex)
        {
            if (subject == null)
                throw new ArgumentNullException("subject");
            if (startIndex < 0 || startIndex > subject.Length)
                throw new ArgumentOutOfRangeException("startIndex");

            return SplitIterator(subject, count, startIndex);
        }

        private IEnumerable<string> SplitIterator(string subject, int count, int startIndex)
        {
            if (count == 0)
            {
                yield return subject;
                yield break;
            }

            var index = 0;

            foreach (var match in Matches(subject, startIndex))
            {
                yield return subject.Substring(index, match.Index - index);
                index = match.Index + match.Length;

                if (--count == 0)
                    break;
            }

            yield return subject.Substring(index);
        }
    }
}
