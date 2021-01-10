using System;
using System.Diagnostics;

namespace PCRE
{
    [DebuggerTypeProxy(typeof(DebugProxy))]
    public readonly ref struct PcreRefGroup
    {
        private const int _emptyFlag = -1;
        private const int _undefinedFlag = -2;

        private readonly ReadOnlySpan<char> _subject;
        private readonly int _indexOrFlag;
        private readonly int _endIndexOrFlag;

        public delegate T Func<out T>(PcreRefGroup group);

        internal static PcreRefGroup Empty => new(_emptyFlag);
        internal static PcreRefGroup Undefined => new(_undefinedFlag);

        internal PcreRefGroup(ReadOnlySpan<char> subject, int startOffset, int endOffset)
        {
            _subject = subject;
            _indexOrFlag = startOffset;
            _endIndexOrFlag = endOffset;
        }

        private PcreRefGroup(int flag)
        {
            _subject = default;
            _indexOrFlag = _endIndexOrFlag = flag;
        }

        public int Index => _indexOrFlag >= _emptyFlag ? _indexOrFlag : _emptyFlag;
        public int EndIndex => _endIndexOrFlag >= _emptyFlag ? _endIndexOrFlag : _emptyFlag;

        public int Length => _endIndexOrFlag > _indexOrFlag ? _endIndexOrFlag - _indexOrFlag : 0;

        public ReadOnlySpan<char> Value => _indexOrFlag < 0 ? default : _subject.Slice(_indexOrFlag, Length);

        public bool Success => _indexOrFlag >= 0;
        public bool IsDefined => _indexOrFlag != _undefinedFlag;

        public static implicit operator string(PcreRefGroup group) => group.Value.ToString();

        public override string ToString() => Value.ToString();

        internal class DebugProxy
        {
            public bool Success { get; }
            public string? Value { get; }

            public DebugProxy(PcreRefGroup group)
            {
                Success = group.Success;
                Value = Success ? group.Value.ToString() : null;
            }

            public override string ToString() => Value ?? "<no match>";
        }
    }
}
