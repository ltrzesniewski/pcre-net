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
public sealed unsafe partial class PcreMatchBufferUtf8 : IPcreMatchBuffer, IRegexHolder8Bit, IDisposable
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
    InternalRegex8Bit IRegexHolder8Bit.Regex => Regex;

    /// <summary>
    /// An enumerable of matches.
    /// </summary>
    public readonly ref partial struct RefMatchEnumerable
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
    }

    /// <summary>
    /// An enumerator of matches.
    /// </summary>
    public ref partial struct RefMatchEnumerator
    {
        private readonly ReadOnlySpan<byte> _subject;
        private readonly int _startIndex;
        private readonly PcreMatchOptions _options;
        private readonly PcreRefCalloutFuncUtf8? _callout;
        private PcreMatchBufferUtf8? _buffer;
        private PcreRefMatchUtf8 _match;
    }
}
