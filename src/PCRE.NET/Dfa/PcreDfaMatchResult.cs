using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return (MatchData)_result; }
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

        public IEnumerator<PcreDfaMatch> GetEnumerator() => GetMatches().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public PcreDfaMatch this[int index] => GetMatch(index);

        public int Count => _matches.Length;

        public bool Success => InternalResult.ResultCode == MatchResultCode.Success;

        public int Index => LongestMatch?.Index ?? -1;

        public PcreDfaMatch LongestMatch => GetMatch(0);

        public PcreDfaMatch ShortestMatch => GetMatch(Count - 1);

        public override string ToString()
        {
            var match = LongestMatch;
            return match != null ? match.Value : string.Empty;
        }
    }
}
