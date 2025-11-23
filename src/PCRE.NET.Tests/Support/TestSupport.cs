using System;
using System.Text;

namespace PCRE.Tests.Support;

public static class TestSupport
{
    public static readonly Encoding Latin1Encoding = Encoding.GetEncoding("ISO-8859-1");

    public static byte[] ToLatin1Bytes(this string str)
        => Latin1Encoding.GetBytes(str);

    public static PcreRegex8Bit CreatePcreRegex8Bit(string pattern)
        => new(pattern.ToLatin1Bytes(), Latin1Encoding);

    public static PcreRegex8Bit CreatePcreRegex8Bit(string pattern, PcreOptions options)
        => new(pattern.ToLatin1Bytes(), Latin1Encoding, options);

    public static PcreRegex8Bit CreatePcreRegex8Bit(string pattern, PcreRegexSettings settings)
        => new(pattern.ToLatin1Bytes(), Latin1Encoding, settings);

    public static PcreRegex8Bit CreatePcreRegex8Bit(ReadOnlySpan<byte> pattern)
        => new(pattern, Latin1Encoding);

    public static PcreRegex8Bit CreatePcreRegex8Bit(ReadOnlySpan<byte> pattern, PcreOptions options)
        => new(pattern, Latin1Encoding, options);

    public static PcreRegex8Bit CreatePcreRegex8Bit(ReadOnlySpan<byte> pattern, PcreRegexSettings settings)
        => new(pattern, Latin1Encoding, settings);
}
