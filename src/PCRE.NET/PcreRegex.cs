using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using PCRE.Dfa;
using PCRE.Internal;

namespace PCRE
{
    /// <summary>
    /// A PCRE regular expression.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "IntroduceOptionalParameters.Global")]
    public sealed partial class PcreRegex
    {
        private PcrePatternInfo? _info;
        private PcreDfaRegex? _dfa;

        internal InternalRegex InternalRegex { get; }

        /// <summary>
        /// Returns information about the pattern.
        /// </summary>
        public PcrePatternInfo PatternInfo => _info ??= new PcrePatternInfo(InternalRegex);

        /// <summary>
        /// The size of the internal caches.
        /// </summary>
        /// <remarks>
        /// The cache is used for the following items:
        /// <list type="bullet">
        /// <item>Regex patterns: each instantiation of <see cref="PcreRegex"/> or usage of the static matching methods tries to find the pattern in the cache before compiling it.</item>
        /// <item>Replacement strings: recently used replacement strings are not parsed if available in the cache.</item>
        /// </list>
        /// Set this to 0 to disable the cache.
        /// </remarks>
        public static int CacheSize
        {
            get => Caches.CacheSize;
            set => Caches.CacheSize = value;
        }

        /// <summary>
        /// Gives access to the DFA (deterministic finite automaton) matching API.
        /// </summary>
        public PcreDfaRegex Dfa => _dfa ??= new PcreDfaRegex(InternalRegex);

        /// <summary>
        /// Creates a PCRE regex.
        /// </summary>
        /// <param name="pattern">The regular expression pattern.</param>
        [SuppressMessage("ReSharper", "IntroduceOptionalParameters.Global")]
        public PcreRegex(string pattern)
            : this(pattern, PcreOptions.None)
        {
        }

        /// <summary>
        /// Creates a PCRE regex.
        /// </summary>
        /// <param name="pattern">The regular expression pattern.</param>
        /// <param name="options">Pattern options.</param>
        public PcreRegex(string pattern, PcreOptions options)
            : this(pattern, new PcreRegexSettings(options))
        {
        }

        /// <summary>
        /// Creates a PCRE regex.
        /// </summary>
        /// <param name="pattern">The regular expression pattern.</param>
        /// <param name="settings">Additional advanced settings.</param>
        public PcreRegex(string pattern, PcreRegexSettings settings)
        {
            if (pattern == null)
                throw new ArgumentNullException(nameof(pattern));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            InternalRegex = Caches.RegexCache.GetOrAdd(new RegexKey(pattern, settings));
        }

        /// <summary>
        /// Creates a buffer for zero-allocation matching.
        /// </summary>
        /// <remarks>
        /// The resulting <see cref="PcreMatchBuffer"/> can be used to perform match operations without allocating any managed memory,
        /// therefore not inducing any GC pressure. Note that the buffer is not thread-safe and not reentrant.
        /// </remarks>
        [Pure]
        public PcreMatchBuffer CreateMatchBuffer()
            => new(InternalRegex, PcreMatchSettings.Default);

        /// <inheritdoc cref="CreateMatchBuffer()"/>
        /// <param name="settings">Additional settings.</param>
        [Pure]
        public PcreMatchBuffer CreateMatchBuffer(PcreMatchSettings settings)
            => new(InternalRegex, settings ?? throw new ArgumentNullException(nameof(settings)));

        /// <summary>
        /// Returns the regex pattern.
        /// </summary>
        public override string ToString()
            => InternalRegex.Pattern;
    }
}
