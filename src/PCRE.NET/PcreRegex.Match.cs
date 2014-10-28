using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using PCRE.Wrapper;

namespace PCRE
{
    public partial class PcreRegex
    {
        // ReSharper disable IntroduceOptionalParameters.Global, MemberCanBePrivate.Global, UnusedMember.Global

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

            var offsets = _re.Match(subject, startIndex, PatternOptions.None);
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
            var offsets = _re.Match(subject, startIndex, PatternOptions.None);

            if (!offsets.IsMatch)
                yield break;

            var match = new PcreMatch(this, subject, offsets);
            yield return match;

            while (true)
            {
                var nextOffset = match.Index + match.Length;
                offsets = _re.Match(subject, nextOffset, match.Length == 0 ? PatternOptions.NotEmptyAtStart : PatternOptions.None);

                if (!offsets.IsMatch)
                    yield break;

                match = new PcreMatch(this, subject, offsets);
                yield return match;
            }
        }
    }
}
