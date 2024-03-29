﻿using System;

namespace PCRE.Internal;

internal readonly struct RegexKey(string pattern, PcreRegexSettings settings) : IEquatable<RegexKey>
{
    public readonly string Pattern = pattern;
    public readonly PcreRegexSettings Settings = settings.AsReadOnly();

    public bool Equals(RegexKey other)
        => Pattern == other.Pattern
           && Settings.CompareValues(other.Settings);

    public override bool Equals(object? obj)
        => obj is RegexKey key && Equals(key);

    public override int GetHashCode()
    {
        unchecked
        {
#if NET
            return Pattern?.GetHashCode(StringComparison.Ordinal) ?? 0;
#else
            return Pattern?.GetHashCode() ?? 0;
#endif
        }
    }

    public static bool operator ==(RegexKey left, RegexKey right) => left.Equals(right);
    public static bool operator !=(RegexKey left, RegexKey right) => !left.Equals(right);
}
