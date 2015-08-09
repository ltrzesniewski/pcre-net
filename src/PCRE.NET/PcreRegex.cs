using System;
using System.Runtime.CompilerServices;
using PCRE.Dfa;
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
        private PcreDfaRegex _dfa;

        public PcrePatternInfo PaternInfo => _info ?? (_info = new PcrePatternInfo(this));

        internal RegexKey Key { get; }

        internal InternalRegex InternalRegex
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return (InternalRegex)_re; }
        }

        internal int CaptureCount => InternalRegex.CaptureCount;

        public static int CacheSize
        {
            get { return Caches.CacheSize; }
            set { Caches.CacheSize = value; }
        }

        public PcreDfaRegex Dfa => _dfa ?? (_dfa = new PcreDfaRegex(this));

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
            _re = Caches.RegexCache.GetOrAdd(Key);
        }

        RegexKey IInternalRegexWrapper.Key => Key;

        InternalRegex IInternalRegexWrapper.InternalRegex => InternalRegex;

        public override string ToString() => Key.Pattern;
    }
}
