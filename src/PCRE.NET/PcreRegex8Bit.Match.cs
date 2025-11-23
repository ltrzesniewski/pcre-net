using System;
using System.Diagnostics.CodeAnalysis;
using PCRE.Internal;

namespace PCRE;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "IntroduceOptionalParameters.Global")]
public partial class PcreRegex8Bit
{
    /// <summary>
    /// An enumerable of matches against a <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    public readonly ref partial struct RefMatchEnumerable
    {
        private readonly ReadOnlySpan<byte> _subject;
        private readonly int _startIndex;
        private readonly PcreMatchOptions _options;
        private readonly PcreRefCalloutFunc8Bit? _callout;
        private readonly PcreMatchSettings _settings;
        private readonly InternalRegex8Bit _regex;
    }

    /// <summary>
    /// An enumerator of matches against a <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    public ref partial struct RefMatchEnumerator
    {
        private readonly ReadOnlySpan<byte> _subject;
        private readonly int _startIndex;
        private readonly PcreMatchOptions _options;
        private readonly PcreRefCalloutFunc8Bit? _callout;
        private readonly PcreMatchSettings _settings;
        private InternalRegex8Bit? _regex;
        private PcreRefMatch8Bit _match;
    }
}
