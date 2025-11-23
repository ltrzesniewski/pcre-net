using System;
using System.Diagnostics.CodeAnalysis;
using PCRE.Internal;

namespace PCRE;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "IntroduceOptionalParameters.Global")]
public partial class PcreRegexUtf8
{
    /// <summary>
    /// An enumerable of matches against a <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    public readonly ref partial struct RefMatchEnumerable
    {
        private readonly ReadOnlySpan<byte> _subject;
        private readonly int _startIndex;
        private readonly PcreMatchOptions _options;
        private readonly PcreRefCalloutFuncUtf8? _callout;
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
        private readonly PcreRefCalloutFuncUtf8? _callout;
        private readonly PcreMatchSettings _settings;
        private InternalRegex8Bit? _regex;
        private PcreRefMatchUtf8 _match;
    }
}
