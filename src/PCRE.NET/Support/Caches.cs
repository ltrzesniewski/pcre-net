using System;
using PCRE.Wrapper;

namespace PCRE.Support
{
    internal static class Caches
    {
        private const int DefaultCacheSize = 15;

        internal static readonly PriorityCache<RegexKey, object> RegexCache = new PriorityCache<RegexKey, object>(DefaultCacheSize, CreateRegex);
        internal static readonly PriorityCache<string, Func<PcreMatch, string>> ReplacementCache = new PriorityCache<string, Func<PcreMatch, string>>(DefaultCacheSize, ReplacementPattern.Parse);

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
            using (var context = key.Settings.CreateCompileContext(key.Pattern))
            {
                return new InternalRegex(context);
            }
        }
    }
}
