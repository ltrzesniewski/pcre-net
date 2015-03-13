using System;

namespace PCRE.Dfa
{
    public class PcreDfaMatch : IPcreGroup
    {
        internal static readonly PcreDfaMatch[] EmptyMatches = new PcreDfaMatch[0];

        private readonly string _subject;
        private string _value;

        internal PcreDfaMatch(string subject, int startOffset, int endOffset)
        {
            Index = startOffset;
            EndIndex = endOffset;

            if (Length <= 0)
                _value = String.Empty;
            else
                _subject = subject;
        }

        public int Index { get; private set; }
        public int EndIndex { get; private set; }

        public int Length
        {
            get { return EndIndex > Index ? EndIndex - Index : 0; }
        }

        public string Value
        {
            get { return _value ?? (_value = _subject.Substring(Index, Length)); }
        }

        public bool Success
        {
            get { return Index >= 0; }
        }

        public static implicit operator string(PcreDfaMatch group)
        {
            return group != null ? group.Value : null;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
