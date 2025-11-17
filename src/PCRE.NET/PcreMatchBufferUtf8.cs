using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using PCRE.Internal;

namespace PCRE;

/// <summary>
/// A buffer that allows execution of regular expression matches without managed allocations.
/// </summary>
/// <remarks>
/// Not thread-safe and not reentrant.
/// </remarks>
public sealed unsafe class PcreMatchBufferUtf8 : IPcreMatchBuffer, IDisposable
{
    internal readonly InternalRegex8Bit Regex;
    private readonly int _outputVectorSize;
    private PcreJitStack? _jitStack; // GC reference

    internal IntPtr NativeBuffer;

    internal readonly nuint* OutputVector;
    internal readonly nuint[] CalloutOutputVector;

    InternalRegex IPcreMatchBuffer.Regex => Regex;
    IntPtr IPcreMatchBuffer.NativeBuffer => NativeBuffer;
    nuint[] IPcreMatchBuffer.CalloutOutputVector => CalloutOutputVector;

    internal PcreMatchBufferUtf8(InternalRegex8Bit regex, PcreMatchSettings settings)
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

        NativeBuffer = (IntPtr)default(Native8Bit).create_match_buffer(&info);
        if (NativeBuffer == IntPtr.Zero)
            throw new InvalidOperationException("Could not create match buffer");

        OutputVector = info.output_vector;

        GC.KeepAlive(this);
    }

    /// <inheritdoc />
    ~PcreMatchBufferUtf8()
        => FreeBuffer();

    /// <inheritdoc />
    public void Dispose()
    {
        FreeBuffer();
        GC.SuppressFinalize(this);
    }

    private void FreeBuffer()
    {
        GC.KeepAlive(_jitStack);
        _jitStack = null;

        var buffer = Interlocked.Exchange(ref NativeBuffer, IntPtr.Zero);
        if (buffer != IntPtr.Zero)
            default(Native8Bit).free_match_buffer((void*)buffer);
    }

    private Span<nuint> GetOutputVectorSpan()
        => new(OutputVector, _outputVectorSize);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="IsMatch"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject"]'/>
    [Pure]
    public bool IsMatch(ReadOnlySpan<byte> subject)
        => Match(subject, 0, PcreMatchOptions.None, null).Success;

    /// <include file='PcreRegex.xml' path='/doc/method[@name="IsMatch"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex"]/*'/>
    /// </remarks>
    [Pure]
    public bool IsMatch(ReadOnlySpan<byte> subject, int startIndex)
        => Match(subject, startIndex, PcreMatchOptions.None, null).Success;

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Match"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject"]'/>
    [Pure]
    public PcreRefMatchUtf8 Match(ReadOnlySpan<byte> subject)
        => Match(subject, 0, PcreMatchOptions.None, null);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Match"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="options"]'/>
    [Pure]
    public PcreRefMatchUtf8 Match(ReadOnlySpan<byte> subject, PcreMatchOptions options)
        => Match(subject, 0, options, null);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Match"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex"]/*'/>
    /// </remarks>
    [Pure]
    public PcreRefMatchUtf8 Match(ReadOnlySpan<byte> subject, int startIndex)
        => Match(subject, startIndex, PcreMatchOptions.None, null);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Match"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex" or @name="options"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex"]/*'/>
    /// </remarks>
    [Pure]
    public PcreRefMatchUtf8 Match(ReadOnlySpan<byte> subject, int startIndex, PcreMatchOptions options)
        => Match(subject, startIndex, options, null);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Match"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="onCallout"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="callout"]/*'/>
    /// </remarks>
    public PcreRefMatchUtf8 Match(ReadOnlySpan<byte> subject, PcreRefCalloutFuncUtf8? onCallout)
        => Match(subject, 0, PcreMatchOptions.None, onCallout);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Match"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="options" or @name="onCallout"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="callout"]/*'/>
    /// </remarks>
    public PcreRefMatchUtf8 Match(ReadOnlySpan<byte> subject, PcreMatchOptions options, PcreRefCalloutFuncUtf8? onCallout)
        => Match(subject, 0, options, onCallout);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Match"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex" or @name="onCallout"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex" or @name="callout"]/*'/>
    /// </remarks>
    public PcreRefMatchUtf8 Match(ReadOnlySpan<byte> subject, int startIndex, PcreRefCalloutFuncUtf8? onCallout)
        => Match(subject, startIndex, PcreMatchOptions.None, onCallout);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Match"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex" or @name="options" or @name="onCallout"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex" or @name="callout"]/*'/>
    /// </remarks>
    public PcreRefMatchUtf8 Match(ReadOnlySpan<byte> subject, int startIndex, PcreMatchOptions options, PcreRefCalloutFuncUtf8? onCallout)
    {
        if (unchecked((uint)startIndex > (uint)subject.Length))
            ThrowInvalidStartIndex();

        var match = new PcreRefMatchUtf8(this, GetOutputVectorSpan());
        match.FirstMatch(this, subject, startIndex, options, onCallout);
        return match;
    }

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Matches"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject"]'/>
    [Pure]
    public RefMatchEnumerable Matches(ReadOnlySpan<byte> subject)
        => Matches(subject, 0, PcreMatchOptions.None, null);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Matches"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex"]/*'/>
    /// </remarks>
    [Pure]
    public RefMatchEnumerable Matches(ReadOnlySpan<byte> subject, int startIndex)
        => Matches(subject, startIndex, PcreMatchOptions.None, null);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Matches"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex" or @name="onCallout"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex" or @name="callout"]/*'/>
    /// </remarks>
    [Pure]
    public RefMatchEnumerable Matches(ReadOnlySpan<byte> subject, int startIndex, PcreRefCalloutFuncUtf8? onCallout)
        => Matches(subject, startIndex, PcreMatchOptions.None, onCallout);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Matches"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex" or @name="options" or @name="onCallout"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex" or @name="callout"]/*'/>
    /// </remarks>
    [Pure]
    public RefMatchEnumerable Matches(ReadOnlySpan<byte> subject, int startIndex, PcreMatchOptions options, PcreRefCalloutFuncUtf8? onCallout)
    {
        if (unchecked((uint)startIndex > (uint)subject.Length))
            ThrowInvalidStartIndex();

        return new RefMatchEnumerable(this, subject, startIndex, options, onCallout);
    }

    /// <summary>
    /// Returns the regex pattern.
    /// </summary>
    public override string ToString()
        => Regex.PatternString;

    private static void ThrowInvalidStartIndex()
        => throw new ArgumentOutOfRangeException("Invalid start index.", default(Exception));

    /// <summary>
    /// An enumerable of matches.
    /// </summary>
    public readonly ref struct RefMatchEnumerable
    {
        private readonly ReadOnlySpan<byte> _subject;
        private readonly int _startIndex;
        private readonly PcreMatchOptions _options;
        private readonly PcreRefCalloutFuncUtf8? _callout;
        private readonly PcreMatchBufferUtf8 _buffer;

        internal RefMatchEnumerable(PcreMatchBufferUtf8 buffer,
                                    ReadOnlySpan<byte> subject,
                                    int startIndex,
                                    PcreMatchOptions options,
                                    PcreRefCalloutFuncUtf8? callout)
        {
            _buffer = buffer;
            _subject = subject;
            _startIndex = startIndex;
            _options = options;
            _callout = callout;
        }

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
        public RefMatchEnumerator GetEnumerator()
            => new(_buffer, _subject, _startIndex, _options, _callout);
    }

    /// <summary>
    /// An enumerator of matches.
    /// </summary>
    public ref struct RefMatchEnumerator
    {
        private readonly ReadOnlySpan<byte> _subject;
        private readonly int _startIndex;
        private readonly PcreMatchOptions _options;
        private readonly PcreRefCalloutFuncUtf8? _callout;
        private PcreMatchBufferUtf8? _buffer;
        private PcreRefMatchUtf8 _match;

        internal RefMatchEnumerator(PcreMatchBufferUtf8 buffer,
                                    ReadOnlySpan<byte> subject,
                                    int startIndex,
                                    PcreMatchOptions options,
                                    PcreRefCalloutFuncUtf8? callout)
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
        public readonly PcreRefMatchUtf8 Current => _match;

        /// <summary>
        /// Moves to the next match.
        /// </summary>
        public bool MoveNext()
        {
            if (_buffer == null)
                return false;

            if (!_match.IsInitialized)
            {
                _match = new PcreRefMatchUtf8(_buffer, _buffer.GetOutputVectorSpan());
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
