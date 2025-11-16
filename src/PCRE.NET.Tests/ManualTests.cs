using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using PCRE.Internal;

namespace PCRE.Tests;

[TestFixture, Explicit]
public class ManualTests
{
    [Test]
    public void generate_error_codes()
    {
        const string errorPrefix = "ERROR_";

        var errorCodes = typeof(PcreConstants).GetFields(BindingFlags.Public | BindingFlags.Static)
                                              .Where(i => i.IsLiteral && i.Name.StartsWith(errorPrefix))
                                              .Select(i => (i.Name, (int)i.GetRawConstantValue()!));

        foreach (var (name, value) in errorCodes)
        {
            var memberName = Regex.Replace(name.Substring(errorPrefix.Length).ToLowerInvariant(), @"(?:^|_)(?<char>\w)", m => m.Groups["char"].Value.ToUpperInvariant());

            Console.WriteLine("/// <summary>");
            Console.WriteLine($"/// <c>PCRE2_{name}</c> - {WebUtility.HtmlEncode(default(Native16Bit).GetErrorMessage(value))}");
            Console.WriteLine("/// </summary>");
            Console.WriteLine($"{memberName} = PcreConstants.{name},");
            Console.WriteLine();
        }
    }
}
