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
        {
            return Replace(subject, replacement, -1, 0);
        }

        [Pure]
        public string Replace(string subject, string replacement, int count)
        {
            return Replace(subject, replacement, count, 0);
        }

        [Pure]
        public string Replace(string subject, string replacement, int count, int startIndex)
        {
            if (replacement == null)
                throw new ArgumentNullException("replacement");

            return Replace(subject, ReplacementPattern.Parse(replacement), count, startIndex);
        }

        public string Replace(string subject, Func<PcreMatch, string> replacementFunc)
        {
            return Replace(subject, replacementFunc, -1, 0);
        }

        public string Replace(string subject, Func<PcreMatch, string> replacementFunc, int count)
        {
            return Replace(subject, replacementFunc, count, 0);
        }

        public string Replace(string subject, Func<PcreMatch, string> replacementFunc, int count, int startIndex)
        {
            if (subject == null)
                throw new ArgumentNullException("subject");
            if (replacementFunc == null)
                throw new ArgumentNullException("replacementFunc");

            if (count == 0)
                return subject;

            var sb = new StringBuilder((int)(subject.Length * 1.2));
            var position = 0;

            foreach (var match in Matches(subject, startIndex))
            {
                sb.Append(subject, position, match.Index - position);
                sb.Append(replacementFunc(match));
                position = match.Index + match.Length;

                if (--count == 0)
                    break;
            }

            sb.Append(subject, position, subject.Length - position);
            return sb.ToString();
        }
    }
}
