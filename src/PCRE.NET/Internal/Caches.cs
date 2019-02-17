using System;

namespace PCRE.Internal
{
    internal static class Caches
    {
        private const int DefaultCacheSize = 15;

        internal static readonly PriorityCache<RegexKey, InternalRegex> RegexCache = new PriorityCache<RegexKey, InternalRegex>(DefaultCacheSize, key => new InternalRegex(key.Pattern, key.Settings));
        internal static readonly PriorityCache<string, Func<PcreMatch, string>> ReplacementCache = new PriorityCache<string, Func<PcreMatch, string>>(DefaultCacheSize, ReplacementPattern.Parse);

        public static int CacheSize
        {
            get => RegexCache.CacheSize;
            set
            {
                RegexCache.CacheSize = value;
                ReplacementCache.CacheSize = value;
            }
        }
    }
}
