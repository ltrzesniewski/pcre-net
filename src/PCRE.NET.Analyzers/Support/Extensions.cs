using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace PCRE.NET.Analyzers.Support;

internal static class Extensions
{
    public static IncrementalValuesProvider<T> WithLambdaComparer<T>(this IncrementalValuesProvider<T> source, Func<T, T, bool> equals, Func<T, int> getHashCode)
        => source.WithComparer(new LambdaComparer<T>(equals, getHashCode));

    public static IncrementalValuesProvider<T> WhereNotNull<T>(this IncrementalValuesProvider<T?> provider)
        where T : class
        => provider.Where(static item => item is not null)!;

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

    private static IncrementalValueProvider<bool> AreInterceptorsEnabled(this IncrementalGeneratorInitializationContext context)
        => context.ParseOptionsProvider
                  .Select(static (i, _) => i.Features.TryGetValue("InterceptorsNamespaces", out var value) && value.Split(';').Any(static i => i.Trim() == "PCRE.Generated"));

    public static IncrementalValuesProvider<T> WhereNotNullAndInterceptorsEnabled<T>(this IncrementalValuesProvider<T?> source, IncrementalGeneratorInitializationContext context)
        where T : class
    {
        return source.Combine(context.AreInterceptorsEnabled())
                     .SelectMany(static (pair, _) =>
                     {
                         var (item, isEnabled) = pair;
                         return isEnabled && item is not null ? ImmutableArray.Create(item) : ImmutableArray<T>.Empty;
                     });
    }

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
