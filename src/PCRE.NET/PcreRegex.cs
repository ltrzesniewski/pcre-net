using System;
using System.Collections.Generic;
using PCRE.Support;
using PCRE.Wrapper;

namespace PCRE
{
    public sealed partial class PcreRegex
    {
        // ReSharper disable IntroduceOptionalParameters.Global, MemberCanBePrivate.Global, UnusedMember.Global

        private static readonly PriorityCache<RegexKey, InternalRegex> RegexCache = new PriorityCache<RegexKey, InternalRegex>(15, CreateRegex);
        private static readonly PriorityCache<string, Func<PcreMatch, string>> ReplacementCache = new PriorityCache<string, Func<PcreMatch, string>>(15, ReplacementPattern.Parse);

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

        public static int CacheSize
        {
            get { return RegexCache.CacheSize; }
            set
            {
                RegexCache.CacheSize = value;
                ReplacementCache.CacheSize = value;
            }
        }

        public PcreRegex(string pattern)
            : this(pattern, PcreOptions.None)
        {
        }

        public PcreRegex(string pattern, PcreOptions options)
        {
            if (pattern == null)
                throw new ArgumentNullException("pattern");

            var key = new RegexKey(pattern, options);
            _re = RegexCache.GetOrAdd(key);

            PaternInfo = new PcrePatternInfo(_re, key);
        }

        private static InternalRegex CreateRegex(RegexKey key)
        {
            return new InternalRegex(key.Pattern, key.Options.ToPatternOptions(), key.Options.ToStudyOptions());
        }
    }
}
