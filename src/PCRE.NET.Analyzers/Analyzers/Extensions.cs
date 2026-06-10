using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Operations;

namespace PCRE.Analyzers;

internal static class Extensions
{
    extension(LanguageVersion languageVersion)
    {
        public bool SupportsFileModifier => languageVersion >= LanguageVersion.CSharp11;
        public bool SupportsStaticAnonymousFunctions => languageVersion >= LanguageVersion.CSharp9;
        public bool SupportsNullableReferenceTypes => languageVersion >= LanguageVersion.CSharp8;
        public bool SupportsCoalescingAssignments => languageVersion >= LanguageVersion.CSharp8;
    }

    public static IncrementalValuesProvider<T> WithLambdaComparer<T>(this IncrementalValuesProvider<T> source, Func<T, T, bool> equals, Func<T, int> getHashCode)
        => source.WithComparer(new LambdaComparer<T>(equals, getHashCode));

    public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
    {
        var hashSet = new HashSet<TKey>();

        foreach (var item in source)
        {
            if (hashSet.Add(keySelector(item)))
                yield return item;
        }
    }

    public static IArgumentOperation? GetArgument(this IInvocationOperation invocation, string parameterName)
        => invocation.Arguments.FirstOrDefault(i => i.Parameter?.Name == parameterName);

    public static string Join(this IEnumerable<string> items, string separator)
        => string.Join(separator, items);

    public static AttributeData? GetAttribute(this ISymbol symbol, INamedTypeSymbol attributeType)
    {
        foreach (var attribute in symbol.GetAttributes())
        {
            if (SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeType))
                return attribute;
        }

        return null;
    }

    private sealed class LambdaComparer<T>(Func<T, T, bool> equals, Func<T, int> getHashCode) : IEqualityComparer<T>
    {
        public bool Equals(T? x, T? y)
            => x is not null
                ? y is not null && equals(x, y)
                : y is null;

        public int GetHashCode(T obj)
            => getHashCode(obj);
    }
}
