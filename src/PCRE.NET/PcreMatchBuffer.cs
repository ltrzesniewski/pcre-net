using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using PCRE.Internal;

namespace PCRE
{
    /// <summary>
    /// A buffer that allows execution of regular expression matches without managed allocations.
    /// </summary>
    /// <remarks>
    /// Not thread-safe and not reentrant.
    /// </remarks>
    public sealed unsafe class PcreMatchBuffer : IDisposable
    {
        internal readonly InternalRegex Regex;
        internal IntPtr NativeBuffer;

        internal readonly uint[] OutputVector;
        internal readonly uint[] CalloutOutputVector;

        internal PcreMatchBuffer(InternalRegex regex, PcreMatchSettings settings)
        {
            Regex = regex;

            OutputVector = new uint[regex.OutputVectorSize];
            CalloutOutputVector = new uint[regex.OutputVectorSize];

            Regex.TryGetCalloutInfoByPatternPosition(0); // Make sure callout info is initialized

            var nativeSettings = default(Native.match_settings);
            settings.FillMatchSettings(ref nativeSettings);

            NativeBuffer = Native.create_match_buffer(regex.Code, &nativeSettings);
            if (NativeBuffer == IntPtr.Zero)
                throw new InvalidOperationException("Could not create match buffer");
        }

        /// <inheritdoc />
        ~PcreMatchBuffer()
        {
            FreeBuffer();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            FreeBuffer();
            GC.SuppressFinalize(this);
        }

        private void FreeBuffer()
        {
            var buffer = Interlocked.Exchange(ref NativeBuffer, IntPtr.Zero);
            if (buffer != IntPtr.Zero)
                Native.free_match_buffer(buffer);
        }

        /// <include file='PcreRegex.xml' path='/doc/method[@name="IsMatch"]/*'/>
        /// <include file='PcreRegex.xml' path='/doc/param[@name="subject"]'/>
        [Pure]
        public bool IsMatch(ReadOnlySpan<char> subject)
            => Match(subject, 0, PcreMatchOptions.None, null).Success;

        /// <include file='PcreRegex.xml' path='/doc/method[@name="IsMatch"]/*'/>
        /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex"]'/>
        /// <remarks>
        /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex"]/*'/>
        /// </remarks>
        [Pure]
        public bool IsMatch(ReadOnlySpan<char> subject, int startIndex)
            => Match(subject, startIndex, PcreMatchOptions.None, null).Success;

        /// <include file='PcreRegex.xml' path='/doc/method[@name="Match"]/*'/>
        /// <include file='PcreRegex.xml' path='/doc/param[@name="subject"]'/>
        [Pure]
        public PcreRefMatch Match(ReadOnlySpan<char> subject)
            => Match(subject, 0, PcreMatchOptions.None, null);

        /// <include file='PcreRegex.xml' path='/doc/method[@name="Match"]/*'/>
        /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="options"]'/>
        [Pure]
        public PcreRefMatch Match(ReadOnlySpan<char> subject, PcreMatchOptions options)
            => Match(subject, 0, options, null);

        /// <include file='PcreRegex.xml' path='/doc/method[@name="Match"]/*'/>
        /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex"]'/>
        /// <remarks>
        /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex"]/*'/>
        /// </remarks>
        [Pure]
        public PcreRefMatch Match(ReadOnlySpan<char> subject, int startIndex)
            => Match(subject, startIndex, PcreMatchOptions.None, null);

        /// <include file='PcreRegex.xml' path='/doc/method[@name="Match"]/*'/>
        /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex" or @name="options"]'/>
        /// <remarks>
        /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex"]/*'/>
        /// </remarks>
        [Pure]
        public PcreRefMatch Match(ReadOnlySpan<char> subject, int startIndex, PcreMatchOptions options)
            => Match(subject, startIndex, options, null);

        /// <include file='PcreRegex.xml' path='/doc/method[@name="Match"]/*'/>
        /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="onCallout"]'/>
        /// <remarks>
        /// <include file='PcreRegex.xml' path='/doc/remarks[@name="callout"]/*'/>
        /// </remarks>
        public PcreRefMatch Match(ReadOnlySpan<char> subject, PcreRefCalloutFunc? onCallout)
            => Match(subject, 0, PcreMatchOptions.None, onCallout);

        /// <include file='PcreRegex.xml' path='/doc/method[@name="Match"]/*'/>
        /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="options" or @name="onCallout"]'/>
        /// <remarks>
        /// <include file='PcreRegex.xml' path='/doc/remarks[@name="callout"]/*'/>
        /// </remarks>
        public PcreRefMatch Match(ReadOnlySpan<char> subject, PcreMatchOptions options, PcreRefCalloutFunc? onCallout)
            => Match(subject, 0, options, onCallout);

        /// <include file='PcreRegex.xml' path='/doc/method[@name="Match"]/*'/>
        /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex" or @name="onCallout"]'/>
        /// <remarks>
        /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex" or @name="callout"]/*'/>
        /// </remarks>
        public PcreRefMatch Match(ReadOnlySpan<char> subject, int startIndex, PcreRefCalloutFunc? onCallout)
            => Match(subject, startIndex, PcreMatchOptions.None, onCallout);

        /// <include file='PcreRegex.xml' path='/doc/method[@name="Match"]/*'/>
        /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex" or @name="options" or @name="onCallout"]'/>
        /// <remarks>
        /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex" or @name="callout"]/*'/>
        /// </remarks>
        public PcreRefMatch Match(ReadOnlySpan<char> subject, int startIndex, PcreMatchOptions options, PcreRefCalloutFunc? onCallout)
        {
            if (unchecked((uint)startIndex > (uint)subject.Length))
                ThrowInvalidStartIndex();

            var match = Regex.CreateRefMatch(OutputVector);
            match.FirstMatch(subject, this, startIndex, options, onCallout, CalloutOutputVector);

            return match;
        }

        /// <include file='PcreRegex.xml' path='/doc/method[@name="Matches"]/*'/>
        /// <include file='PcreRegex.xml' path='/doc/param[@name="subject"]'/>
        [Pure]
        public RefMatchEnumerable Matches(ReadOnlySpan<char> subject)
            => Matches(subject, 0, PcreMatchOptions.None, null);

        /// <include file='PcreRegex.xml' path='/doc/method[@name="Matches"]/*'/>
        /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex"]'/>
        /// <remarks>
        /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex"]/*'/>
        /// </remarks>
        [Pure]
        public RefMatchEnumerable Matches(ReadOnlySpan<char> subject, int startIndex)
            => Matches(subject, startIndex, PcreMatchOptions.None, null);

        /// <include file='PcreRegex.xml' path='/doc/method[@name="Matches"]/*'/>
        /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex" or @name="onCallout"]'/>
        /// <remarks>
        /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex" or @name="callout"]/*'/>
        /// </remarks>
        [Pure]
        public RefMatchEnumerable Matches(ReadOnlySpan<char> subject, int startIndex, PcreRefCalloutFunc? onCallout)
            => Matches(subject, startIndex, PcreMatchOptions.None, onCallout);

        /// <include file='PcreRegex.xml' path='/doc/method[@name="Matches"]/*'/>
        /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="startIndex" or @name="options" or @name="onCallout"]'/>
        /// <remarks>
        /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex" or @name="callout"]/*'/>
        /// </remarks>
        [Pure]
        public RefMatchEnumerable Matches(ReadOnlySpan<char> subject, int startIndex, PcreMatchOptions options, PcreRefCalloutFunc? onCallout)
        {
            if (unchecked((uint)startIndex > (uint)subject.Length))
                ThrowInvalidStartIndex();

            return new RefMatchEnumerable(this, subject, startIndex, options, onCallout);
        }

        /// <summary>
        /// Returns the regex pattern.
        /// </summary>
        public override string ToString()
            => Regex.Pattern;

        private static void ThrowInvalidStartIndex()
            => throw new ArgumentOutOfRangeException("Invalid start index.", default(Exception));

        /// <summary>
        /// An enumerable of matches.
        /// </summary>
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
            public RefMatchEnumerator GetEnumerator()
                => new(_buffer, _subject, _startIndex, _options, _callout);
        }

        /// <summary>
        /// An enumerator of matches.
        /// </summary>
        public ref struct RefMatchEnumerator
        {
            private readonly ReadOnlySpan<char> _subject;
            private readonly int _startIndex;
            private readonly PcreMatchOptions _options;
            private readonly PcreRefCalloutFunc? _callout;
            private PcreMatchBuffer? _buffer;
            private PcreRefMatch _match;

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
            public readonly PcreRefMatch Current => _match;

            /// <summary>
            /// Moves to the next match.
            /// </summary>
            public bool MoveNext()
            {
                if (_buffer == null)
                    return false;

                if (!_match.IsInitialized)
                {
                    _match = _buffer.Regex.CreateRefMatch(_buffer.OutputVector);
                    _match.FirstMatch(_subject, _buffer, _startIndex, _options, _callout, _buffer.CalloutOutputVector);
                }
                else
                {
                    _match.NextMatch(_buffer, _options, _callout, _buffer.CalloutOutputVector);
                }

                if (_match.Success)
                    return true;

                _buffer = null;
                return false;
            }
        }
    }
}
