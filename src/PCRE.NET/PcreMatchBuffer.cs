using System;
using System.Diagnostics.Contracts;
using PCRE.Internal;

namespace PCRE
{
    public class PcreMatchBuffer
    {
        private readonly InternalRegex _regex;
        private readonly PcreMatchSettings _settings;

        internal readonly uint[] OutputVector;
        internal readonly uint[] CalloutOutputVector;

        internal PcreMatchBuffer(InternalRegex regex, PcreMatchSettings settings)
        {
            _regex = regex;
            _settings = settings;

            OutputVector = new uint[regex.OutputVectorSize];
            CalloutOutputVector = new uint[regex.OutputVectorSize];

            _regex.TryGetCalloutInfoByPatternPosition(0); // Make sure callout info is initialized
        }

        [Pure]
        public bool IsMatch(ReadOnlySpan<char> subject)
            => Match(subject, 0, PcreMatchOptions.None, null).Success;

        [Pure]
        public bool IsMatch(ReadOnlySpan<char> subject, int startIndex)
            => Match(subject, startIndex, PcreMatchOptions.None, null).Success;

        [Pure]
        public PcreRefMatch Match(ReadOnlySpan<char> subject)
            => Match(subject, 0, PcreMatchOptions.None, null);

        [Pure]
        public PcreRefMatch Match(ReadOnlySpan<char> subject, PcreMatchOptions options)
            => Match(subject, 0, options, null);

        [Pure]
        public PcreRefMatch Match(ReadOnlySpan<char> subject, int startIndex)
            => Match(subject, startIndex, PcreMatchOptions.None, null);

        [Pure]
        public PcreRefMatch Match(ReadOnlySpan<char> subject, int startIndex, PcreMatchOptions options)
            => Match(subject, startIndex, options, null);

        public PcreRefMatch Match(ReadOnlySpan<char> subject, PcreRefCalloutFunc? onCallout)
            => Match(subject, 0, PcreMatchOptions.None, onCallout);

        public PcreRefMatch Match(ReadOnlySpan<char> subject, PcreMatchOptions options, PcreRefCalloutFunc? onCallout)
            => Match(subject, 0, options, onCallout);

        public PcreRefMatch Match(ReadOnlySpan<char> subject, int startIndex, PcreRefCalloutFunc? onCallout)
            => Match(subject, startIndex, PcreMatchOptions.None, onCallout);

        public PcreRefMatch Match(ReadOnlySpan<char> subject, int startIndex, PcreMatchOptions options, PcreRefCalloutFunc? onCallout)
        {
            if (startIndex < 0 || startIndex > subject.Length)
                throw new ArgumentOutOfRangeException("Invalid StartIndex value", default(Exception));

            var match = _regex.CreateRefMatch(OutputVector);
            match.FirstMatch(subject, _settings, startIndex, options, onCallout, CalloutOutputVector);

            return match;
        }

        [Pure]
        public RefMatchEnumerable Matches(ReadOnlySpan<char> subject)
            => Matches(subject, 0, PcreMatchOptions.None, null);

        [Pure]
        public RefMatchEnumerable Matches(ReadOnlySpan<char> subject, int startIndex)
            => Matches(subject, startIndex, PcreMatchOptions.None, null);

        [Pure]
        public RefMatchEnumerable Matches(ReadOnlySpan<char> subject, int startIndex, PcreRefCalloutFunc? onCallout)
            => Matches(subject, startIndex, PcreMatchOptions.None, onCallout);

        [Pure]
        public RefMatchEnumerable Matches(ReadOnlySpan<char> subject, int startIndex, PcreMatchOptions options, PcreRefCalloutFunc? onCallout)
        {
            if (startIndex < 0 || startIndex > subject.Length)
                throw new ArgumentOutOfRangeException("Invalid StartIndex value", default(Exception));

            return new RefMatchEnumerable(this, subject, startIndex, options, onCallout);
        }

        public override string ToString()
            => _regex.Pattern;

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

            public RefMatchEnumerator GetEnumerator()
                => new(_buffer, _subject, _startIndex, _options, _callout);
        }

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

            public readonly PcreRefMatch Current => _match;

            public bool MoveNext()
            {
                if (_buffer == null)
                    return false;

                if (!_match.IsInitialized)
                {
                    _match = _buffer._regex.CreateRefMatch(_buffer.OutputVector);
                    _match.FirstMatch(_subject, _buffer._settings, _startIndex, _options, _callout, _buffer.CalloutOutputVector);
                }
                else
                {
                    _match.NextMatch(_buffer._settings, _options, _callout, _buffer.CalloutOutputVector);
                }

                if (_match.Success)
                    return true;

                _buffer = null;
                return false;
            }
        }
    }
}
