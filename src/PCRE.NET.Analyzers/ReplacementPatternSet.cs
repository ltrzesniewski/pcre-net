using System.Collections.Generic;
using System.Linq;
using PCRE.Internal;
using PCRE.NET.Analyzers.Support;

namespace PCRE.NET.Analyzers;

internal sealed class ReplacementPatternSet
{
    private readonly Dictionary<string, PatternItem> _replacementPatterns = [];

    public PatternItem? GetOrAdd(string? replacementPattern, out bool added)
    {
        if (replacementPattern is null)
        {
            added = false;
            return null;
        }

        if (_replacementPatterns.TryGetValue(replacementPattern, out var existingItem))
        {
            added = false;
            return existingItem;
        }

        if (ReplacementPattern.Parse(replacementPattern) is not { } patternModel)
        {
            added = false;
            return null;
        }

        var patternId = _replacementPatterns.Count;
        var item = new PatternItem(patternId, patternModel);
        _replacementPatterns.Add(replacementPattern, item);

        added = true;
        return item;
    }

    internal void AppendFields(CodeWriter writer)
    {
        foreach (var item in _replacementPatterns.Values.OrderBy(i => i.PatternId))
            item.AppendField(writer);
    }

    public void AppendHelpers(CodeWriter writer)
    {
        foreach (var part in _replacementPatterns.Values.SelectMany(i => i.PatternModel.Parts).DistinctBy(p => p.GetType()))
            part.AppendHelpers(writer);
    }

    public sealed record PatternItem(int PatternId, ReplacementPattern.PatternModel PatternModel)
    {
        public void AppendField(CodeWriter writer)
        {
            writer.AppendLine(
                $"""
                        private static readonly {PatternModel.GetLambdaType()} _replacementFunc{PatternId}
                            = {PatternModel.GetLambda()};

                """
            );
        }

        public string GetReplacementFuncCall()
        {
            return PatternModel.NeedsSubject
                ? $"match => _replacementFunc{PatternId}(match, subject)"
                : $"_replacementFunc{PatternId}";
        }
    }
}
