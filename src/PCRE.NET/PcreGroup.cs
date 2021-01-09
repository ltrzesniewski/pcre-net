namespace PCRE
{
    public sealed class PcreGroup : IPcreGroup
    {
        private readonly string? _subject;
        private string? _value;

        // The offsets match the truncated value of PCRE2_UNSET
        internal static readonly PcreGroup Empty = new(string.Empty, -1, -1);

        internal PcreGroup(string subject, int startOffset, int endOffset)
        {
            Index = startOffset;
            EndIndex = endOffset;

            if (Length <= 0)
                _value = string.Empty;
            else
                _subject = subject;
        }

        public int Index { get; }
        public int EndIndex { get; }

        public int Length => EndIndex > Index ? EndIndex - Index : 0;

        public string Value => _value ??= _subject!.Substring(Index, Length);

        public bool Success => Index >= 0;

        public static implicit operator string(PcreGroup group) => group.Value;

        public override string ToString() => Value;
    }
}
