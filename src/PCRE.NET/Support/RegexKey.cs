using System;

namespace PCRE.Support
{
    internal struct RegexKey : IEquatable<RegexKey>
    {
        public readonly string Pattern;
        public readonly PcreOptions Options;

        public RegexKey(string pattern, PcreOptions options)
        {
            Pattern = pattern;
            Options = options;
        }

        public bool Equals(RegexKey other)
        {
            return Options == other.Options && Pattern == other.Pattern;
        }

        public override bool Equals(object obj)
        {
            return obj is RegexKey && Equals((RegexKey)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Pattern != null ? Pattern.GetHashCode() : 0) * 397) ^ Options.GetHashCode();
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
