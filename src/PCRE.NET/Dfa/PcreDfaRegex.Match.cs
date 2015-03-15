using System;
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
        public PcreDfaMatchSet Match(string subject)
        {
            return Match(subject, 0, PcreDfaMatchOptions.None);
        }

        [Pure]
        public PcreDfaMatchSet Match(string subject, PcreDfaMatchOptions options)
        {
            return Match(subject, 0, options);
        }

        [Pure]
        public PcreDfaMatchSet Match(string subject, int startIndex)
        {
            return Match(subject, startIndex, PcreDfaMatchOptions.None);
        }

        [Pure]
        public PcreDfaMatchSet Match(string subject, int startIndex, PcreDfaMatchOptions options)
        {
            var settings = new PcreDfaMatchSettings
            {
                AdditionalOptions = options,
                StartIndex = startIndex
            };

            return Match(subject, settings);
        }

        public PcreDfaMatchSet Match(string subject, PcreDfaMatchSettings settings)
        {
            if (subject == null)
                throw new ArgumentNullException("subject");

            if (settings == null)
                throw new ArgumentNullException("settings");

            if (settings.StartIndex < 0 || settings.StartIndex > subject.Length)
                throw new IndexOutOfRangeException("Invalid StartIndex value");

            using (var context = settings.CreateMatchContext(subject))
            {
                return new PcreDfaMatchSet(ExecuteDfaMatch(context));
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
