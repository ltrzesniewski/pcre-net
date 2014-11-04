using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace PCRE
{
    public partial class PcreRegex
    {
        // ReSharper disable IntroduceOptionalParameters.Global, MemberCanBePrivate.Global, UnusedMember.Global

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

        [Pure]
        public static IEnumerable<string> Split(string subject, string pattern)
        {
            return Split(subject, pattern, PcreOptions.None, -1, 0);
        }

        [Pure]
        public static IEnumerable<string> Split(string subject, string pattern, PcreOptions options)
        {
            return Split(subject, pattern, options, -1, 0);
        }

        [Pure]
        public static IEnumerable<string> Split(string subject, string pattern, PcreOptions options, int count)
        {
            return Split(subject, pattern, options, count, 0);
        }

        [Pure]
        public static IEnumerable<string> Split(string subject, string pattern, PcreOptions options, int count, int startIndex)
        {
            return new PcreRegex(pattern, options).Split(subject, count, startIndex);
        }
    }
}
