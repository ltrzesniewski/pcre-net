using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace PCRE.Internal;

internal partial class ReplacementPattern
{
    [SuppressMessage("ReSharper", "MergeIntoPattern")]
    public static Func<PcreMatch, string> Parse(string replacementPattern)
    {
        if (ReferenceEquals(replacementPattern, null))
            throw new ArgumentNullException(nameof(replacementPattern));

        if (!TryParse(replacementPattern, out var parts))
            throw new ArgumentException("Invalid replacement pattern", nameof(replacementPattern));

        if (parts.Count == 0)
            return static _ => string.Empty;

        if (parts.Count == 1 && parts[0] is LiteralPart literalPart)
        {
            var value = literalPart.LiteralText;
            return _ => value;
        }

        if (parts.TrueForAll(static part => part is LiteralPart))
        {
            var value = string.Concat(parts.Select(static part => ((LiteralPart)part).LiteralText));
            return _ => value;
        }

        return match =>
        {
            var sb = new StringBuilder();

            foreach (var part in parts)
                part.Append(match, sb);

            return sb.ToString();
        };
    }

    internal partial class ReplacementPart
    {
        public abstract void Append(PcreMatch match, StringBuilder sb);
    }

    internal partial class LiteralPart
    {
        public override void Append(PcreMatch match, StringBuilder sb)
            => sb.Append(_text, _startIndex, _length);
    }

    internal partial class IndexedGroupPart
    {
        public override void Append(PcreMatch match, StringBuilder sb)
        {
            if (match.TryGetGroup(_index, out var group))
            {
                if (group.Success)
                    sb.Append(match.Subject, group.Index, group.Length);
            }
            else
            {
                sb.Append(_fallback);
            }
        }
    }

    internal partial class NamedGroupPart
    {
        public override void Append(PcreMatch match, StringBuilder sb)
        {
            if (match.TryGetGroup(_name, out var group) || match.TryGetGroup(_index, out group))
            {
                if (group.Success)
                    sb.Append(match.Subject, group.Index, group.Length);
            }
            else
            {
                sb.Append(_fallback);
            }
        }
    }

    internal partial class PreMatchPart
    {
        public override void Append(PcreMatch match, StringBuilder sb)
            => sb.Append(match.Subject, 0, match.Index);
    }

    internal partial class PostMatchPart
    {
        public override void Append(PcreMatch match, StringBuilder sb)
        {
            var endOfMatch = match.EndIndex;
            sb.Append(match.Subject, endOfMatch, match.Subject.Length - endOfMatch);
        }
    }

    internal partial class FullInputPart
    {
        public override void Append(PcreMatch match, StringBuilder sb)
            => sb.Append(match.Subject);
    }

    internal partial class LastMatchedGroupPart
    {
        public override void Append(PcreMatch match, StringBuilder sb)
        {
            for (var i = match.CaptureCount; i > 0; --i)
            {
                if (match.TryGetGroup(i, out var group) && group.Success)
                {
                    sb.Append(match.Subject, group.Index, group.Length);
                    return;
                }
            }
        }
    }
}
