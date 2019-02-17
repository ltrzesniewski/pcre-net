using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using PCRE.Internal;

namespace PCRE.Dfa
{
    public sealed class PcreDfaRegex
    {
        private readonly InternalRegex _regex;

        internal PcreDfaRegex(InternalRegex regex)
        {
            _regex = regex;
        }

        [Pure]
        public PcreDfaMatchResult Match(string subject)
            => Match(subject, 0, PcreDfaMatchOptions.None);

        [Pure]
        public PcreDfaMatchResult Match(string subject, PcreDfaMatchOptions options)
            => Match(subject, 0, options);

        [Pure]
        public PcreDfaMatchResult Match(string subject, int startIndex)
            => Match(subject, startIndex, PcreDfaMatchOptions.None);

        [Pure]
        public PcreDfaMatchResult Match(string subject, int startIndex, PcreDfaMatchOptions options)
        {
            var settings = new PcreDfaMatchSettings
            {
                AdditionalOptions = options,
                StartIndex = startIndex
            };

            return Match(subject, settings);
        }

        public PcreDfaMatchResult Match(string subject, PcreDfaMatchSettings settings)
        {
            if (subject == null)
                throw new ArgumentNullException(nameof(subject));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (settings.StartIndex < 0 || settings.StartIndex > subject.Length)
                throw new IndexOutOfRangeException("Invalid StartIndex value");

            return _regex.DfaMatch(subject, settings, settings.StartIndex);
        }

        [Pure]
        public IEnumerable<PcreDfaMatchResult> Matches(string subject)
            => Matches(subject, 0);

        [Pure]
        public IEnumerable<PcreDfaMatchResult> Matches(string subject, int startIndex)
        {
            var settings = new PcreDfaMatchSettings
            {
                StartIndex = startIndex
            };

            return Matches(subject, settings);
        }

        [Pure]
        public IEnumerable<PcreDfaMatchResult> Matches(string subject, PcreDfaMatchSettings settings)
        {
            if (subject == null)
                throw new ArgumentNullException(nameof(subject));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (settings.StartIndex < 0 || settings.StartIndex > subject.Length)
                throw new IndexOutOfRangeException("Invalid StartIndex value");

            return MatchesIterator(subject, settings);
        }

        private IEnumerable<PcreDfaMatchResult> MatchesIterator(string subject, PcreDfaMatchSettings settings)
        {
            var match = _regex.DfaMatch(subject, settings, settings.StartIndex);
            if (!match.Success)
                yield break;

            yield return match;

            while (true)
            {
                match = _regex.DfaMatch(subject, settings, match.Index + 1);
                if (!match.Success)
                    yield break;

                yield return match;
            }
        }
    }
}
