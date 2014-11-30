using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PCRE.Support;
using PCRE.Wrapper;

namespace PCRE
{
    public sealed partial class PcreRegex
    {
        // ReSharper disable IntroduceOptionalParameters.Global, MemberCanBePrivate.Global, UnusedMember.Global

        private static readonly PriorityCache<RegexKey, object> RegexCache = new PriorityCache<RegexKey, object>(15, CreateRegex);
        private static readonly PriorityCache<string, Func<PcreMatch, string>> ReplacementCache = new PriorityCache<string, Func<PcreMatch, string>>(15, ReplacementPattern.Parse);

        // Cannot store an InternalRegex field, because the x64 JIT may try to load this type before executing the module initializer in certain cases.
        // This type should always be loadable before the module initializer is executed.
        private readonly object _re;
        private PcrePatternInfo _info;

        public PcrePatternInfo PaternInfo
        {
            get { return _info ?? (_info = new PcrePatternInfo(this)); }
        }

        internal RegexKey Key { get; private set; }

        internal InternalRegex InternalRegex
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return (InternalRegex)_re; }
        }

        internal int CaptureCount
        {
            get { return InternalRegex.CaptureCount; }
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

            Key = new RegexKey(pattern, options);
            _re = RegexCache.GetOrAdd(Key);
        }

        private static object CreateRegex(RegexKey key)
        {
            return new InternalRegex(key.Pattern, key.Options.ToPatternOptions(), key.Options.ToStudyOptions());
        }
    }
}
