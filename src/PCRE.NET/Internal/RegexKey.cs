using System;

namespace PCRE.Internal
{
    internal readonly struct RegexKey : IEquatable<RegexKey>
    {
        public readonly string Pattern;
        public readonly PcreRegexSettings Settings;

        public RegexKey(string pattern, PcreRegexSettings settings)
        {
            Pattern = pattern;
            Settings = settings.AsReadOnly();
        }

        public bool Equals(RegexKey other)
            => Pattern == other.Pattern
               && Settings.CompareValues(other.Settings);

        public override bool Equals(object obj)
            => obj is RegexKey key && Equals(key);

        public override int GetHashCode()
        {
            unchecked
            {
                return Pattern?.GetHashCode() ?? 0;
            }
        }

        public static bool operator ==(RegexKey left, RegexKey right) => left.Equals(right);
        public static bool operator !=(RegexKey left, RegexKey right) => !left.Equals(right);
    }
}
