using System;
using System.Diagnostics.Contracts;
using System.Text;
using PCRE.Support;

namespace PCRE
{
    public partial class PcreRegex
    {
        // ReSharper disable IntroduceOptionalParameters.Global, MemberCanBePrivate.Global, UnusedMember.Global

        [Pure]
        public string Replace(string subject, string replacement)
            => Replace(subject, replacement, -1, 0);

        [Pure]
        public string Replace(string subject, string replacement, int count)
            => Replace(subject, replacement, count, 0);

        [Pure]
        public string Replace(string subject, string replacement, int count, int startIndex)
        {
            if (replacement == null)
                throw new ArgumentNullException(nameof(replacement));

            return Replace(subject, Caches.ReplacementCache.GetOrAdd(replacement), count, startIndex);
        }

        public string Replace(string subject, Func<PcreMatch, string> replacementFunc)
            => Replace(subject, replacementFunc, -1, 0);

        public string Replace(string subject, Func<PcreMatch, string> replacementFunc, int count)
            => Replace(subject, replacementFunc, count, 0);

        public string Replace(string subject, Func<PcreMatch, string> replacementFunc, int count, int startIndex)
        {
            if (subject == null)
                throw new ArgumentNullException(nameof(subject));
            if (replacementFunc == null)
                throw new ArgumentNullException(nameof(replacementFunc));

            if (count == 0)
                return subject;

            StringBuilder sb = null;
            var position = 0;

            foreach (var match in Matches(subject, startIndex))
            {
                if (sb == null)
                    sb = new StringBuilder((int)(subject.Length * 1.2));
                sb.Append(subject, position, match.Index - position);
                sb.Append(replacementFunc(match));
                position = match.GetStartOfNextMatchIndex();

                if (--count == 0)
                    break;
            }
            if (sb == null)
                return subject;

            sb.Append(subject, position, subject.Length - position);
            return sb.ToString();
        }

        [Pure]
        public static string Replace(string subject, string pattern, string replacement)
            => Replace(subject, pattern, replacement, PcreOptions.None, -1, 0);

        [Pure]
        public static string Replace(string subject, string pattern, string replacement, PcreOptions options)
            => Replace(subject, pattern, replacement, options, -1, 0);

        [Pure]
        public static string Replace(string subject, string pattern, string replacement, PcreOptions options, int count)
            => Replace(subject, pattern, replacement, options, count, 0);

        [Pure]
        public static string Replace(string subject, string pattern, string replacement, PcreOptions options, int count, int startIndex)
            => new PcreRegex(pattern, options).Replace(subject, replacement, count, startIndex);

        public static string Replace(string subject, string pattern, Func<PcreMatch, string> replacementFunc)
            => Replace(subject, pattern, replacementFunc, PcreOptions.None, -1, 0);

        public static string Replace(string subject, string pattern, Func<PcreMatch, string> replacementFunc, PcreOptions options)
            => Replace(subject, pattern, replacementFunc, options, -1, 0);

        public static string Replace(string subject, string pattern, Func<PcreMatch, string> replacementFunc, PcreOptions options, int count)
            => Replace(subject, pattern, replacementFunc, options, count, 0);

        public static string Replace(string subject, string pattern, Func<PcreMatch, string> replacementFunc, PcreOptions options, int count, int startIndex)
            => new PcreRegex(pattern, options).Replace(subject, replacementFunc, count, startIndex);
    }
}
