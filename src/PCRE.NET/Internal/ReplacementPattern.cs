using System;
using System.Collections.Generic;
using System.Globalization;

namespace PCRE.Internal;

internal static partial class ReplacementPattern
{
    private static bool TryParse(string? replacementPattern, out List<ReplacementPart> parts)
    {
        parts = [];

        if (replacementPattern is null)
            return false;

        if (replacementPattern.Length == 0)
            return true;

#if NET
        var placeholderIndex = replacementPattern.IndexOf('$', StringComparison.Ordinal);
#else
        var placeholderIndex = replacementPattern.IndexOf('$');
#endif

        if (placeholderIndex < 0)
        {
            parts.Add(new LiteralPart(replacementPattern));
            return true;
        }

        var idx = 0;

        if (placeholderIndex > 0)
        {
            idx = placeholderIndex;
            parts.Add(new LiteralPart(replacementPattern, 0, idx));
        }

        while (true)
        {
            if (idx >= replacementPattern.Length)
                break;

            if (replacementPattern[idx] == '$')
            {
                ++idx;

                if (idx >= replacementPattern.Length)
                {
                    parts.Add(LiteralPart.Dollar);
                    break;
                }

                switch (replacementPattern[idx])
                {
                    case '$':
                        parts.Add(LiteralPart.Dollar);
                        ++idx;
                        break;

                    case '&':
                        parts.Add(IndexedGroupPart.FullMatch);
                        ++idx;
                        break;

                    case '`':
                        parts.Add(PreMatchPart.Instance);
                        ++idx;
                        break;

                    case '\'':
                        parts.Add(PostMatchPart.Instance);
                        ++idx;
                        break;

                    case '_':
                        parts.Add(FullInputPart.Instance);
                        ++idx;
                        break;

                    case '+':
                        parts.Add(LastMatchedGroupPart.Instance);
                        ++idx;
                        break;

                    case '{':
                    {
                        var startIdx = idx;
                        while (idx < replacementPattern.Length && replacementPattern[idx] != '}')
                            ++idx;

                        if (idx < replacementPattern.Length && replacementPattern[idx] == '}' && idx > startIdx + 1)
                        {
                            var groupName = replacementPattern.Substring(startIdx + 1, idx - startIdx - 1);
                            var fallback = replacementPattern.Substring(startIdx - 1, idx - startIdx + 2);
                            parts.Add(new NamedGroupPart(groupName, fallback));
                            ++idx;
                            break;
                        }

                        parts.Add(new LiteralPart(replacementPattern, startIdx - 1, idx - startIdx + 1));
                        break;
                    }

                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    {
                        var startIdx = idx;
                        while (idx < replacementPattern.Length && replacementPattern[idx] is >= '0' and <= '9')
                            ++idx;

                        var fallback = replacementPattern.Substring(startIdx - 1, idx - startIdx + 1);

#if NET
                        var groupIndexString = replacementPattern.AsSpan(startIdx, idx - startIdx);
#else
                        var groupIndexString = replacementPattern.Substring(startIdx, idx - startIdx);
#endif

                        if (int.TryParse(groupIndexString, NumberStyles.None, CultureInfo.InvariantCulture, out var groupIndex))
                        {
                            parts.Add(new IndexedGroupPart(groupIndex, fallback));
                            break;
                        }

                        parts.Add(new LiteralPart(fallback));
                        break;
                    }

                    default:
                        parts.Add(new LiteralPart(replacementPattern, idx - 1, 2));
                        ++idx;
                        break;
                }
            }
            else
            {
                var startIdx = idx;
                while (idx < replacementPattern.Length && replacementPattern[idx] != '$')
                    ++idx;

                parts.Add(new LiteralPart(replacementPattern, startIdx, idx - startIdx));
            }
        }

        return true;
    }

    internal abstract partial class ReplacementPart
    {
        public abstract override string ToString();
    }

    internal sealed partial class LiteralPart : ReplacementPart
    {
        public static readonly LiteralPart Dollar = new("$");

        private readonly string _text;
        private readonly int _startIndex;
        private readonly int _length;

        public string LiteralText => _text.Substring(_startIndex, _length);

        public LiteralPart(string text)
        {
            _text = text;
            _startIndex = 0;
            _length = _text.Length;
        }

        public LiteralPart(string text, int startIndex, int length)
        {
            _text = text;
            _startIndex = startIndex;
            _length = length;
        }

        public override string ToString()
            => $"Literal: {LiteralText}";
    }

    internal sealed partial class IndexedGroupPart : ReplacementPart
    {
        public static readonly IndexedGroupPart FullMatch = new(0, null);

        private readonly int _index;
        private readonly string? _fallback;

        public IndexedGroupPart(int index, string? fallback)
        {
            _index = index;
            _fallback = fallback;
        }

        public override string ToString()
            => _index == 0
                ? "Full match"
                : $"Group: #{_index}";
    }

    internal sealed partial class NamedGroupPart : ReplacementPart
    {
        private readonly string _name;
        private readonly int _index;
        private readonly string? _fallback;

        public NamedGroupPart(string name, string? fallback)
        {
            _name = name;
            _fallback = fallback;

            if (!int.TryParse(name, NumberStyles.None, CultureInfo.InvariantCulture, out _index))
                _index = -1;
        }

        public override string ToString()
            => $"Group: {_name}";
    }

    internal sealed partial class PreMatchPart : ReplacementPart
    {
        public static readonly PreMatchPart Instance = new();

        public override string ToString()
            => "Pre match";
    }

    internal sealed partial class PostMatchPart : ReplacementPart
    {
        public static readonly PostMatchPart Instance = new();

        public override string ToString()
            => "Post match";
    }

    internal sealed partial class FullInputPart : ReplacementPart
    {
        public static readonly FullInputPart Instance = new();

        public override string ToString()
            => "Full input";
    }

    internal sealed partial class LastMatchedGroupPart : ReplacementPart
    {
        public static readonly LastMatchedGroupPart Instance = new();

        public override string ToString()
            => "Last matched group";
    }
}
