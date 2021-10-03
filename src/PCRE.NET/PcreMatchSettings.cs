using System;
using PCRE.Internal;

namespace PCRE
{
    /// <summary>
    /// Advanced match settings.
    /// </summary>
    public sealed class PcreMatchSettings
    {
        internal static PcreMatchSettings Default { get; } = new();

        private uint? _matchLimit;
        private uint? _depthLimit;
        private uint? _heapLimit;

        /// <summary>
        /// Limit for the amount of backtracking that can take place.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The <see cref="MatchLimit"/> parameter provides a means of preventing PCRE2 from using up too many computing resources when processing patterns that are not going to match,
        /// but which have a very large number of possibilities in their search trees. The classic example is a pattern that uses nested unlimited repeats.
        /// </para>
        /// <para>
        /// There is an internal counter in <c>pcre2_match()</c> that is incremented each time round its main matching loop.
        /// If this value reaches the match limit, <c>pcre2_match()</c> returns the negative value <c>PCRE2_ERROR_MATCHLIMIT</c>.
        /// This has the effect of limiting the amount of backtracking that can take place.
        /// For patterns that are not anchored, the count restarts from zero for each position in the subject string.
        /// This limit also applies to <c>pcre2_dfa_match()</c>, though the counting is done in a different way.
        /// </para>
        /// <para>
        /// When <c>pcre2_match()</c> is called with a pattern that was successfully processed by <c>pcre2_jit_compile()</c>, the way in which matching is executed is entirely different.
        /// However, there is still the possibility of runaway matching that goes on for a very long time, and so the <see cref="MatchLimit"/> value is also used in this case (but in a different way)
        /// to limit how long the matching can continue.
        /// </para>
        /// <para>
        /// The default value for the limit can be set when PCRE2 is built; the default default is 10 million, which handles all but the most extreme cases.
        /// A value for the match limit may also be supplied by an item at the start of a pattern of the form <c>(*LIMIT_MATCH=ddd)</c>
        /// where <c>ddd</c> is a decimal number. However, such a setting is ignored unless <c>ddd</c> is less than the limit set by the caller of <c>pcre2_match()</c> or <c>pcre2_dfa_match()</c> or,
        /// if no such limit is set, less than the default.
        /// </para>
        /// </remarks>
        public uint MatchLimit
        {
            get => _matchLimit ?? PcreBuildInfo.MatchLimit;
            set => _matchLimit = value;
        }

        /// <summary>
        /// Depth limit of nested backtracking.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This parameter limits the depth of nested backtracking in <c>pcre2_match()</c>. Each time a nested backtracking point is passed, a new memory "frame" is used to remember the
        /// state of matching at that point. Thus, this parameter indirectly limits the amount of memory that is used in a match.
        /// However, because the size of each memory "frame" depends on the number of capturing parentheses, the actual memory limit varies from pattern to pattern.
        /// This limit was more useful in versions before 10.30, where function recursion was used for backtracking.
        /// </para>
        /// <para>
        /// The depth limit is not relevant, and is ignored, when matching is done using JIT compiled code. However, it is supported by <c>pcre2_dfa_match()</c>,
        /// which uses it to limit the depth of nested internal recursive function calls that implement atomic groups, lookaround assertions, and pattern recursions.
        /// This limits, indirectly, the amount of system stack that is used. It was more useful in versions before 10.32, when stack memory was used for local workspace vectors for recursive function calls.
        /// From version 10.32, only local variables are allocated on the stack and as each call uses only a few hundred bytes, even a small stack can support quite a lot of recursion.
        /// </para>
        /// <para>
        /// If the depth of internal recursive function calls is great enough, local workspace vectors are allocated on the heap from version 10.32 onwards, so the depth limit also indirectly
        /// limits the amount of heap memory that is used. A recursive pattern such as <c>/(.(?2))((?1)|)/</c>, when matched to a very long string using <c>pcre2_dfa_match()</c>,
        /// can use a great deal of memory. However, it is probably better to limit heap usage directly by calling <c>pcre2_set_heap_limit()</c>.
        /// </para>
        /// <para>
        /// The default value for the depth limit can be set when PCRE2 is built; if it is not, the default is set to the same value as the default for the match limit.
        /// If the limit is exceeded, <c>pcre2_match()</c> or <c>pcre2_dfa_match()</c> returns <c>PCRE2_ERROR_DEPTHLIMIT</c>.
        /// A value for the depth limit may also be supplied by an item at the start of a pattern of the form <c>(*LIMIT_DEPTH=ddd)</c>
        /// where <c>ddd</c> is a decimal number. However, such a setting is ignored unless <c>ddd</c> is less than the limit set by the caller of <c>pcre2_match()</c> or <c>pcre2_dfa_match()</c> or,
        /// if no such limit is set, less than the default.
        /// </para>
        /// </remarks>
        public uint DepthLimit
        {
            get => _depthLimit ?? PcreBuildInfo.DepthLimit;
            set => _depthLimit = value;
        }

        /// <summary>
        /// Limit of heap memory that can be allocated at matching time, in KB.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The <see cref="HeapLimit"/> parameter specifies, in units of kibibytes (1024 bytes), the maximum amount of heap memory that <c>pcre2_match()</c> may use to hold backtracking information
        /// when running an interpretive match. This limit also applies to <c>pcre2_dfa_match()</c>, which may use the heap when processing patterns with a lot of nested pattern recursion
        /// or lookarounds or atomic groups. This limit does not apply to matching with the JIT optimization, which has its own memory control arrangements (see the pcre2jit documentation for more details).
        /// If the limit is reached, the negative error code <c>PCRE2_ERROR_HEAPLIMIT</c> is returned. The default limit can be set when PCRE2 is built;
        /// if it is not, the default is set very large and is essentially "unlimited".
        /// </para>
        /// <para>
        /// A value for the heap limit may also be supplied by an item at the start of a pattern of the form <c>(*LIMIT_HEAP=ddd)</c>
        /// where <c>ddd</c> is a decimal number. However, such a setting is ignored unless <c>ddd</c> is less than the limit set by the caller of <c>pcre2_match()</c> or,
        /// if no such limit is set, less than the default.
        /// </para>
        /// <para>
        /// The <c>pcre2_match()</c> function starts out using a 20KiB vector on the system stack for recording backtracking points.
        /// The more nested backtracking points there are (that is, the deeper the search tree), the more memory is needed. Heap memory is used only if the initial vector is too small.
        /// If the heap limit is set to a value less than 21 (in particular, zero) no heap memory will be used.
        /// In this case, only patterns that do not have a lot of nested backtracking can be successfully processed.
        /// </para>
        /// <para>
        /// Similarly, for <c>pcre2_dfa_match()</c>, a vector on the system stack is used when processing pattern recursions, lookarounds, or atomic groups,
        /// and only if this is not big enough is heap memory used. In this case, too, setting a value of zero disables the use of the heap.
        /// </para>
        /// </remarks>
        public uint HeapLimit
        {
            get => _heapLimit ?? PcreBuildInfo.HeapLimit;
            set => _heapLimit = value;
        }

        /// <summary>
        /// Limits how far a match can start after the initial start offset in the subject string.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The <see cref="OffsetLimit"/> parameter limits how far an unanchored search can advance in the subject string.
        /// The default value is PCRE2_UNSET. The <c>pcre2_match()</c> and <c>pcre2_dfa_match()</c> functions return <c>PCRE2_ERROR_NOMATCH</c> if a match with a starting point before
        /// or at the given offset is not found. The <c>pcre2_substitute()</c> function makes no more substitutions.
        /// </para>
        /// <para>
        /// For example, if the pattern <c>/abc/</c> is matched against "123abc" with an offset limit less than 3, the result is <c>PCRE2_ERROR_NOMATCH</c>.
        /// A match can never be found if the startoffset argument of <c>pcre2_match(),</c> <c>pcre2_dfa_match()</c>, or <c>pcre2_substitute()</c> is greater than the offset limit set in the match context.
        /// </para>
        /// <para>
        /// When using this facility, you must set the <see cref="PcreOptions.UseOffsetLimit"/> option when calling <c>pcre2_compile()</c> so that when JIT is in use, different code can be compiled.
        /// If a match is started with a non-default match limit when <see cref="PcreOptions.UseOffsetLimit"/> is not set, an error is generated.
        /// </para>
        /// <para>
        /// The offset limit facility can be used to track progress when searching large subject strings or to limit the extent of global substitutions.
        /// See also the <see cref="PcreOptions.FirstLine"/> option, which requires a match to start before or at the first newline that follows the start of matching in the subject.
        /// If this is set with an offset limit, a match must occur in the first line and also within the offset limit. In other words, whichever limit comes first is used.
        /// </para>
        /// </remarks>
        public uint? OffsetLimit { get; set; }

        /// <summary>
        /// Assign a non-default stack for use by the JIT when matching a pattern.
        /// </summary>
        public PcreJitStack? JitStack { get; set; }

        internal void FillMatchInput(ref Native.match_input input)
        {
            input.match_limit = _matchLimit.GetValueOrDefault();
            input.depth_limit = _depthLimit.GetValueOrDefault();
            input.heap_limit = _heapLimit.GetValueOrDefault();
            input.offset_limit = OffsetLimit.GetValueOrDefault();
            input.jit_stack = JitStack?.GetStack() ?? IntPtr.Zero;
        }
    }
}
