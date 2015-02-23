using System;

namespace PCRE.Tests.Pcre
{
    public sealed class TestPattern : IEquatable<TestPattern>
    {
        public string FullString { get; set; }

        public string Pattern { get; set; }
        public string OptionsString { get; set; }
        public PcreOptions PatternOptions { get; set; }
        public PcreOptions ResetOptionBits { get; set; }

        public int LineNumber { get; set; }

        public bool AllMatches { get; set; }
        public bool GetRemainingString { get; set; }
        public bool ExtractMarks { get; set; }
        public bool HexEncoding { get; set; }
        public bool IncludeInfo { get; set; }

        public string ReplaceWith { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as TestPattern);
        }

        public bool Equals(TestPattern other)
        {
            return other != null && string.Equals(FullString, other.FullString);
        }

        public override int GetHashCode()
        {
            return (FullString != null ? FullString.GetHashCode() : 0);
        }

        public static bool operator ==(TestPattern left, TestPattern right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TestPattern left, TestPattern right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return FullString ?? "???";
        }
    }
}
