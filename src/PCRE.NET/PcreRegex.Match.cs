using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using PCRE.Support;
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
            return Match(subject, startIndex).Success;
        }

        [Pure]
        public PcreMatch Match(string subject)
        {
            return Match(subject, 0, PcreMatchOptions.None, null);
        }

        [Pure]
        public PcreMatch Match(string subject, PcreMatchOptions options)
        {
            return Match(subject, 0, options, null);
        }

        [Pure]
        public PcreMatch Match(string subject, int startIndex)
        {
            return Match(subject, startIndex, PcreMatchOptions.None, null);
        }

        [Pure]
        public PcreMatch Match(string subject, int startIndex, PcreMatchOptions options)
        {
            return Match(subject, startIndex, options, null);
        }

        public PcreMatch Match(string subject, Func<PcreCallout, PcreCalloutResult> onCallout)
        {
            return Match(subject, 0, PcreMatchOptions.None, onCallout);
        }

        public PcreMatch Match(string subject, PcreMatchOptions options, Func<PcreCallout, PcreCalloutResult> onCallout)
        {
            return Match(subject, 0, options, onCallout);
        }

        public PcreMatch Match(string subject, int startIndex, Func<PcreCallout, PcreCalloutResult> onCallout)
        {
            return Match(subject, startIndex, PcreMatchOptions.None, onCallout);
        }

        public PcreMatch Match(string subject, int startIndex, PcreMatchOptions options, Func<PcreCallout, PcreCalloutResult> onCallout)
        {
            if (subject == null)
                throw new ArgumentNullException("subject");

            if (startIndex < 0 || startIndex > subject.Length)
                throw new ArgumentOutOfRangeException("startIndex");

            var result = InternalRegex.Match(subject, startIndex, options.ToPatternOptions(), WrapCallout(onCallout));
            return new PcreMatch(result);
        }

        [Pure]
        public IEnumerable<PcreMatch> Matches(string subject)
        {
            return Matches(subject, 0, null);
        }

        [Pure]
        public IEnumerable<PcreMatch> Matches(string subject, int startIndex)
        {
            return Matches(subject, startIndex, null);
        }

        [Pure]
        public IEnumerable<PcreMatch> Matches(string subject, int startIndex, Func<PcreCallout, PcreCalloutResult> onCallout)
        {
            if (subject == null)
                throw new ArgumentNullException("subject");

            if (startIndex < 0 || startIndex > subject.Length)
                throw new ArgumentOutOfRangeException("startIndex");

            return MatchesIterator(subject, startIndex, WrapCallout(onCallout));
        }

        private IEnumerable<PcreMatch> MatchesIterator(string subject, int startIndex, Func<CalloutData, CalloutResult> onCallout)
        {
            var result = InternalRegex.Match(subject, startIndex, PatternOptions.None, onCallout);

            if (result.ResultCode != MatchResultCode.Success)
                yield break;

            var match = new PcreMatch(result);
            yield return match;

            while (true)
            {
                var nextOffset = match.GetStartOfNextMatchIndex();
                result = InternalRegex.Match(subject, nextOffset, match.Length == 0 ? PatternOptions.NotEmptyAtStart : PatternOptions.None, onCallout);

                if (result.ResultCode != MatchResultCode.Success)
                    yield break;

                match = new PcreMatch(result);
                yield return match;
            }
        }

        [Pure]
        public static bool IsMatch(string subject, string pattern)
        {
            return IsMatch(subject, pattern, PcreOptions.None, 0);
        }

        [Pure]
        public static bool IsMatch(string subject, string pattern, PcreOptions options)
        {
            return IsMatch(subject, pattern, options, 0);
        }

        [Pure]
        public static bool IsMatch(string subject, string pattern, PcreOptions options, int startIndex)
        {
            return new PcreRegex(pattern, options).IsMatch(subject, startIndex);
        }

        [Pure]
        public static PcreMatch Match(string subject, string pattern)
        {
            return Match(subject, pattern, PcreOptions.None, 0);
        }

        [Pure]
        public static PcreMatch Match(string subject, string pattern, PcreOptions options)
        {
            return Match(subject, pattern, options, 0);
        }

        [Pure]
        public static PcreMatch Match(string subject, string pattern, PcreOptions options, int startIndex)
        {
            return new PcreRegex(pattern, options).Match(subject, startIndex);
        }

        [Pure]
        public static IEnumerable<PcreMatch> Matches(string subject, string pattern)
        {
            return Matches(subject, pattern, PcreOptions.None, 0);
        }

        [Pure]
        public static IEnumerable<PcreMatch> Matches(string subject, string pattern, PcreOptions options)
        {
            return Matches(subject, pattern, options, 0);
        }

        [Pure]
        public static IEnumerable<PcreMatch> Matches(string subject, string pattern, PcreOptions options, int startIndex)
        {
            return new PcreRegex(pattern, options).Matches(subject, startIndex);
        }

        private static Func<CalloutData, CalloutResult> WrapCallout(Func<PcreCallout, PcreCalloutResult> callout)
        {
            if (callout == null)
                return null;

            return data => (CalloutResult)callout(new PcreCallout(data));
        }
    }
}
