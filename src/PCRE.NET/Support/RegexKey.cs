using System;

namespace PCRE.Support
{
    internal struct RegexKey : IEquatable<RegexKey>
    {
        public readonly string Pattern;
        public readonly PcreRegexSettings Settings;

        public RegexKey(string pattern, PcreRegexSettings settings)
        {
            Pattern = pattern;
            Settings = settings.AsReadOnly();
        }

        public bool Equals(RegexKey other)
        {
            return Pattern == other.Pattern && Settings.CompareValues(other.Settings);
        }

        public override bool Equals(object obj)
        {
            return obj is RegexKey && Equals((RegexKey)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Pattern != null ? Pattern.GetHashCode() : 0);
            }
        }

        public static bool operator ==(RegexKey left, RegexKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RegexKey left, RegexKey right)
        {
            return !left.Equals(right);
        }
    }
}
