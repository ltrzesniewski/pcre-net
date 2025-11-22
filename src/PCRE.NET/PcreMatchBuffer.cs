using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using PCRE.Internal;

namespace PCRE;

internal interface IPcreMatchBuffer
{
    InternalRegex Regex { get; }
    IntPtr NativeBuffer { get; }
    nuint[] CalloutOutputVector { get; }
}

/// <summary>
/// A buffer that allows execution of regular expression matches without managed allocations.
/// </summary>
/// <remarks>
/// Not thread-safe and not reentrant.
/// </remarks>
[ForwardTo8Bit]
public sealed unsafe class PcreMatchBuffer : IPcreMatchBuffer, IRegexHolder16Bit, IDisposable
{
    internal readonly InternalRegex16Bit Regex;
    private readonly int _outputVectorSize;
    private PcreJitStack? _jitStack; // GC reference

    internal IntPtr NativeBuffer;

    internal readonly nuint* OutputVector;
    internal readonly nuint[] CalloutOutputVector;

    InternalRegex IPcreMatchBuffer.Regex => Regex;
    IntPtr IPcreMatchBuffer.NativeBuffer => NativeBuffer;
    nuint[] IPcreMatchBuffer.CalloutOutputVector => CalloutOutputVector;
    InternalRegex16Bit IRegexHolder16Bit.Regex => Regex;

    [ForwardTo8Bit]
    internal PcreMatchBuffer(InternalRegex16Bit regex, PcreMatchSettings settings)
    {
        Regex = regex;
        _outputVectorSize = regex.OutputVectorSize;

        CalloutOutputVector = new nuint[_outputVectorSize];

        Regex.TryGetCalloutInfoByPatternPosition(0); // Make sure callout info is initialized

        var info = new Native.match_buffer_info
        {
            code = regex.Code
        };

        settings.FillMatchSettings(ref info.settings, out _jitStack);

        NativeBuffer = (IntPtr)default(Native16Bit).create_match_buffer(&info);
        if (NativeBuffer == IntPtr.Zero)
            throw new InvalidOperationException("Could not create match buffer");

        OutputVector = info.output_vector;

        GC.KeepAlive(this);
    }

    /// <inheritdoc />
    [ForwardTo8Bit]
    ~PcreMatchBuffer()
        => FreeBuffer();

    /// <inheritdoc />
    [ForwardTo8Bit]
    public void Dispose()
    {
        FreeBuffer();
        GC.SuppressFinalize(this);
    }

    [ForwardTo8Bit]
    private void FreeBuffer()
    {
        GC.KeepAlive(_jitStack);
        _jitStack = null;

        var buffer = Interlocked.Exchange(ref NativeBuffer, IntPtr.Zero);
        if (buffer != IntPtr.Zero)
            default(Native16Bit).free_match_buffer((void*)buffer);
    }

    [ForwardTo8Bit]
    private Span<nuint> GetOutputVectorSpan()
        => new(OutputVector, _outputVectorSize);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="IsMatch"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject"]'/>
    [Pure]
    [ForwardTo8Bit]
    public bool IsMatch(ReadOnlySpan<char> subject)
        => Match(subject, 0, PcreMatchOptions.None, null).Success;

    /// <include file='PcreRegex.xml' path='/doc/method[@name="IsMatch"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex"]/*'/>
    /// </remarks>
    [Pure]
    [ForwardTo8Bit]
    public bool IsMatch(ReadOnlySpan<char> subject, int startIndex)
        => Match(subject, startIndex, PcreMatchOptions.None, null).Success;

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Match"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject"]'/>
    [Pure]
    [ForwardTo8Bit]
    public PcreRefMatch Match(ReadOnlySpan<char> subject)
        => Match(subject, 0, PcreMatchOptions.None, null);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Match"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="options"]'/>
    [Pure]
    [ForwardTo8Bit]
    public PcreRefMatch Match(ReadOnlySpan<char> subject, PcreMatchOptions options)
        => Match(subject, 0, options, null);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Match"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex"]/*'/>
    /// </remarks>
    [Pure]
    [ForwardTo8Bit]
    public PcreRefMatch Match(ReadOnlySpan<char> subject, int startIndex)
        => Match(subject, startIndex, PcreMatchOptions.None, null);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Match"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex" or @name="options"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex"]/*'/>
    /// </remarks>
    [Pure]
    [ForwardTo8Bit]
    public PcreRefMatch Match(ReadOnlySpan<char> subject, int startIndex, PcreMatchOptions options)
        => Match(subject, startIndex, options, null);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Match"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="onCallout"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="callout"]/*'/>
    /// </remarks>
    [ForwardTo8Bit]
    public PcreRefMatch Match(ReadOnlySpan<char> subject, PcreRefCalloutFunc? onCallout)
        => Match(subject, 0, PcreMatchOptions.None, onCallout);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Match"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="options" or @name="onCallout"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="callout"]/*'/>
    /// </remarks>
    [ForwardTo8Bit]
    public PcreRefMatch Match(ReadOnlySpan<char> subject, PcreMatchOptions options, PcreRefCalloutFunc? onCallout)
        => Match(subject, 0, options, onCallout);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Match"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex" or @name="onCallout"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex" or @name="callout"]/*'/>
    /// </remarks>
    [ForwardTo8Bit]
    public PcreRefMatch Match(ReadOnlySpan<char> subject, int startIndex, PcreRefCalloutFunc? onCallout)
        => Match(subject, startIndex, PcreMatchOptions.None, onCallout);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Match"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex" or @name="options" or @name="onCallout"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex" or @name="callout"]/*'/>
    /// </remarks>
    [ForwardTo8Bit]
    public PcreRefMatch Match(ReadOnlySpan<char> subject, int startIndex, PcreMatchOptions options, PcreRefCalloutFunc? onCallout)
    {
        if (unchecked((uint)startIndex > (uint)subject.Length))
            ThrowInvalidStartIndex();

        var match = new PcreRefMatch(this, GetOutputVectorSpan());
        match.FirstMatch(this, subject, startIndex, options, onCallout);
        return match;
    }

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Matches"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject"]'/>
    [Pure]
    [ForwardTo8Bit]
    public RefMatchEnumerable Matches(ReadOnlySpan<char> subject)
        => Matches(subject, 0, PcreMatchOptions.None, null);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Matches"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex"]/*'/>
    /// </remarks>
    [Pure]
    [ForwardTo8Bit]
    public RefMatchEnumerable Matches(ReadOnlySpan<char> subject, int startIndex)
        => Matches(subject, startIndex, PcreMatchOptions.None, null);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Matches"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex" or @name="onCallout"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex" or @name="callout"]/*'/>
    /// </remarks>
    [Pure]
    [ForwardTo8Bit]
    public RefMatchEnumerable Matches(ReadOnlySpan<char> subject, int startIndex, PcreRefCalloutFunc? onCallout)
        => Matches(subject, startIndex, PcreMatchOptions.None, onCallout);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Matches"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex" or @name="options" or @name="onCallout"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex" or @name="callout"]/*'/>
    /// </remarks>
    [Pure]
    [ForwardTo8Bit]
    public RefMatchEnumerable Matches(ReadOnlySpan<char> subject, int startIndex, PcreMatchOptions options, PcreRefCalloutFunc? onCallout)
    {
        if (unchecked((uint)startIndex > (uint)subject.Length))
            ThrowInvalidStartIndex();

        return new RefMatchEnumerable(this, subject, startIndex, options, onCallout);
    }

    /// <summary>
    /// Returns the regex pattern.
    /// </summary>
    [ForwardTo8Bit]
    public override string ToString()
        => Regex.PatternString;

    [ForwardTo8Bit]
    private static void ThrowInvalidStartIndex()
        => throw new ArgumentOutOfRangeException("Invalid start index.", default(Exception));

    /// <summary>
    /// An enumerable of matches.
    /// </summary>
    [ForwardTo8Bit]
    public readonly ref struct RefMatchEnumerable
    {
        private readonly ReadOnlySpan<char> _subject;
        private readonly int _startIndex;
        private readonly PcreMatchOptions _options;
        private readonly PcreRefCalloutFunc? _callout;
        private readonly PcreMatchBuffer _buffer;

        internal RefMatchEnumerable(PcreMatchBuffer buffer,
                                    ReadOnlySpan<char> subject,
                                    int startIndex,
                                    PcreMatchOptions options,
                                    PcreRefCalloutFunc? callout)
        {
            _buffer = buffer;
            _subject = subject;
            _startIndex = startIndex;
            _options = options;
            _callout = callout;
        }

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
        [ForwardTo8Bit]
        public RefMatchEnumerator GetEnumerator()
            => new(_buffer, _subject, _startIndex, _options, _callout);
    }

    /// <summary>
    /// An enumerator of matches.
    /// </summary>
    [ForwardTo8Bit]
    public ref struct RefMatchEnumerator
    {
        private readonly ReadOnlySpan<char> _subject;
        private readonly int _startIndex;
        private readonly PcreMatchOptions _options;
        private readonly PcreRefCalloutFunc? _callout;
        private PcreMatchBuffer? _buffer;
        private PcreRefMatch _match;

        [ForwardTo8Bit]
        internal RefMatchEnumerator(PcreMatchBuffer buffer,
                                    ReadOnlySpan<char> subject,
                                    int startIndex,
                                    PcreMatchOptions options,
                                    PcreRefCalloutFunc? callout)
        {
            _buffer = buffer;
            _subject = subject;
            _startIndex = startIndex;
            _options = options;
            _callout = callout;
            _match = default;
        }

        /// <summary>
        /// Gets the current match.
        /// </summary>
        [ForwardTo8Bit]
        public readonly PcreRefMatch Current => _match;

        /// <summary>
        /// Moves to the next match.
        /// </summary>
        [ForwardTo8Bit]
        public bool MoveNext()
        {
            if (_buffer == null)
                return false;

            if (!_match.IsInitialized)
            {
                _match = new PcreRefMatch(_buffer, _buffer.GetOutputVectorSpan());
                _match.FirstMatch(_buffer, _subject, _startIndex, _options, _callout);
            }
            else
            {
                _match.NextMatch(_buffer, _options, _callout);
            }

            if (_match.Success)
                return true;

            _buffer = null;
            return false;
        }
    }
}
