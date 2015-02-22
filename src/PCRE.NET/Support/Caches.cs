using System;
using PCRE.Wrapper;

namespace PCRE.Support
{
    internal static class Caches
    {
        public static readonly PriorityCache<RegexKey, object> RegexCache = new PriorityCache<RegexKey, object>(15, CreateRegex);
        public static readonly PriorityCache<string, Func<PcreMatch, string>> ReplacementCache = new PriorityCache<string, Func<PcreMatch, string>>(15, ReplacementPattern.Parse);

        public static int CacheSize
        {
            get { return RegexCache.CacheSize; }
            set
            {
                RegexCache.CacheSize = value;
                ReplacementCache.CacheSize = value;
            }
        }

        private static object CreateRegex(RegexKey key)
        {
            return new InternalRegex(key.Pattern, key.Options.ToPatternOptions(), key.Options.ToJitCompileOptions());
        }
    }
}
