using System;

namespace PCRE
{
    public sealed class PcreGroup
    {
        private string _subject;
        private string _value;

        internal static readonly PcreGroup Empty = new PcreGroup(String.Empty, -1, -1);

        internal PcreGroup(string subject, int startOffset, int endOffset)
        {
            Index = startOffset;
            Length = endOffset - startOffset;

            if (Length == 0)
                _value = String.Empty;
            else
                _subject = subject;
        }

        public int Index { get; private set; }
        public int Length { get; private set; }

        public string Value
        {
            get
            {
                if (_value == null)
                {
                    _value = _subject.Substring(Index, Length);
                    _subject = null;
                }

                return _value;
            }
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
