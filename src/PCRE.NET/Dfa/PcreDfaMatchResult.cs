using System;
using System.Collections;
using System.Collections.Generic;
using PCRE.Internal;

namespace PCRE.Dfa
{
    public sealed class PcreDfaMatchResult : IReadOnlyList<PcreDfaMatch>
    {
        private readonly uint[] _oVector;
        private readonly int _resultCode;
        private readonly PcreDfaMatch?[] _matches;

        internal string Subject { get; }

        internal PcreDfaMatchResult(string subject, ref Native.match_result result, uint[] oVector)
        {
            // Real match

            Subject = subject;
            _oVector = oVector;

            _resultCode = result.result_code;

            if (_resultCode > 0)
                _matches = new PcreDfaMatch[_resultCode];
            else if (_resultCode == 0)
                _matches = new PcreDfaMatch[_oVector.Length / 2];
            else
                _matches = Array.Empty<PcreDfaMatch>();
        }

        private PcreDfaMatch? GetMatch(int index)
        {
            if (index < 0 || index >= Count)
                return null;

            var match = _matches[index];
            if (match == null)
                _matches[index] = match = CreateMatch(index);

            return match;
        }

        private PcreDfaMatch? CreateMatch(int index)
        {
            index *= 2;
            if (index >= _oVector.Length)
                return null;

            var startOffset = (int)_oVector[index];
            var endOffset = (int)_oVector[index + 1];

            return new PcreDfaMatch(Subject, startOffset, endOffset);
        }

        private IEnumerable<PcreDfaMatch> GetMatches()
        {
            for (var i = 0; i < Count; ++i)
                yield return GetMatch(i)!;
        }

        public IEnumerator<PcreDfaMatch> GetEnumerator() => GetMatches().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public PcreDfaMatch? this[int index] => GetMatch(index);
        PcreDfaMatch IReadOnlyList<PcreDfaMatch>.this[int index] => this[index]!;

        public int Count => _matches.Length;

        public bool Success => _resultCode >= 0;

        public int Index => LongestMatch?.Index ?? -1;

        public PcreDfaMatch? LongestMatch => GetMatch(0);

        public PcreDfaMatch? ShortestMatch => GetMatch(Count - 1);

        public override string ToString()
        {
            var match = LongestMatch;
            return match != null ? match.Value : string.Empty;
        }
    }
}
