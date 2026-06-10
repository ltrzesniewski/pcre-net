using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using PCRE.Internal;

namespace PCRE.Analyzers;

internal sealed class ReplacementPatternSet
{
    private readonly Dictionary<string, PatternItem> _replacementPatterns = [];

    public PatternItem? GetOrAdd(string? replacementPattern)
    {
        if (replacementPattern is null)
            return null;

        if (_replacementPatterns.TryGetValue(replacementPattern, out var existingItem))
            return existingItem;

        if (ReplacementPattern.Parse(replacementPattern) is not { } patternModel)
            return null;

        var patternId = _replacementPatterns.Count;
        var item = new PatternItem(patternId, patternModel);
        _replacementPatterns.Add(replacementPattern, item);

        return item;
    }

    internal void AppendFields(CodeWriter writer, LanguageVersion languageVersion)
    {
        foreach (var item in _replacementPatterns.Values.OrderBy(i => i.PatternId))
            item.AppendField(writer, languageVersion);
    }

    public void AppendHelpers(CodeWriter writer, LanguageVersion languageVersion)
    {
        foreach (var part in _replacementPatterns.Values.SelectMany(i => i.PatternModel.Parts).DistinctBy(p => p.GetType()))
            part.AppendHelpers(writer, languageVersion);
    }

    public sealed record PatternItem(int PatternId, ReplacementPattern.PatternModel PatternModel)
    {
        public void AppendField(CodeWriter writer, LanguageVersion languageVersion)
        {
            writer.AppendLine(
                $"""
                private static readonly {PatternModel.GetLambdaType()} _replacementFunc{PatternId}
                    = {PatternModel.GetLambda(languageVersion)};

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
