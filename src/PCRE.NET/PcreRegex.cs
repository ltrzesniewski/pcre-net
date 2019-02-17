using System;
using System.Diagnostics.CodeAnalysis;
using PCRE.Dfa;
using PCRE.Internal;
using PCRE.Support;

namespace PCRE
{
    public sealed partial class PcreRegex : IInternalRegexWrapper
    {
        private PcrePatternInfo _info;
        private PcreDfaRegex _dfa;

        public PcrePatternInfo PatternInfo => _info ?? (_info = new PcrePatternInfo(this));

        internal RegexKey Key { get; }

        internal InternalRegex InternalRegex { get; }

        internal int CaptureCount => InternalRegex.CaptureCount;

        public static int CacheSize
        {
            get => Caches.CacheSize;
            set => Caches.CacheSize = value;
        }

        public PcreDfaRegex Dfa => _dfa ?? (_dfa = new PcreDfaRegex(this));

        [SuppressMessage("ReSharper", "IntroduceOptionalParameters.Global")]
        public PcreRegex(string pattern)
            : this(pattern, PcreOptions.None)
        {
        }

        public PcreRegex(string pattern, PcreOptions options)
            : this(pattern, new PcreRegexSettings(options))
        {
        }

        public PcreRegex(string pattern, PcreRegexSettings settings)
        {
            if (pattern == null)
                throw new ArgumentNullException(nameof(pattern));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            Key = new RegexKey(pattern, settings);
            InternalRegex = Caches.RegexCache.GetOrAdd(Key);
        }

        RegexKey IInternalRegexWrapper.Key => Key;

        InternalRegex IInternalRegexWrapper.InternalRegex => InternalRegex;

        public override string ToString() => Key.Pattern;
    }
}
