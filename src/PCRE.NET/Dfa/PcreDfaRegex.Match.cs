using System;
using System.Diagnostics.Contracts;
using PCRE.Wrapper;

namespace PCRE.Dfa
{
    public static class PcreDfaRegex
    {
        // ReSharper disable IntroduceOptionalParameters.Global

        [Pure]
        public static PcreDfaMatchSet DfaMatch(this PcreRegex regex, string subject)
        {
            return DfaMatch(regex, subject, 0, PcreDfaMatchOptions.None);
        }

        [Pure]
        public static PcreDfaMatchSet DfaMatch(this PcreRegex regex, string subject, PcreDfaMatchOptions options)
        {
            return DfaMatch(regex, subject, 0, options);
        }

        [Pure]
        public static PcreDfaMatchSet DfaMatch(this PcreRegex regex, string subject, int startIndex)
        {
            return DfaMatch(regex, subject, startIndex, PcreDfaMatchOptions.None);
        }

        [Pure]
        public static PcreDfaMatchSet DfaMatch(this PcreRegex regex, string subject, int startIndex, PcreDfaMatchOptions options)
        {
            var settings = new PcreDfaMatchSettings
            {
                AdditionalOptions = options,
                StartIndex = startIndex
            };

            return DfaMatch(regex, subject, settings);
        }

        public static PcreDfaMatchSet DfaMatch(this PcreRegex regex, string subject, PcreDfaMatchSettings settings)
        {
            if (subject == null)
                throw new ArgumentNullException("subject");

            if (settings == null)
                throw new ArgumentNullException("settings");

            if (settings.StartIndex < 0 || settings.StartIndex > subject.Length)
                throw new IndexOutOfRangeException("Invalid StartIndex value");

            using (var context = settings.CreateMatchContext(subject))
            {
                return new PcreDfaMatchSet(ExecuteDfaMatch(regex, context));
            }
        }

        private static MatchData ExecuteDfaMatch(PcreRegex regex, MatchContext context)
        {
            try
            {
                return regex.InternalRegex.DfaMatch(context);
            }
            catch (MatchException ex)
            {
                throw PcreMatchException.FromException(ex);
            }
        }
    }
}
