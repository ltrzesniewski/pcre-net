using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PCRE.Wrapper;

namespace PCRE.Dfa
{
    public sealed class PcreDfaMatchResult : IReadOnlyList<PcreDfaMatch>
    {
        private readonly object _result; // See remark about JIT in PcreRegex
        private readonly PcreDfaMatch[] _matches;

        internal PcreDfaMatchResult(MatchData result)
        {
            _result = result;

            if (result.RawResultCode > 0)
                _matches = new PcreDfaMatch[result.RawResultCode];
            else if (result.RawResultCode == 0)
                _matches = new PcreDfaMatch[result.OutputVectorLength];
            else
                _matches = PcreDfaMatch.EmptyMatches;
        }

        private MatchData InternalResult
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return (MatchData)_result; }
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
            var uindex = (uint)index;
            return new PcreDfaMatch(result.Subject, result.GetStartOffset(uindex), result.GetEndOffset(uindex));
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

        public PcreDfaMatch this[int index]
        {
            get { return GetMatch(index); }
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

        public override string ToString()
        {
            var match = LongestMatch;
            return match != null ? match.Value : String.Empty;
        }
    }
}
