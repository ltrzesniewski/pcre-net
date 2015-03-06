using System;
using System.Runtime.CompilerServices;
using PCRE.Support;
using PCRE.Wrapper;

namespace PCRE
{
    public sealed partial class PcreRegex : IInternalRegexWrapper
    {
        // ReSharper disable IntroduceOptionalParameters.Global, MemberCanBePrivate.Global, UnusedMember.Global

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
            get { return Caches.CacheSize; }
            set { Caches.CacheSize = value; }
        }

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
                throw new ArgumentNullException("pattern");
            if (settings == null)
                throw new ArgumentNullException("settings");

            Key = new RegexKey(pattern, settings);
            _re = Caches.RegexCache.GetOrAdd(Key);
        }

        RegexKey IInternalRegexWrapper.Key
        {
            get { return Key; }
        }

        InternalRegex IInternalRegexWrapper.InternalRegex
        {
            get { return InternalRegex; }
        }
    }
}
