using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PCRE.Wrapper;

namespace PCRE.Dfa
{
    public class PcreDfaMatchSet : IReadOnlyCollection<PcreDfaMatch>
    {
        private readonly object _result; // See remark about JIT in PcreRegex
        private readonly PcreDfaMatch[] _matches;

        internal PcreDfaMatchSet(MatchResult result)
        {
            _result = result;
            _matches = result.ResultCount > 0 ? new PcreDfaMatch[result.ResultCount] : PcreDfaMatch.EmptyMatches;
        }

        private MatchResult InternalResult
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return (MatchResult)_result; }
        }

        private PcreDfaMatch GetMatch(int index)
        {
            if (index < 0 || index >= Count)
                return null;

            var match = _matches[index];
            if (match == null)
                _matches[index] = match = CreateMatch(index);

            return match;
        }

        private PcreDfaMatch CreateMatch(int index)
        {
            var result = InternalResult;
            return new PcreDfaMatch(result.Subject, result.GetStartOffset(index), result.GetEndOffset(index));
        }

        private IEnumerable<PcreDfaMatch> GetMatches()
        {
            for (var i = 0; i < Count; ++i)
                yield return GetMatch(i);
        }

        public IEnumerator<PcreDfaMatch> GetEnumerator()
        {
            return GetMatches().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count
        {
            get { return _matches.Length; }
        }

        public bool Success
        {
            get { return InternalResult.ResultCode == MatchResultCode.Success; }
        }

        public int Index
        {
            // All matches share the same index
            get
            {
                var match = LongestMatch;
                return match != null ? match.Index : -1;
            }
        }

        public PcreDfaMatch LongestMatch
        {
            get { return GetMatch(0); }
        }

        public PcreDfaMatch ShortestMatch
        {
            get { return GetMatch(Count - 1); }
        }
    }
}
