using System;
using System.Diagnostics.CodeAnalysis;

namespace PCRE
{
    /// <summary>
    /// The result of a capturing group.
    /// </summary>
    public sealed class PcreGroup : IPcreGroup
    {
        private readonly string? _subject;
        private string? _value;

        // The offsets match the truncated value of PCRE2_UNSET
        internal static readonly PcreGroup Empty = new(string.Empty, -1, -1);
        internal static readonly PcreGroup Undefined = new(string.Empty, -1, -1);

        internal PcreGroup(string subject, int startOffset, int endOffset)
        {
            Index = startOffset;
            EndIndex = endOffset;

            if (Length <= 0)
                _value = string.Empty;
            else
                _subject = subject;
        }

        /// <inheritdoc/>
        public int Index { get; }

        /// <inheritdoc/>
        public int EndIndex { get; }

        /// <inheritdoc/>
        public int Length => EndIndex > Index ? EndIndex - Index : 0;

        /// <inheritdoc/>
        public string Value => _value ??= _subject!.Substring(Index, Length);

        /// <inheritdoc/>
        public ReadOnlySpan<char> ValueSpan => Length <= 0 ? ReadOnlySpan<char>.Empty : _subject!.AsSpan(Index, Length);

        /// <inheritdoc/>
        public bool Success => Index >= 0;

        /// <summary>
        /// Indicates if the group exists in the pattern.
        /// </summary>
        public bool IsDefined => !ReferenceEquals(this, Undefined);

        /// <summary>
        /// Converts a group to its matched substring.
        /// </summary>
        [return: NotNullIfNotNull("group")]
        public static implicit operator string?(PcreGroup? group)
            => group?.Value;

        /// <summary>
        /// Returns the matched substring.
        /// </summary>
        public override string ToString()
            => Value;
    }
}
