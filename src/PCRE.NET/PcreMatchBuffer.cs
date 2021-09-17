using System;
using System.Diagnostics.Contracts;
using PCRE.Internal;

namespace PCRE
{
    public class PcreMatchBuffer
    {
        private readonly InternalRegex _regex;
        internal readonly uint[] OutputVector;
        internal readonly uint[] CalloutOutputVector;

        internal PcreMatchBuffer(InternalRegex regex)
        {
            _regex = regex;
            OutputVector = new uint[regex.OutputVectorSize];
            CalloutOutputVector = new uint[regex.OutputVectorSize];
        }

        [Pure]
        public bool IsMatch(ReadOnlySpan<char> subject)
            => Match(subject, 0, PcreMatchOptions.None, null, PcreMatchSettings.Default).Success;

        [Pure]
        public bool IsMatch(ReadOnlySpan<char> subject, int startIndex)
            => Match(subject, startIndex, PcreMatchOptions.None, null, PcreMatchSettings.Default).Success;

        [Pure]
        public PcreRefMatch Match(ReadOnlySpan<char> subject)
            => Match(subject, 0, PcreMatchOptions.None, null, PcreMatchSettings.Default);

        [Pure]
        public PcreRefMatch Match(ReadOnlySpan<char> subject, PcreMatchOptions options)
            => Match(subject, 0, options, null, PcreMatchSettings.Default);

        [Pure]
        public PcreRefMatch Match(ReadOnlySpan<char> subject, int startIndex)
            => Match(subject, startIndex, PcreMatchOptions.None, null, PcreMatchSettings.Default);

        [Pure]
        public PcreRefMatch Match(ReadOnlySpan<char> subject, int startIndex, PcreMatchOptions options)
            => Match(subject, startIndex, options, null, PcreMatchSettings.Default);

        public PcreRefMatch Match(ReadOnlySpan<char> subject, PcreRefCalloutFunc? onCallout)
            => Match(subject, 0, PcreMatchOptions.None, onCallout, PcreMatchSettings.Default);

        public PcreRefMatch Match(ReadOnlySpan<char> subject, PcreMatchOptions options, PcreRefCalloutFunc? onCallout)
            => Match(subject, 0, options, onCallout, PcreMatchSettings.Default);

        public PcreRefMatch Match(ReadOnlySpan<char> subject, int startIndex, PcreRefCalloutFunc? onCallout)
            => Match(subject, startIndex, PcreMatchOptions.None, onCallout, PcreMatchSettings.Default);

        public PcreRefMatch Match(ReadOnlySpan<char> subject, int startIndex, PcreMatchOptions options, PcreRefCalloutFunc? onCallout)
            => Match(subject, startIndex, options, onCallout, PcreMatchSettings.Default);

        public PcreRefMatch Match(ReadOnlySpan<char> subject, int startIndex, PcreMatchOptions options, PcreRefCalloutFunc? onCallout, PcreMatchSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (startIndex < 0 || startIndex > subject.Length)
                throw new ArgumentOutOfRangeException("Invalid StartIndex value", default(Exception));

            var match = _regex.CreateRefMatch(OutputVector);
            match.FirstMatch(subject, settings, startIndex, options.ToPatternOptions(), onCallout, CalloutOutputVector);

            return match;
        }

        [Pure]
        public RefMatchEnumerable Matches(ReadOnlySpan<char> subject)
            => Matches(subject, 0, PcreMatchOptions.None, null, PcreMatchSettings.Default);

        [Pure]
        public RefMatchEnumerable Matches(ReadOnlySpan<char> subject, int startIndex)
            => Matches(subject, startIndex, PcreMatchOptions.None, null, PcreMatchSettings.Default);

        [Pure]
        public RefMatchEnumerable Matches(ReadOnlySpan<char> subject, int startIndex, PcreRefCalloutFunc? onCallout)
            => Matches(subject, startIndex, PcreMatchOptions.None, onCallout, PcreMatchSettings.Default);

        [Pure]
        public RefMatchEnumerable Matches(ReadOnlySpan<char> subject, int startIndex, PcreMatchOptions options, PcreRefCalloutFunc? onCallout, PcreMatchSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (startIndex < 0 || startIndex > subject.Length)
                throw new ArgumentOutOfRangeException("Invalid StartIndex value", default(Exception));

            return new RefMatchEnumerable(this, subject, startIndex, options, onCallout, settings);
        }

        public override string ToString()
            => _regex.Pattern;

        public readonly ref struct RefMatchEnumerable
        {
            private readonly ReadOnlySpan<char> _subject;
            private readonly int _startIndex;
            private readonly PcreMatchOptions _options;
            private readonly PcreRefCalloutFunc? _callout;
            private readonly PcreMatchSettings _settings;
            private readonly PcreMatchBuffer _buffer;

            internal RefMatchEnumerable(PcreMatchBuffer buffer,
                                        ReadOnlySpan<char> subject,
                                        int startIndex,
                                        PcreMatchOptions options,
                                        PcreRefCalloutFunc? callout,
                                        PcreMatchSettings settings)
            {
                _buffer = buffer;
                _subject = subject;
                _startIndex = startIndex;
                _options = options;
                _callout = callout;
                _settings = settings;
            }

            public RefMatchEnumerator GetEnumerator()
                => new(_buffer, _subject, _startIndex, _options, _callout, _settings);
        }

        public ref struct RefMatchEnumerator
        {
            private readonly ReadOnlySpan<char> _subject;
            private readonly int _startIndex;
            private readonly PcreMatchOptions _options;
            private readonly PcreRefCalloutFunc? _callout;
            private readonly PcreMatchSettings _settings;
            private PcreMatchBuffer? _buffer;
            private PcreRefMatch _match;

            internal RefMatchEnumerator(PcreMatchBuffer buffer,
                                        ReadOnlySpan<char> subject,
                                        int startIndex,
                                        PcreMatchOptions options,
                                        PcreRefCalloutFunc? callout,
                                        PcreMatchSettings settings)
            {
                _buffer = buffer;
                _subject = subject;
                _startIndex = startIndex;
                _options = options;
                _callout = callout;
                _settings = settings;
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
                    _match.FirstMatch(_subject, _settings, _startIndex, _options.ToPatternOptions(), _callout, _buffer.CalloutOutputVector);
                }
                else
                {
                    _match.NextMatch(_settings, _options.ToPatternOptions(), _callout, _buffer.CalloutOutputVector);
                }

                if (_match.Success)
                    return true;

                _buffer = null;
                return false;
            }
        }
    }
}
