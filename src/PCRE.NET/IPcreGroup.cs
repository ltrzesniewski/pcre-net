using System;

namespace PCRE
{
    /// <summary>
    /// The result of a capturing group.
    /// </summary>
    public interface IPcreGroup
    {
        /// <summary>
        /// The start index of the group.
        /// </summary>
        int Index { get; }

        /// <summary>
        /// The end index of the group.
        /// </summary>
        int EndIndex { get; }

        /// <summary>
        /// The length of the group.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// The value of the group.
        /// </summary>
        string Value { get; }

        /// <summary>
        /// The value of the group, as a <see cref="ReadOnlySpan{T}"/>.
        /// </summary>
        ReadOnlySpan<char> ValueSpan { get; }

        /// <summary>
        /// Indicates whether the group matched.
        /// </summary>
        bool Success { get; }
    }
}
