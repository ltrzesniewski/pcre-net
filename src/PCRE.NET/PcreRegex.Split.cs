using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace PCRE
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "IntroduceOptionalParameters.Global")]
    public partial class PcreRegex
    {
        /// <include file='PcreRegex.xml' path='/doc/method[@name="Split"]/*'/>
        /// <include file='PcreRegex.xml' path='/doc/param[@name="subject"]'/>
        [Pure]
        public IEnumerable<string> Split(string subject)
            => Split(subject, PcreSplitOptions.None, -1, 0);

        /// <include file='PcreRegex.xml' path='/doc/method[@name="Split"]/*'/>
        /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="options"]'/>
        [Pure]
        public IEnumerable<string> Split(string subject, PcreSplitOptions options)
            => Split(subject, options, -1, 0);

        /// <include file='PcreRegex.xml' path='/doc/method[@name="Split"]/*'/>
        /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="count"]'/>
        [Pure]
        public IEnumerable<string> Split(string subject, int count)
            => Split(subject, PcreSplitOptions.None, count, 0);

        /// <include file='PcreRegex.xml' path='/doc/method[@name="Split"]/*'/>
        /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="count" or @name="startIndex"]'/>
        /// <remarks>
        /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex"]/*'/>
        /// </remarks>
        [Pure]
        public IEnumerable<string> Split(string subject, int count, int startIndex)
            => Split(subject, PcreSplitOptions.None, count, startIndex);

        /// <include file='PcreRegex.xml' path='/doc/method[@name="Split"]/*'/>
        /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="options" or @name="count" or @name="startIndex"]'/>
        /// <remarks>
        /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex"]/*'/>
        /// </remarks>
        [Pure]
        public IEnumerable<string> Split(string subject, PcreSplitOptions options, int count, int startIndex)
        {
            if (subject == null)
                throw new ArgumentNullException(nameof(subject));
            if (startIndex < 0 || startIndex > subject.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            return SplitIterator(subject, options, count, startIndex);
        }

        private IEnumerable<string> SplitIterator(string subject, PcreSplitOptions options, int count, int startIndex)
        {
            if (count == 0)
            {
                yield return subject;
                yield break;
            }

            var index = 0;
            var captureCount = InternalRegex.CaptureCount;
            var includeGroupValues = (options & PcreSplitOptions.IncludeGroupValues) != 0;

            foreach (var match in Matches(subject, startIndex))
            {
                yield return subject.Substring(index, match.Index - index);
                index = match.GetStartOfNextMatchIndex();

                if (includeGroupValues)
                {
                    for (var groupIdx = 1; groupIdx <= captureCount; ++groupIdx)
                    {
                        var group = match[groupIdx];
                        if (group.Success)
                            yield return group.Value;
                    }
                }

                if (--count == 0)
                    break;
            }

            yield return subject.Substring(index);
        }

        /// <include file='PcreRegex.xml' path='/doc/method[@name="Split"]/*'/>
        /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="pattern"]'/>
        /// <remarks>
        /// <include file='PcreRegex.xml' path='/doc/remarks[@name="static"]/*'/>
        /// </remarks>
        [Pure]
        public static IEnumerable<string> Split(string subject, string pattern)
            => Split(subject, pattern, PcreOptions.None, PcreSplitOptions.None, -1, 0);

        /// <include file='PcreRegex.xml' path='/doc/method[@name="Split"]/*'/>
        /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="pattern" or @name="options"]'/>
        /// <remarks>
        /// <include file='PcreRegex.xml' path='/doc/remarks[@name="static"]/*'/>
        /// </remarks>
        [Pure]
        public static IEnumerable<string> Split(string subject, string pattern, PcreOptions options)
            => Split(subject, pattern, options, PcreSplitOptions.None, -1, 0);

        /// <include file='PcreRegex.xml' path='/doc/method[@name="Split"]/*'/>
        /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="pattern" or @name="count"]'/>
        /// <remarks>
        /// <include file='PcreRegex.xml' path='/doc/remarks[@name="static"]/*'/>
        /// </remarks>
        [Pure]
        public static IEnumerable<string> Split(string subject, string pattern, int count)
            => Split(subject, pattern, PcreOptions.None, PcreSplitOptions.None, count, 0);

        /// <include file='PcreRegex.xml' path='/doc/method[@name="Split"]/*'/>
        /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="pattern" or @name="options" or @name="splitOptions"]'/>
        /// <remarks>
        /// <include file='PcreRegex.xml' path='/doc/remarks[@name="static"]/*'/>
        /// </remarks>
        [Pure]
        public static IEnumerable<string> Split(string subject, string pattern, PcreOptions options, PcreSplitOptions splitOptions)
            => Split(subject, pattern, options, splitOptions, -1, 0);

        /// <include file='PcreRegex.xml' path='/doc/method[@name="Split"]/*'/>
        /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="pattern" or @name="options" or @name="splitOptions" or @name="count"]'/>
        /// <remarks>
        /// <include file='PcreRegex.xml' path='/doc/remarks[@name="static"]/*'/>
        /// </remarks>
        [Pure]
        public static IEnumerable<string> Split(string subject, string pattern, PcreOptions options, PcreSplitOptions splitOptions, int count)
            => Split(subject, pattern, options, splitOptions, count, 0);

        /// <include file='PcreRegex.xml' path='/doc/method[@name="Split"]/*'/>
        /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="pattern" or @name="options" or @name="splitOptions" or @name="count" or @name="startIndex"]'/>
        /// <remarks>
        /// <include file='PcreRegex.xml' path='/doc/remarks[@name="static" or @name="startIndex"]/*'/>
        /// </remarks>
        [Pure]
        public static IEnumerable<string> Split(string subject, string pattern, PcreOptions options, PcreSplitOptions splitOptions, int count, int startIndex)
            => new PcreRegex(pattern, options).Split(subject, splitOptions, count, startIndex);
    }
}
