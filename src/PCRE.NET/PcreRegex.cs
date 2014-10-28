using System;
using System.Collections.Generic;
using PCRE.Support;
using PCRE.Wrapper;

namespace PCRE
{
    public sealed partial class PcreRegex
    {
        // ReSharper disable IntroduceOptionalParameters.Global, MemberCanBePrivate.Global, UnusedMember.Global

        private readonly InternalRegex _re;

        public PcrePatternInfo PaternInfo { get; private set; }

        internal int CaptureCount
        {
            get { return _re.CaptureCount; }
        }

        internal Dictionary<string, int[]> CaptureNameMap
        {
            get { return _re.CaptureNames; }
        }

        public PcreRegex(string pattern)
            : this(pattern, PcreOptions.None)
        {
        }

        public PcreRegex(string pattern, PcreOptions options)
        {
            if (pattern == null)
                throw new ArgumentNullException("pattern");

            _re = new InternalRegex(pattern, options.ToPatternOptions(), options.ToStudyOptions());
            PaternInfo = new PcrePatternInfo(_re, pattern, options);
        }
    }
}
