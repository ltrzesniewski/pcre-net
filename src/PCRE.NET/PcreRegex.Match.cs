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
            var settings = new PcreMatchSettings
            {
                StartIndex = startIndex,
                AdditionalOptions = options
            };

            if (onCallout != null)
                settings.OnCallout += onCallout;

            return Match(subject, settings);
        }

        public PcreMatch Match(string subject, PcreMatchSettings settings)
        {
            if (subject == null)
                throw new ArgumentNullException("subject");

            if (settings == null)
                throw new ArgumentNullException("settings");

            if (settings.StartIndex < 0 || settings.StartIndex > subject.Length)
                throw new IndexOutOfRangeException("Invalid StartIndex value");

            using (var context = settings.CreateMatchContext(subject))
            {
                var result = InternalRegex.Match(context);
                return new PcreMatch(result);
            }
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

            var settings = new PcreMatchSettings
            {
                StartIndex = startIndex
            };

            if (onCallout != null)
                settings.OnCallout += onCallout;

            return MatchesIterator(subject, settings);
        }

        [Pure]
        public IEnumerable<PcreMatch> Matches(string subject, PcreMatchSettings settings)
        {
            if (subject == null)
                throw new ArgumentNullException("subject");

            if (settings == null)
                throw new ArgumentNullException("settings");

            if (settings.StartIndex < 0 || settings.StartIndex > subject.Length)
                throw new IndexOutOfRangeException("Invalid StartIndex value");

            return MatchesIterator(subject, settings);
        }

        private IEnumerable<PcreMatch> MatchesIterator(string subject, PcreMatchSettings settings)
        {
            using (var context = settings.CreateMatchContext(subject))
            {
                var result = InternalRegex.Match(context);

                if (result.ResultCode != MatchResultCode.Success)
                    yield break;

                var match = new PcreMatch(result);
                yield return match;

                var options = context.AdditionalOptions;

                while (true)
                {
                    context.StartIndex = match.GetStartOfNextMatchIndex();
                    context.AdditionalOptions = options | (match.Length == 0 ? PatternOptions.NotEmptyAtStart : PatternOptions.None);

                    result = InternalRegex.Match(context);

                    if (result.ResultCode != MatchResultCode.Success)
                        yield break;

                    match = new PcreMatch(result);
                    yield return match;
                }
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
    }
}
