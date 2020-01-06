using System;

namespace PCRE
{
    public ref struct PcreRefGroup
    {
        private readonly ReadOnlySpan<char> _subject;

        internal PcreRefGroup(ReadOnlySpan<char> subject, int startOffset, int endOffset)
        {
            _subject = subject;
            Index = startOffset;
            EndIndex = endOffset;
        }

        public int Index { get; }
        public int EndIndex { get; }

        public int Length => EndIndex > Index ? EndIndex - Index : 0;

        public ReadOnlySpan<char> Value => _subject.Slice(Index, Length);

        public bool Success => _subject != default;

        public static implicit operator string(PcreRefGroup group) => group.Value.ToString();

        public override string ToString() => Value.ToString();
    }
}
