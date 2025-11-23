using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace PCRE.NET.InternalAnalyzers.Support;

internal static class Extensions
{
    public static IncrementalValuesProvider<T> WithLambdaComparer<T>(this IncrementalValuesProvider<T> source, Func<T, T, bool> equals, Func<T, int> getHashCode)
        => source.WithComparer(new LambdaComparer<T>(equals, getHashCode));

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
