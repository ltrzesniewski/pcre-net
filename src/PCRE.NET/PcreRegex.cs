using System;
using System.Diagnostics.CodeAnalysis;
using PCRE.Dfa;
using PCRE.Internal;

namespace PCRE
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "IntroduceOptionalParameters.Global")]
    public sealed partial class PcreRegex
    {
        private PcrePatternInfo? _info;
        private PcreDfaRegex? _dfa;

        public PcrePatternInfo PatternInfo => _info ??= new PcrePatternInfo(InternalRegex);

        internal InternalRegex InternalRegex { get; }

        internal int CaptureCount => InternalRegex.CaptureCount;

        public static int CacheSize
        {
            get => Caches.CacheSize;
            set => Caches.CacheSize = value;
        }

        public PcreDfaRegex Dfa => _dfa ??= new PcreDfaRegex(InternalRegex);

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

            InternalRegex = Caches.RegexCache.GetOrAdd(new RegexKey(pattern, settings));
        }

        public PcreMatchBuffer CreateMatchBuffer()
            => new(InternalRegex);

        public override string ToString()
            => InternalRegex.Pattern;
    }
}
