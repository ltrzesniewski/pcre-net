using System;
using System.Diagnostics;

namespace PCRE
{
    [DebuggerTypeProxy(typeof(DebugProxy))]
    public readonly ref struct PcreRefGroup
    {
        private readonly ReadOnlySpan<char> _subject;

        public delegate T Func<out T>(PcreRefGroup group);

        internal PcreRefGroup(ReadOnlySpan<char> subject, int startOffset, int endOffset)
        {
            _subject = subject;
            Index = startOffset;
            EndIndex = endOffset;
        }

        public int Index { get; }
        public int EndIndex { get; }

        public int Length => EndIndex > Index ? EndIndex - Index : 0;

        public ReadOnlySpan<char> Value => Index < 0 ? default : _subject.Slice(Index, Length);

        public bool Success => _subject != default && Index >= 0;

        public static implicit operator string(PcreRefGroup group) => group.Value.ToString();

        public override string ToString() => Value.ToString();

        internal class DebugProxy
        {
            public bool Success { get; }
            public string Value { get; }

            public DebugProxy(PcreRefGroup group)
            {
                Success = group.Success;
                Value = Success ? group.Value.ToString() : null;
            }

            public override string ToString() => Value ?? "<no match>";
        }
    }
}
