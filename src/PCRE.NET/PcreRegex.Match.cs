using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using PCRE.Internal;

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
        public bool IsMatch(ReadOnlySpan<char> subject)
            => IsMatch(subject, 0);

        [Pure]
        public bool IsMatch(string subject, int startIndex)
            => IsMatch(subject.AsSpan(), startIndex);

        [Pure]
        public bool IsMatch(ReadOnlySpan<char> subject, int startIndex)
            => Match(subject, startIndex).Success;

        [Pure]
        public PcreMatch Match(string subject)
            => Match(subject, PcreMatchSettings.Default);

        [Pure]
        public PcreRefMatch Match(ReadOnlySpan<char> subject)
            => Match(subject, PcreMatchSettings.Default);

        [Pure]
        public PcreMatch Match(string subject, PcreMatchOptions options)
            => Match(subject, 0, options, null);

        [Pure]
        public PcreRefMatch Match(ReadOnlySpan<char> subject, PcreMatchOptions options)
            => Match(subject, 0, options, null);

        [Pure]
        public PcreMatch Match(string subject, int startIndex)
            => Match(subject, startIndex, PcreMatchOptions.None, null);

        [Pure]
        public PcreRefMatch Match(ReadOnlySpan<char> subject, int startIndex)
            => Match(subject, startIndex, PcreMatchOptions.None, null);

        [Pure]
        public PcreMatch Match(string subject, int startIndex, PcreMatchOptions options)
            => Match(subject, startIndex, options, null);

        [Pure]
        public PcreRefMatch Match(ReadOnlySpan<char> subject, int startIndex, PcreMatchOptions options)
            => Match(subject, startIndex, options, null);

        public PcreMatch Match(string subject, Func<PcreCallout, PcreCalloutResult> onCallout)
            => Match(subject, 0, PcreMatchOptions.None, onCallout);

        public PcreRefMatch Match(ReadOnlySpan<char> subject, Func<PcreCallout, PcreCalloutResult> onCallout)
            => Match(subject, 0, PcreMatchOptions.None, onCallout);

        public PcreMatch Match(string subject, PcreMatchOptions options, Func<PcreCallout, PcreCalloutResult> onCallout)
            => Match(subject, 0, options, onCallout);

        public PcreRefMatch Match(ReadOnlySpan<char> subject, PcreMatchOptions options, Func<PcreCallout, PcreCalloutResult> onCallout)
            => Match(subject, 0, options, onCallout);

        public PcreMatch Match(string subject, int startIndex, Func<PcreCallout, PcreCalloutResult> onCallout)
            => Match(subject, startIndex, PcreMatchOptions.None, onCallout);

        public PcreRefMatch Match(ReadOnlySpan<char> subject, int startIndex, Func<PcreCallout, PcreCalloutResult> onCallout)
            => Match(subject, startIndex, PcreMatchOptions.None, onCallout);

        public PcreMatch Match(string subject, int startIndex, PcreMatchOptions options, Func<PcreCallout, PcreCalloutResult> onCallout)
            => Match(subject, PcreMatchSettings.GetSettings(startIndex, options, onCallout));

        public PcreRefMatch Match(ReadOnlySpan<char> subject, int startIndex, PcreMatchOptions options, Func<PcreCallout, PcreCalloutResult> onCallout)
            => Match(subject, PcreMatchSettings.GetSettings(startIndex, options, onCallout));

        public PcreMatch Match(string subject, PcreMatchSettings settings)
        {
            if (subject == null)
                throw new ArgumentNullException(nameof(subject));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (settings.StartIndex < 0 || settings.StartIndex > subject.Length)
                throw new IndexOutOfRangeException("Invalid StartIndex value");

            return InternalRegex.Match(subject, settings, settings.StartIndex, settings.AdditionalOptions.ToPatternOptions());
        }

        public PcreRefMatch Match(ReadOnlySpan<char> subject, PcreMatchSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (settings.StartIndex < 0 || settings.StartIndex > subject.Length)
                throw new IndexOutOfRangeException("Invalid StartIndex value");

            return InternalRegex.Match(subject, settings, settings.StartIndex, settings.AdditionalOptions.ToPatternOptions());
        }

        [Pure]
        public IEnumerable<PcreMatch> Matches(string subject)
            => Matches(subject, PcreMatchSettings.Default);

        [Pure]
        public RefMatchEnumerator Matches(ReadOnlySpan<char> subject)
            => Matches(subject, PcreMatchSettings.Default);

        [Pure]
        public IEnumerable<PcreMatch> Matches(string subject, int startIndex)
            => Matches(subject, startIndex, null);

        [Pure]
        public RefMatchEnumerator Matches(ReadOnlySpan<char> subject, int startIndex)
            => Matches(subject, startIndex, null);

        [Pure]
        public IEnumerable<PcreMatch> Matches(string subject, int startIndex, Func<PcreCallout, PcreCalloutResult> onCallout)
        {
            if (subject == null)
                throw new ArgumentNullException(nameof(subject));

            return Matches(subject, PcreMatchSettings.GetSettings(startIndex, PcreMatchOptions.None, onCallout));
        }

        [Pure]
        public RefMatchEnumerator Matches(ReadOnlySpan<char> subject, int startIndex, Func<PcreCallout, PcreCalloutResult> onCallout)
        {
            if (subject == null)
                throw new ArgumentNullException(nameof(subject));

            return Matches(subject, PcreMatchSettings.GetSettings(startIndex, PcreMatchOptions.None, onCallout));
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

        [Pure]
        public RefMatchEnumerator Matches(ReadOnlySpan<char> subject, PcreMatchSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (settings.StartIndex < 0 || settings.StartIndex > subject.Length)
                throw new IndexOutOfRangeException("Invalid StartIndex value");

            return new RefMatchEnumerator(InternalRegex, subject, settings);
        }

        private IEnumerable<PcreMatch> MatchesIterator(string subject, PcreMatchSettings settings)
        {
            var match = InternalRegex.Match(subject, settings, settings.StartIndex, settings.AdditionalOptions.ToPatternOptions());
            if (!match.Success)
                yield break;

            yield return match;

            var baseOptions = settings.AdditionalOptions.ToPatternOptions() | PcreConstants.NO_UTF_CHECK;

            while (true)
            {
                var startIndex = match.GetStartOfNextMatchIndex();
                var options = baseOptions | (match.Length == 0 ? PcreConstants.NOTEMPTY_ATSTART : 0);

                match = InternalRegex.Match(subject, settings, startIndex, options);
                if (!match.Success)
                    yield break;

                yield return match;
            }
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

        public ref struct RefMatchEnumerator
        {
            private readonly ReadOnlySpan<char> _subject;
            private readonly PcreMatchSettings _settings;
            private InternalRegex _regex;
            private PcreRefMatch _match;

            internal RefMatchEnumerator(InternalRegex regex, ReadOnlySpan<char> subject, PcreMatchSettings settings)
            {
                _regex = regex;
                _subject = subject;
                _settings = settings;
                _match = default;
            }

            public PcreRefMatch Current => _match;

            public bool MoveNext()
            {
                if (_regex == null)
                    return false;

                if (!_match.Success)
                    return Match(_settings.StartIndex, 0);

                return Match(_match.GetStartOfNextMatchIndex(), PcreConstants.NO_UTF_CHECK | (_match.Length == 0 ? PcreConstants.NOTEMPTY_ATSTART : 0));
            }

            private bool Match(int startIndex, uint additionalOptions)
            {
                _match = _regex.Match(_subject, _settings, startIndex, _settings.AdditionalOptions.ToPatternOptions() | additionalOptions);
                if (_match.Success)
                    return true;

                _regex = null;
                return false;
            }
        }
    }
}
