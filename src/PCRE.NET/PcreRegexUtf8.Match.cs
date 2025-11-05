using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using PCRE.Internal;

namespace PCRE;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "IntroduceOptionalParameters.Global")]
public partial class PcreRegexUtf8
{
    /// <include file='PcreRegex.xml' path='/doc/method[@name="IsMatch"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject"]'/>
    [Pure]
    public bool IsMatch(ReadOnlySpan<byte> subject)
        => IsMatch(subject, 0);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="IsMatch"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex"]/*'/>
    /// </remarks>
    [Pure]
    public bool IsMatch(ReadOnlySpan<byte> subject, int startIndex)
    {
        if (unchecked((uint)startIndex > (uint)subject.Length))
            ThrowInvalidStartIndex();

        var outputVector = InternalRegex.CanStackAllocOutputVector
            ? stackalloc nuint[InternalRegex.OutputVectorSize]
            : new nuint[InternalRegex.OutputVectorSize];

        var match = new PcreRefMatchUtf8(InternalRegex, outputVector);
        match.FirstMatch(subject, PcreMatchSettings.Default, startIndex, 0, null, null);

        return match.Success;
    }

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Match"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject"]'/>
    [Pure]
    public PcreRefMatchUtf8 Match(ReadOnlySpan<byte> subject)
        => Match(subject, 0, PcreMatchOptions.None, null, PcreMatchSettings.Default);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Match"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="options"]'/>
    [Pure]
    public PcreRefMatchUtf8 Match(ReadOnlySpan<byte> subject, PcreMatchOptions options)
        => Match(subject, 0, options, null, PcreMatchSettings.Default);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Match"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex"]/*'/>
    /// </remarks>
    [Pure]
    public PcreRefMatchUtf8 Match(ReadOnlySpan<byte> subject, int startIndex)
        => Match(subject, startIndex, PcreMatchOptions.None, null, PcreMatchSettings.Default);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Match"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex" or @name="options"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex"]/*'/>
    /// </remarks>
    [Pure]
    public PcreRefMatchUtf8 Match(ReadOnlySpan<byte> subject, int startIndex, PcreMatchOptions options)
        => Match(subject, startIndex, options, null, PcreMatchSettings.Default);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Match"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="onCallout"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="callout"]/*'/>
    /// </remarks>
    public PcreRefMatchUtf8 Match(ReadOnlySpan<byte> subject, PcreRefCalloutFunc? onCallout)
        => Match(subject, 0, PcreMatchOptions.None, onCallout, PcreMatchSettings.Default);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Match"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="options" or @name="onCallout"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="callout"]/*'/>
    /// </remarks>
    public PcreRefMatchUtf8 Match(ReadOnlySpan<byte> subject, PcreMatchOptions options, PcreRefCalloutFunc? onCallout)
        => Match(subject, 0, options, onCallout, PcreMatchSettings.Default);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Match"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex" or @name="onCallout"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex" or @name="callout"]/*'/>
    /// </remarks>
    public PcreRefMatchUtf8 Match(ReadOnlySpan<byte> subject, int startIndex, PcreRefCalloutFunc? onCallout)
        => Match(subject, startIndex, PcreMatchOptions.None, onCallout, PcreMatchSettings.Default);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Match"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex" or @name="options" or @name="onCallout"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex" or @name="callout"]/*'/>
    /// </remarks>
    public PcreRefMatchUtf8 Match(ReadOnlySpan<byte> subject, int startIndex, PcreMatchOptions options, PcreRefCalloutFunc? onCallout)
        => Match(subject, startIndex, options, onCallout, PcreMatchSettings.Default);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Match"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex" or @name="options" or @name="onCallout" or @name="settings"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex" or @name="callout"]/*'/>
    /// </remarks>
    public PcreRefMatchUtf8 Match(ReadOnlySpan<byte> subject, int startIndex, PcreMatchOptions options, PcreRefCalloutFunc? onCallout, PcreMatchSettings settings)
    {
        if (settings == null)
            throw new ArgumentNullException(nameof(settings));

        if (unchecked((uint)startIndex > (uint)subject.Length))
            ThrowInvalidStartIndex();

        var match = new PcreRefMatchUtf8(InternalRegex, Span<nuint>.Empty);
        match.FirstMatch(subject, settings, startIndex, options, onCallout, null);

        return match;
    }

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Matches"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject"]'/>
    [Pure]
    public RefMatchEnumerable Matches(ReadOnlySpan<byte> subject)
        => Matches(subject, 0, PcreMatchOptions.None, null, PcreMatchSettings.Default);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Matches"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex"]/*'/>
    /// </remarks>
    [Pure]
    public RefMatchEnumerable Matches(ReadOnlySpan<byte> subject, int startIndex)
        => Matches(subject, startIndex, PcreMatchOptions.None, null, PcreMatchSettings.Default);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Matches"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex" or @name="onCallout"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex" or @name="callout"]/*'/>
    /// </remarks>
    [Pure]
    public RefMatchEnumerable Matches(ReadOnlySpan<byte> subject, int startIndex, PcreRefCalloutFunc? onCallout)
        => Matches(subject, startIndex, PcreMatchOptions.None, onCallout, PcreMatchSettings.Default);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Matches"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex" or @name="options" or @name="onCallout" or @name="settings"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex" or @name="callout"]/*'/>
    /// </remarks>
    [Pure]
    public RefMatchEnumerable Matches(ReadOnlySpan<byte> subject, int startIndex, PcreMatchOptions options, PcreRefCalloutFunc? onCallout, PcreMatchSettings settings)
    {
        if (settings == null)
            throw new ArgumentNullException(nameof(settings));

        if (unchecked((uint)startIndex > (uint)subject.Length))
            ThrowInvalidStartIndex();

        return new RefMatchEnumerable(InternalRegex, subject, startIndex, options, onCallout, settings);
    }

    private static void ThrowInvalidStartIndex()
        => throw new ArgumentOutOfRangeException("Invalid start index.", default(Exception));

    /// <summary>
    /// An enumerable of matches against a <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    public readonly ref struct RefMatchEnumerable
    {
        private readonly ReadOnlySpan<byte> _subject;
        private readonly int _startIndex;
        private readonly PcreMatchOptions _options;
        private readonly PcreRefCalloutFunc? _callout;
        private readonly PcreMatchSettings _settings;
        private readonly InternalRegex8Bit _regex;

        internal RefMatchEnumerable(InternalRegex8Bit regex,
                                    ReadOnlySpan<byte> subject,
                                    int startIndex,
                                    PcreMatchOptions options,
                                    PcreRefCalloutFunc? callout,
                                    PcreMatchSettings settings)
        {
            _regex = regex;
            _subject = subject;
            _startIndex = startIndex;
            _options = options;
            _callout = callout;
            _settings = settings;
        }

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
        public RefMatchEnumerator GetEnumerator()
            => new(_regex, _subject, _startIndex, _options, _callout, _settings);

        /// <summary>
        /// Creates a <see cref="List{T}"/> out of the matches by applying a <paramref name="selector"/> method.
        /// </summary>
        /// <param name="selector">The selector that converts a match to a list item.</param>
        /// <typeparam name="T">The type of list items.</typeparam>
        [SuppressMessage("Microsoft.Design", "CA1002")]
        [SuppressMessage("Microsoft.Design", "CA1062")]
        public List<T> ToList<T>(PcreRefMatchUtf8.Func<T> selector)
        {
            var result = new List<T>();

            foreach (var item in this)
                result.Add(selector(item));

            return result;
        }
    }

    /// <summary>
    /// An enumerator of matches against a <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    public ref struct RefMatchEnumerator
    {
        private readonly ReadOnlySpan<byte> _subject;
        private readonly int _startIndex;
        private readonly PcreMatchOptions _options;
        private readonly PcreRefCalloutFunc? _callout;
        private readonly PcreMatchSettings _settings;
        private InternalRegex8Bit? _regex;
        private PcreRefMatchUtf8 _match;

        internal RefMatchEnumerator(InternalRegex8Bit regex,
                                    ReadOnlySpan<byte> subject,
                                    int startIndex,
                                    PcreMatchOptions options,
                                    PcreRefCalloutFunc? callout,
                                    PcreMatchSettings settings)
        {
            _regex = regex;
            _subject = subject;
            _startIndex = startIndex;
            _options = options;
            _callout = callout;
            _settings = settings;
            _match = default;
        }

        /// <summary>
        /// Gets the current match.
        /// </summary>
        public readonly PcreRefMatchUtf8 Current => _match;

        /// <summary>
        /// Moves to the next match.
        /// </summary>
        public bool MoveNext()
        {
            if (_regex == null)
                return false;

            if (!_match.IsInitialized)
            {
                _match = new PcreRefMatchUtf8(_regex, Span<nuint>.Empty);
                _match.FirstMatch(_subject, _settings, _startIndex, _options, _callout, null);
            }
            else
            {
                _match.NextMatch(_settings, _options, _callout, null);
            }

            if (_match.Success)
                return true;

            _regex = null;
            return false;
        }
    }
}
