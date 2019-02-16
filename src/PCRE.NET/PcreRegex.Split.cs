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
            => Split(subject, PcreSplitOptions.None, -1, 0);

        [Pure]
        public IEnumerable<string> Split(string subject, PcreSplitOptions options)
            => Split(subject, options, -1, 0);

        [Pure]
        public IEnumerable<string> Split(string subject, int count)
            => Split(subject, PcreSplitOptions.None, count, 0);

        [Pure]
        public IEnumerable<string> Split(string subject, int count, int startIndex)
            => Split(subject, PcreSplitOptions.None, count, startIndex);

        [Pure]
        public IEnumerable<string> Split(string subject, PcreSplitOptions options, int count, int startIndex)
        {
            if (subject == null)
                throw new ArgumentNullException(nameof(subject));
            if (startIndex < 0 || startIndex > subject.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            return SplitIterator(subject, options, count, startIndex);
        }

        private IEnumerable<string> SplitIterator(string subject, PcreSplitOptions options, int count, int startIndex)
        {
            if (count == 0)
            {
                yield return subject;
                yield break;
            }

            throw new NotImplementedException();

//            var index = 0;
//            var captureCount = CaptureCount;
//            var includeGroupValues = (options & PcreSplitOptions.IncludeGroupValues) != 0;
////
//            foreach (var match in Matches(subject, startIndex))
//            {
//                yield return subject.Substring(index, match.Index - index);
//                index = match.GetStartOfNextMatchIndex();
//
//                if (includeGroupValues)
//                {
//                    for (var groupIdx = 1; groupIdx <= captureCount; ++groupIdx)
//                    {
//                        var group = match[groupIdx];
//                        if (group.Success)
//                            yield return group.Value;
//                    }
//                }
//
//                if (--count == 0)
//                    break;
//            }
//
//            yield return subject.Substring(index);
        }

        [Pure]
        public static IEnumerable<string> Split(string subject, string pattern)
            => Split(subject, pattern, PcreOptions.None, PcreSplitOptions.None, -1, 0);

        [Pure]
        public static IEnumerable<string> Split(string subject, string pattern, PcreOptions options)
            => Split(subject, pattern, options, PcreSplitOptions.None, -1, 0);

        [Pure]
        public static IEnumerable<string> Split(string subject, string pattern, int count)
            => Split(subject, pattern, PcreOptions.None, PcreSplitOptions.None, count, 0);

        [Pure]
        public static IEnumerable<string> Split(string subject, string pattern, PcreOptions options, PcreSplitOptions splitOptions)
            => Split(subject, pattern, options, splitOptions, -1, 0);

        [Pure]
        public static IEnumerable<string> Split(string subject, string pattern, PcreOptions options, PcreSplitOptions splitOptions, int count)
            => Split(subject, pattern, options, splitOptions, count, 0);

        [Pure]
        public static IEnumerable<string> Split(string subject, string pattern, PcreOptions options, PcreSplitOptions splitOptions, int count, int startIndex)
            => new PcreRegex(pattern, options).Split(subject, splitOptions, count, startIndex);
    }
}
