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
    private static readonly string[] _cuts =
    [
        "QUERY_BAR|JX_NEST_TOO_DEEP",
        "E|CLASS_NEST_TOO_DEEP",
        "E|CLASS_INVALID_OPERATOR",
        "E|CLASS_UNEXPECTED_OPERATOR",
        "E|CLASS_EXPECTED_OPERAND",
        "E|CLASS_MIXED_OPERATORS",
        "E|CLASS_HINT_SQUARE_BRACKET",
        "PERL_E|CLASS_UNEXPECTED_EXPR",
        "PERL_E|CLASS_EMPTY_EXPR",
        "PERL_E|CLASS_MISSING_CLOSE",
        "PERL_E|CLASS_UNEXPECTED_CHAR",
        "NULL_ERROR|OFFSET",
        "NO|MATCH",
        "BAD|DATA",
        "MIXED|TABLES",
        "BAD|MAGIC",
        "BAD|MODE",
        "BAD|OFFSET",
        "BAD|OPTION",
        "BAD|REPLACEMENT",
        "BAD|UTF|OFFSET",
        "DFA_BAD|RESTART",
        "DFA_RECURSE",
        "DFA_U|COND",
        "DFA_U|FUNC",
        "DFA_U|ITEM",
        "DFA_WS|SIZE",
        "JIT_BAD|OPTION",
        "JIT_STACK|LIMIT",
        "MATCH|LIMIT",
        "NO|MEMORY",
        "NO|SUBSTRING",
        "NO|UNIQUE|SUBSTRING",
        "RECURSE|LOOP",
        "DEPTH|LIMIT",
        "BAD|OFFSET|LIMIT",
        "BAD|REP|ESCAPE",
        "REP|MISSING|BRACE",
        "BAD|SUBSTITUTION",
        "BAD|SUBS|PATTERN",
        "TOO|MANY|REPLACE",
        "BAD|SERIALIZED|DATA",
        "HEAP|LIMIT",
        "CONVERT|SYNTAX",
        "INTERNAL_DUP|MATCH",
        "DFA_U|INVALID_UTF",
        "INVALID|OFFSET",
        "JIT|UNSUPPORTED",
        "REPLACE|CASE",
        "TOO|LARGE|REPLACE",
        "DIFF|SUBS|PATTERN",
        "DIFF|SUBS|SUBJECT",
        "DIFF|SUBS|OFFSET",
        "DIFF|SUBS|OPTIONS",
        "BAD|BACKSLASH|K",
        "PARTIAL|SUBS",
    ];

    [Test]
    public void generate_error_codes()
    {
        const string errorPrefix = "PCRE2_ERROR_";

        var errorCodes = typeof(PcreConstants).GetFields(BindingFlags.Public | BindingFlags.Static)
                                              .Where(i => i.IsLiteral && i.Name.StartsWith(errorPrefix))
                                              .Select(i => (i.Name, (int)i.GetRawConstantValue()!));

        foreach (var (constantName, errorCode) in errorCodes)
        {
            var constantNameWithoutPrefix = constantName.Substring(errorPrefix.Length);
            var memberName = constantNameWithoutPrefix;

            foreach (var cut in _cuts)
            {
                if (constantNameWithoutPrefix == cut.Replace("|", string.Empty))
                {
                    memberName = cut.Replace("|", "_");
                    break;
                }
            }

            memberName = Regex.Replace(memberName.ToLowerInvariant(), @"(?:^|_)(?<char>\w)", m => m.Groups["char"].Value.ToUpperInvariant());

            var errorMessage = default(Native16Bit).GetErrorMessage(errorCode);

            errorMessage = WebUtility.HtmlEncode(errorMessage)
                                     .Replace("&#39;", "'");

            errorMessage = Regex.Replace(
                errorMessage,
                """
                (?<=^|[ ])
                (?:
                    [a-z0-9]+_[a-z0-9_]+ \(\)  # Function
                    | \\   [^ ]+               # Escape
                    | \(\? [^ ]+               # Group
                    | \(\* [a-zA-Z_]+ \)       # Verb
                    | { [^}]+ }                # Braces
                    | (?! POSIX | ASCII | UTF | UCP | DFA | JIT) [A-Z]{2,}
                    | (?! - ) \W+
                    | ^erroroffset
                )
                (?=[ :),]|$)
                """,
                "<c>$0</c>",
                RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace
            );

            errorMessage = errorMessage.Replace(",</c>", "</c>,");

            if (char.IsLower(errorMessage[0]))
                errorMessage = char.ToUpperInvariant(errorMessage[0]) + errorMessage.Substring(1);

            var lastChar = errorMessage[errorMessage.Length - 1];
            if ((!char.IsPunctuation(lastChar) || lastChar is ')') && (!errorMessage.EndsWith("</c>") || errorMessage.EndsWith("()</c>")))
                errorMessage += ".";

            Console.WriteLine("/// <summary>");
            Console.WriteLine($"/// <c>{constantName}</c> - {errorMessage}");
            Console.WriteLine("/// </summary>");
            Console.WriteLine($"{memberName} = PcreConstants.{constantName},");
            Console.WriteLine();
        }
    }
}
