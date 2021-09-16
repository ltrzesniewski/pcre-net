using System;
using System.Diagnostics.Contracts;
using PCRE.Internal;

namespace PCRE
{
    public class PcreMatchBuffer
    {
        private readonly InternalRegex _regex;
        internal readonly uint[] OutputVector;

        internal PcreMatchBuffer(InternalRegex regex)
        {
            _regex = regex;
            OutputVector = new uint[regex.OutputVectorSize];
        }

        [Pure]
        public bool IsMatch(ReadOnlySpan<char> subject)
            => IsMatch(subject, 0);

        [Pure]
        public bool IsMatch(ReadOnlySpan<char> subject, int startIndex)
            => Match(subject, startIndex).Success;

        [Pure]
        public PcreRefMatch Match(ReadOnlySpan<char> subject)
            => Match(subject, PcreMatchSettings.Default);

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
            => Match(subject, PcreMatchSettings.GetSettings(startIndex, options, onCallout)); // TODO: Allocation

        public PcreRefMatch Match(ReadOnlySpan<char> subject, PcreMatchSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (settings.StartIndex < 0 || settings.StartIndex > subject.Length)
                throw new ArgumentOutOfRangeException("Invalid StartIndex value", default(Exception));

            var match = _regex.CreateRefMatch(OutputVector);
            match.FirstMatch(subject, settings, settings.StartIndex);

            return match;
        }

        [Pure]
        public RefMatchEnumerable Matches(ReadOnlySpan<char> subject)
            => Matches(subject, PcreMatchSettings.Default);

        [Pure]
        public RefMatchEnumerable Matches(ReadOnlySpan<char> subject, int startIndex)
            => Matches(subject, startIndex, null);

        [Pure]
        public RefMatchEnumerable Matches(ReadOnlySpan<char> subject, int startIndex, PcreRefCalloutFunc? onCallout)
            => Matches(subject, PcreMatchSettings.GetSettings(startIndex, PcreMatchOptions.None, onCallout)); // TODO: Allocation

        [Pure]
        public RefMatchEnumerable Matches(ReadOnlySpan<char> subject, PcreMatchSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (settings.StartIndex < 0 || settings.StartIndex > subject.Length)
                throw new ArgumentOutOfRangeException("Invalid StartIndex value", default(Exception));

            return new RefMatchEnumerable(this, subject, settings);
        }

        public override string ToString()
            => _regex.Pattern;

        public readonly ref struct RefMatchEnumerable
        {
            private readonly ReadOnlySpan<char> _subject;
            private readonly PcreMatchSettings _settings;
            private readonly PcreMatchBuffer _buffer;

            internal RefMatchEnumerable(PcreMatchBuffer buffer, ReadOnlySpan<char> subject, PcreMatchSettings settings)
            {
                _buffer = buffer;
                _subject = subject;
                _settings = settings;
            }

            public RefMatchEnumerator GetEnumerator()
                => new(_buffer, _subject, _settings);
        }

        public ref struct RefMatchEnumerator
        {
            private readonly ReadOnlySpan<char> _subject;
            private readonly PcreMatchSettings _settings;
            private PcreMatchBuffer? _buffer;
            private PcreRefMatch _match;

            internal RefMatchEnumerator(PcreMatchBuffer buffer, ReadOnlySpan<char> subject, PcreMatchSettings settings)
            {
                _buffer = buffer;
                _subject = subject;
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
                    _match.FirstMatch(_subject, _settings, _settings.StartIndex);
                }
                else
                {
                    _match.NextMatch(_settings);
                }

                if (_match.Success)
                    return true;

                _buffer = null;
                return false;
            }
        }
    }
}
