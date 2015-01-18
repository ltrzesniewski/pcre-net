using System;
using System.Runtime.CompilerServices;
using PCRE.Support;
using PCRE.Wrapper;

namespace PCRE.Dfa
{
    public sealed partial class PcreDfaRegex : IInternalRegexWrapper
    {
        // ReSharper disable IntroduceOptionalParameters.Global, MemberCanBePrivate.Global, UnusedMember.Global

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

        public PcreDfaRegex(string pattern)
            : this(pattern, PcreOptions.None)
        {
        }

        public PcreDfaRegex(string pattern, PcreOptions options)
        {
            if (pattern == null)
                throw new ArgumentNullException("pattern");

            Key = new RegexKey(pattern, options);
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
