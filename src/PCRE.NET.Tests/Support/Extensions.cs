using System.Collections.Generic;

namespace PCRE.Tests.Support;

internal static class Extensions
{
    public static List<T> ToList<T>(this PcreMatchBuffer.RefMatchEnumerable enumerable, PcreRefMatch.Func<T> selector)
    {
        var result = new List<T>();

        foreach (var item in enumerable)
            result.Add(selector(item));

        return result;
    }

    public static List<T> ToList<T>(this PcreMatchBuffer8Bit.RefMatchEnumerable enumerable, PcreRefMatch8Bit.Func<T> selector)
    {
        var result = new List<T>();

        foreach (var item in enumerable)
            result.Add(selector(item));

        return result;
    }
}
