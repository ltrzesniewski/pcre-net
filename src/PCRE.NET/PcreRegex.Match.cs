using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using PCRE.Internal;
using PCRE.Support;

namespace PCRE
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "IntroduceOptionalParameters.Global")]
    public partial class PcreRegex
    {
        [Pure]
        public bool IsMatch(string subject)
            => IsMatch(subject, 0);

        [Pure]
        public bool IsMatch(string subject, int startIndex)
            => Match(subject, startIndex).Success;

        [Pure]
        public PcreMatch Match(string subject)
            => Match(subject, 0, PcreMatchOptions.None, null);

        [Pure]
        public PcreMatch Match(string subject, PcreMatchOptions options)
            => Match(subject, 0, options, null);

        [Pure]
        public PcreMatch Match(string subject, int startIndex)
            => Match(subject, startIndex, PcreMatchOptions.None, null);

        [Pure]
        public PcreMatch Match(string subject, int startIndex, PcreMatchOptions options)
            => Match(subject, startIndex, options, null);

        public PcreMatch Match(string subject, Func<PcreCallout, PcreCalloutResult> onCallout)
            => Match(subject, 0, PcreMatchOptions.None, onCallout);

        public PcreMatch Match(string subject, PcreMatchOptions options, Func<PcreCallout, PcreCalloutResult> onCallout)
            => Match(subject, 0, options, onCallout);

        public PcreMatch Match(string subject, int startIndex, Func<PcreCallout, PcreCalloutResult> onCallout)
            => Match(subject, startIndex, PcreMatchOptions.None, onCallout);

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
                throw new ArgumentNullException(nameof(subject));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (settings.StartIndex < 0 || settings.StartIndex > subject.Length)
                throw new IndexOutOfRangeException("Invalid StartIndex value");

            return InternalMatch(subject, settings);
        }

        [Pure]
        public IEnumerable<PcreMatch> Matches(string subject)
            => Matches(subject, 0, null);

        [Pure]
        public IEnumerable<PcreMatch> Matches(string subject, int startIndex)
            => Matches(subject, startIndex, null);

        [Pure]
        public IEnumerable<PcreMatch> Matches(string subject, int startIndex, Func<PcreCallout, PcreCalloutResult> onCallout)
        {
            if (subject == null)
                throw new ArgumentNullException(nameof(subject));

            var settings = new PcreMatchSettings
            {
                StartIndex = startIndex
            };

            if (onCallout != null)
                settings.OnCallout += onCallout;

            return Matches(subject, settings);
        }

        [Pure]
        public IEnumerable<PcreMatch> Matches(string subject, PcreMatchSettings settings)
        {
            if (subject == null)
                throw new ArgumentNullException(nameof(subject));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (settings.StartIndex < 0 || settings.StartIndex > subject.Length)
                throw new IndexOutOfRangeException("Invalid StartIndex value");

            return MatchesIterator(subject, settings);
        }

        private IEnumerable<PcreMatch> MatchesIterator(string subject, PcreMatchSettings settings)
        {
            var match = InternalMatch(subject, settings);
            if (!match.Success)
                yield break;

            yield return match;

            var baseOptions = settings.AdditionalOptions.ToPatternOptions();

            while (true)
            {
                var startIndex = match.GetStartOfNextMatchIndex();
                var options = baseOptions | (match.Length == 0 ? PcreConstants.NOTEMPTY_ATSTART : 0);

                match = InternalMatch(subject, settings, startIndex, options);

                if (!match.Success)
                    yield break;

                yield return match;
            }
        }

        private PcreMatch InternalMatch(string subject, PcreMatchSettings settings)
        {
            var input = new Native.match_input();
            settings.FillMatchInput(ref input);
            return InternalRegex.Match(subject, ref input);
        }

        private PcreMatch InternalMatch(string subject, PcreMatchSettings settings, int startIndex, uint additionalOptions)
        {
            var input = new Native.match_input();
            settings.FillMatchInput(ref input);
            input.start_index = (uint)startIndex;
            input.additional_options = additionalOptions;
            return InternalRegex.Match(subject, ref input);
        }

        [Pure]
        public static bool IsMatch(string subject, string pattern)
            => IsMatch(subject, pattern, PcreOptions.None, 0);

        [Pure]
        public static bool IsMatch(string subject, string pattern, PcreOptions options)
            => IsMatch(subject, pattern, options, 0);

        [Pure]
        public static bool IsMatch(string subject, string pattern, PcreOptions options, int startIndex)
            => new PcreRegex(pattern, options).IsMatch(subject, startIndex);

        [Pure]
        public static PcreMatch Match(string subject, string pattern)
            => Match(subject, pattern, PcreOptions.None, 0);

        [Pure]
        public static PcreMatch Match(string subject, string pattern, PcreOptions options)
            => Match(subject, pattern, options, 0);

        [Pure]
        public static PcreMatch Match(string subject, string pattern, PcreOptions options, int startIndex)
            => new PcreRegex(pattern, options).Match(subject, startIndex);

        [Pure]
        public static IEnumerable<PcreMatch> Matches(string subject, string pattern)
            => Matches(subject, pattern, PcreOptions.None, 0);

        [Pure]
        public static IEnumerable<PcreMatch> Matches(string subject, string pattern, PcreOptions options)
            => Matches(subject, pattern, options, 0);

        [Pure]
        public static IEnumerable<PcreMatch> Matches(string subject, string pattern, PcreOptions options, int startIndex)
            => new PcreRegex(pattern, options).Matches(subject, startIndex);
    }
}
