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
            => IsMatch(subject.AsSpan(), 0);

        [Pure]
        public bool IsMatch(ReadOnlySpan<char> subject)
            => IsMatch(subject, 0);

        [Pure]
        public bool IsMatch(string subject, int startIndex)
            => IsMatch(subject.AsSpan(), startIndex);

        [Pure]
        public bool IsMatch(ReadOnlySpan<char> subject, int startIndex)
        {
            if (unchecked((uint)startIndex > (uint)subject.Length))
                ThrowInvalidStartIndex();

            var outputVector = InternalRegex.CanStackAllocOutputVector
                ? stackalloc uint[InternalRegex.OutputVectorSize]
                : new uint[InternalRegex.OutputVectorSize];

            var match = InternalRegex.CreateRefMatch(outputVector);
            match.FirstMatch(subject, PcreMatchSettings.Default, startIndex, 0, null, null);

            return match.Success;
        }

        [Pure]
        public PcreMatch Match(string subject)
            => Match(subject, 0, PcreMatchOptions.None, null, PcreMatchSettings.Default);

        [Pure]
        public PcreRefMatch Match(ReadOnlySpan<char> subject)
            => Match(subject, 0, PcreMatchOptions.None, null, PcreMatchSettings.Default);

        [Pure]
        public PcreMatch Match(string subject, PcreMatchOptions options)
            => Match(subject, 0, options, null, PcreMatchSettings.Default);

        [Pure]
        public PcreRefMatch Match(ReadOnlySpan<char> subject, PcreMatchOptions options)
            => Match(subject, 0, options, null, PcreMatchSettings.Default);

        [Pure]
        public PcreMatch Match(string subject, int startIndex)
            => Match(subject, startIndex, PcreMatchOptions.None, null, PcreMatchSettings.Default);

        [Pure]
        public PcreRefMatch Match(ReadOnlySpan<char> subject, int startIndex)
            => Match(subject, startIndex, PcreMatchOptions.None, null, PcreMatchSettings.Default);

        [Pure]
        public PcreMatch Match(string subject, int startIndex, PcreMatchOptions options)
            => Match(subject, startIndex, options, null, PcreMatchSettings.Default);

        [Pure]
        public PcreRefMatch Match(ReadOnlySpan<char> subject, int startIndex, PcreMatchOptions options)
            => Match(subject, startIndex, options, null, PcreMatchSettings.Default);

        public PcreMatch Match(string subject, Func<PcreCallout, PcreCalloutResult>? onCallout)
            => Match(subject, 0, PcreMatchOptions.None, onCallout, PcreMatchSettings.Default);

        public PcreRefMatch Match(ReadOnlySpan<char> subject, PcreRefCalloutFunc? onCallout)
            => Match(subject, 0, PcreMatchOptions.None, onCallout, PcreMatchSettings.Default);

        public PcreMatch Match(string subject, PcreMatchOptions options, Func<PcreCallout, PcreCalloutResult>? onCallout)
            => Match(subject, 0, options, onCallout, PcreMatchSettings.Default);

        public PcreRefMatch Match(ReadOnlySpan<char> subject, PcreMatchOptions options, PcreRefCalloutFunc? onCallout)
            => Match(subject, 0, options, onCallout, PcreMatchSettings.Default);

        public PcreMatch Match(string subject, int startIndex, Func<PcreCallout, PcreCalloutResult>? onCallout)
            => Match(subject, startIndex, PcreMatchOptions.None, onCallout, PcreMatchSettings.Default);

        public PcreRefMatch Match(ReadOnlySpan<char> subject, int startIndex, PcreRefCalloutFunc? onCallout)
            => Match(subject, startIndex, PcreMatchOptions.None, onCallout, PcreMatchSettings.Default);

        public PcreMatch Match(string subject, int startIndex, PcreMatchOptions options, Func<PcreCallout, PcreCalloutResult>? onCallout)
            => Match(subject, startIndex, options, onCallout, PcreMatchSettings.Default);

        public PcreRefMatch Match(ReadOnlySpan<char> subject, int startIndex, PcreMatchOptions options, PcreRefCalloutFunc? onCallout)
            => Match(subject, startIndex, options, onCallout, PcreMatchSettings.Default);

        public PcreMatch Match(string subject, int startIndex, PcreMatchOptions options, Func<PcreCallout, PcreCalloutResult>? onCallout, PcreMatchSettings settings)
        {
            if (subject == null)
                throw new ArgumentNullException(nameof(subject));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (unchecked((uint)startIndex > (uint)subject.Length))
                ThrowInvalidStartIndex();

            return InternalRegex.Match(subject, settings, startIndex, options.ToPatternOptions(), onCallout);
        }

        public PcreRefMatch Match(ReadOnlySpan<char> subject, int startIndex, PcreMatchOptions options, PcreRefCalloutFunc? onCallout, PcreMatchSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (unchecked((uint)startIndex > (uint)subject.Length))
                ThrowInvalidStartIndex();

            var match = InternalRegex.CreateRefMatch();
            match.FirstMatch(subject, settings, startIndex, options, onCallout, null);

            return match;
        }

        [Pure]
        public IEnumerable<PcreMatch> Matches(string subject)
            => Matches(subject, 0, PcreMatchOptions.None, null, PcreMatchSettings.Default);

        [Pure]
        public RefMatchEnumerable Matches(ReadOnlySpan<char> subject)
            => Matches(subject, 0, PcreMatchOptions.None, null, PcreMatchSettings.Default);

        [Pure]
        public IEnumerable<PcreMatch> Matches(string subject, int startIndex)
            => Matches(subject, startIndex, PcreMatchOptions.None, null, PcreMatchSettings.Default);

        [Pure]
        public RefMatchEnumerable Matches(ReadOnlySpan<char> subject, int startIndex)
            => Matches(subject, startIndex, PcreMatchOptions.None, null, PcreMatchSettings.Default);

        [Pure]
        public IEnumerable<PcreMatch> Matches(string subject, int startIndex, Func<PcreCallout, PcreCalloutResult>? onCallout)
            => Matches(subject, startIndex, PcreMatchOptions.None, onCallout, PcreMatchSettings.Default);

        [Pure]
        public RefMatchEnumerable Matches(ReadOnlySpan<char> subject, int startIndex, PcreRefCalloutFunc? onCallout)
            => Matches(subject, startIndex, PcreMatchOptions.None, onCallout, PcreMatchSettings.Default);

        [Pure]
        public IEnumerable<PcreMatch> Matches(string subject, int startIndex, PcreMatchOptions options, Func<PcreCallout, PcreCalloutResult>? onCallout, PcreMatchSettings settings)
        {
            if (subject == null)
                throw new ArgumentNullException(nameof(subject));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (unchecked((uint)startIndex > (uint)subject.Length))
                ThrowInvalidStartIndex();

            return MatchesIterator(subject, startIndex, options, onCallout, settings);
        }

        [Pure]
        public RefMatchEnumerable Matches(ReadOnlySpan<char> subject, int startIndex, PcreMatchOptions options, PcreRefCalloutFunc? onCallout, PcreMatchSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (unchecked((uint)startIndex > (uint)subject.Length))
                ThrowInvalidStartIndex();

            return new RefMatchEnumerable(InternalRegex, subject, startIndex, options, onCallout, settings);
        }

        private IEnumerable<PcreMatch> MatchesIterator(string subject, int startIndex, PcreMatchOptions options, Func<PcreCallout, PcreCalloutResult>? onCallout, PcreMatchSettings settings)
        {
            var match = InternalRegex.Match(subject, settings, startIndex, options.ToPatternOptions(), onCallout);
            if (!match.Success)
                yield break;

            yield return match;

            var baseOptions = options.ToPatternOptions() | PcreConstants.NO_UTF_CHECK;

            while (true)
            {
                var nextOptions = baseOptions | (match.Length == 0 ? PcreConstants.NOTEMPTY_ATSTART : 0);

                match = InternalRegex.Match(subject, settings, match.GetStartOfNextMatchIndex(), nextOptions, onCallout);
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

        private static void ThrowInvalidStartIndex()
            => throw new ArgumentOutOfRangeException("Invalid start index.", default(Exception));

        public readonly ref struct RefMatchEnumerable
        {
            private readonly ReadOnlySpan<char> _subject;
            private readonly int _startIndex;
            private readonly PcreMatchOptions _options;
            private readonly PcreRefCalloutFunc? _callout;
            private readonly PcreMatchSettings _settings;
            private readonly InternalRegex _regex;

            internal RefMatchEnumerable(InternalRegex regex,
                                        ReadOnlySpan<char> subject,
                                        int startIndex,
                                        PcreMatchOptions options,
                                        PcreRefCalloutFunc? callout,
                                        PcreMatchSettings settings)
            {
                _regex = regex;
                _subject = subject;
                _startIndex = startIndex;
                _options = options;
                _callout = callout;
                _settings = settings;
            }

            public RefMatchEnumerator GetEnumerator()
                => new(_regex, _subject, _startIndex, _options, _callout, _settings);

            [SuppressMessage("Microsoft.Design", "CA1002")]
            [SuppressMessage("Microsoft.Design", "CA1062")]
            public List<T> ToList<T>(PcreRefMatch.Func<T> selector)
            {
                var result = new List<T>();

                foreach (var item in this)
                    result.Add(selector(item));

                return result;
            }
        }

        public ref struct RefMatchEnumerator
        {
            private readonly ReadOnlySpan<char> _subject;
            private readonly int _startIndex;
            private readonly PcreMatchOptions _options;
            private readonly PcreRefCalloutFunc? _callout;
            private readonly PcreMatchSettings _settings;
            private InternalRegex? _regex;
            private PcreRefMatch _match;

            internal RefMatchEnumerator(InternalRegex regex,
                                        ReadOnlySpan<char> subject,
                                        int startIndex,
                                        PcreMatchOptions options,
                                        PcreRefCalloutFunc? callout,
                                        PcreMatchSettings settings)
            {
                _regex = regex;
                _subject = subject;
                _startIndex = startIndex;
                _options = options;
                _callout = callout;
                _settings = settings;
                _match = default;
            }

            public readonly PcreRefMatch Current => _match;

            public bool MoveNext()
            {
                if (_regex == null)
                    return false;

                if (!_match.IsInitialized)
                {
                    _match = _regex.CreateRefMatch();
                    _match.FirstMatch(_subject, _settings, _startIndex, _options, _callout, null);
                }
                else
                {
                    _match.NextMatch(_settings, _options, _callout, null, false);
                }

                if (_match.Success)
                    return true;

                _regex = null;
                return false;
            }
        }
    }
}
