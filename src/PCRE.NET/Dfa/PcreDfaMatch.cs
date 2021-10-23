using System;
using System.Diagnostics.CodeAnalysis;

namespace PCRE.Dfa
{
    /// <summary>
    /// An output item of a DFA match.
    /// </summary>
    public sealed class PcreDfaMatch : IPcreGroup
    {
        private readonly string? _subject;
        private string? _value;

        internal static readonly PcreDfaMatch Empty = new(string.Empty, -1, -1);

        internal PcreDfaMatch(string subject, int startOffset, int endOffset)
        {
            Index = startOffset;
            EndIndex = endOffset;

            if (Length <= 0)
                _value = string.Empty;
            else
                _subject = subject;
        }

        /// <inheritdoc cref="PcreMatch.Index"/>
        public int Index { get; }

        /// <inheritdoc cref="PcreMatch.EndIndex"/>
        public int EndIndex { get; }

        /// <inheritdoc cref="PcreMatch.Length"/>
        public int Length => EndIndex > Index ? EndIndex - Index : 0;

        /// <inheritdoc cref="PcreMatch.Value"/>
        public string Value => _value ??= _subject!.Substring(Index, Length);

        /// <inheritdoc cref="PcreMatch.ValueSpan"/>
        public ReadOnlySpan<char> ValueSpan => Length <= 0 ? ReadOnlySpan<char>.Empty : _subject!.AsSpan(Index, Length);

        /// <inheritdoc cref="PcreMatch.Success"/>
        public bool Success => Index >= 0;

        /// <summary>
        /// Converts a match to its matched substring.
        /// </summary>
        [return: NotNullIfNotNull("group")]
        public static implicit operator string?(PcreDfaMatch? group)
            => group?.Value;

        /// <inheritdoc cref="PcreMatch.ToString"/>
        public override string ToString()
            => Value;
    }
}
