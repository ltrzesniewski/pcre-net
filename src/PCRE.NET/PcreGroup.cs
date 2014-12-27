using System;

namespace PCRE
{
    public sealed class PcreGroup : IPcreGroup
    {
        private readonly string _subject;
        private string _value;

        internal static readonly PcreGroup Empty = new PcreGroup(String.Empty, -1, -1);

        internal PcreGroup(string subject, int startOffset, int endOffset)
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

        public bool IsMatch
        {
            get { return Index >= 0; }
        }

        public static implicit operator string(PcreGroup group)
        {
            return group != null ? group.Value : null;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
