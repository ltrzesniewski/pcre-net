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
            => Match(subject, PcreDfaMatchSettings.GetSettings(startIndex, options));

        public PcreDfaMatchResult Match(string subject, PcreDfaMatchSettings settings)
        {
            if (subject == null)
                throw new ArgumentNullException(nameof(subject));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (settings.StartIndex < 0 || settings.StartIndex > subject.Length)
                throw new ArgumentException("Invalid StartIndex value");

            return _regex.DfaMatch(subject, settings, settings.StartIndex, ((PcreMatchOptions)settings.AdditionalOptions).ToPatternOptions());
        }

        [Pure]
        public IEnumerable<PcreDfaMatchResult> Matches(string subject)
            => Matches(subject, 0);

        [Pure]
        public IEnumerable<PcreDfaMatchResult> Matches(string subject, int startIndex)
            => Matches(subject, PcreDfaMatchSettings.GetSettings(startIndex, PcreDfaMatchOptions.None));

        [Pure]
        public IEnumerable<PcreDfaMatchResult> Matches(string subject, PcreDfaMatchSettings settings)
        {
            if (subject == null)
                throw new ArgumentNullException(nameof(subject));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (settings.StartIndex < 0 || settings.StartIndex > subject.Length)
                throw new ArgumentException("Invalid StartIndex value");

            return MatchesIterator(subject, settings);
        }

        private IEnumerable<PcreDfaMatchResult> MatchesIterator(string subject, PcreDfaMatchSettings settings)
        {
            var additionalOptions = ((PcreMatchOptions)settings.AdditionalOptions).ToPatternOptions();

            var match = _regex.DfaMatch(subject, settings, settings.StartIndex, additionalOptions);
            if (!match.Success)
                yield break;

            yield return match;

            additionalOptions |= PcreConstants.NO_UTF_CHECK;

            while (true)
            {
                var nextIndex = match.Index + 1;

                if (nextIndex > subject.Length)
                    yield break;

                if (nextIndex < subject.Length && char.IsLowSurrogate(subject[nextIndex]))
                    ++nextIndex;

                match = _regex.DfaMatch(subject, settings, nextIndex, additionalOptions);
                if (!match.Success)
                    yield break;

                yield return match;
            }
        }
    }
}
