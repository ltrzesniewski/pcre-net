using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using PCRE.Wrapper;

namespace PCRE.Dfa
{
    public sealed class PcreDfaRegex
    {
        // ReSharper disable IntroduceOptionalParameters.Global

        private readonly PcreRegex _regex;

        internal PcreDfaRegex(PcreRegex regex)
        {
            _regex = regex;
        }

        [Pure]
        public PcreDfaMatchResult Match(string subject)
        {
            return Match(subject, 0, PcreDfaMatchOptions.None);
        }

        [Pure]
        public PcreDfaMatchResult Match(string subject, PcreDfaMatchOptions options)
        {
            return Match(subject, 0, options);
        }

        [Pure]
        public PcreDfaMatchResult Match(string subject, int startIndex)
        {
            return Match(subject, startIndex, PcreDfaMatchOptions.None);
        }

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
                throw new ArgumentNullException("subject");

            if (settings == null)
                throw new ArgumentNullException("settings");

            if (settings.StartIndex < 0 || settings.StartIndex > subject.Length)
                throw new IndexOutOfRangeException("Invalid StartIndex value");

            using (var context = settings.CreateMatchContext(subject))
            {
                return new PcreDfaMatchResult(ExecuteDfaMatch(context));
            }
        }

        [Pure]
        public IEnumerable<PcreDfaMatchResult> Matches(string subject)
        {
            return Matches(subject, 0);
        }

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
                throw new ArgumentNullException("subject");

            if (settings == null)
                throw new ArgumentNullException("settings");

            if (settings.StartIndex < 0 || settings.StartIndex > subject.Length)
                throw new IndexOutOfRangeException("Invalid StartIndex value");

            return MatchesIterator(subject, settings);
        }

        private IEnumerable<PcreDfaMatchResult> MatchesIterator(string subject, PcreDfaMatchSettings settings)
        {
            using (var context = settings.CreateMatchContext(subject))
            {
                var result = ExecuteDfaMatch(context);

                if (result.ResultCode != MatchResultCode.Success)
                    yield break;

                var match = new PcreDfaMatchResult(result);
                yield return match;

                while (true)
                {
                    context.StartIndex = match.Index + 1;

                    result = ExecuteDfaMatch(context);

                    if (result.ResultCode != MatchResultCode.Success)
                        yield break;

                    match = new PcreDfaMatchResult(result);
                    yield return match;
                }
            }
        }

        private MatchData ExecuteDfaMatch(MatchContext context)
        {
            try
            {
                return _regex.InternalRegex.DfaMatch(context);
            }
            catch (MatchException ex)
            {
                throw PcreMatchException.FromException(ex);
            }
        }
    }
}
