
PCRE.NET
=========

**Perl Compatible Regular Expressions for .NET**

[NuGet package](https://www.nuget.org/packages/PCRE.NET)

PCRE.NET is a .NET wrapper for the [PCRE library](http://www.pcre.org/). The goal of this project is to bring most of PCRE's features for use from .NET applications with as little overhead as possible.

**Status:** Version **0.6**, based on PCRE **10.20**, is available on [NuGet](https://www.nuget.org/packages/PCRE.NET). Feedback is welcome.  
As it is still at major version 0, the API may change anytime.

## Dependencies ##

- Version 0.6 requires the [Visual C++ 2015 Redistributable](https://www.microsoft.com/en-us/download/details.aspx?id=48145) package to be installed.
- Versions prior to 0.6 require the [Visual C++ 2013 Redistributable](http://www.microsoft.com/en-us/download/details.aspx?id=40784) package.

## Features ##

The following regex operations are supported:

- NFA matching and substring extraction:
  - `PcreRegex.Matches`
  - `PcreRegex.Match`
  - `PcreRegex.IsMatch`
- Matched string replacement: `PcreRegex.Replace`
  - Callbacks: `Func<PcreMatch, string>`
  - Replacement strings with placeholders: ``$n ${name} $& $_ $` $' $+``
- String splitting on matches: `PcreRegex.Split`
  - Captured groups are included in the result
- DFA matching:
  - `regexInstance.Dfa.Matches`
  - `regexInstance.Dfa.Match`
- Partial matching (when the subject is too short to match the pattern)

Library highlights:

- No marshaling - uses a mixed mode assembly to maximize performance
- Support for studied patterns
- Support for compiled patterns (x86/x64 JIT)
- Lazy evaluation whenever possible (for instance `PcreRegex.Matches` returns `IEnumerable<PcreMatch>`)
- The API is similar to .NET's `System.Text.RegularExpressions`
- Callout support (numbered and string-based)
- Mark retrieval support

## Example usage ##

- Extract all words except those within parentheses:

```C#
var matches = PcreRegex.Matches("(foo) bar (baz) 42", @"\(\w+\)(*SKIP)(*FAIL)|\w+")
                       .Select(m => m.Value)
                       .ToList();
// result: "bar", "42"
```

- Enclose a series of punctuation characters within angle brackets:

```C#
var result = PcreRegex.Replace("hello, world!!!", @"\p{P}+", "<$&>");
// result: "hello<,> world<!!!>"
```

- Partial matching:

```C#
var regex = new PcreRegex(@"(?<=abc)123");
var match = regex.Match("xyzabc12", PcreMatchOptions.PartialSoft);
// result: match.IsPartialMatch == true
```

- Validate a JSON string:

```C#
const string jsonPattern = @"
    (?(DEFINE)
        # An object is an unordered set of name/value pairs.
        (?<object> \{
            (?: (?&keyvalue) (?: , (?&keyvalue) )* )?
        (?&ws) \} )
        (?<keyvalue>
            (?&ws) (?&string) (?&ws) : (?&value)
        )

        # An array is an ordered collection of values.
        (?<array> \[
            (?: (?&value) (?: , (?&value) )* )?
        (?&ws) \] )

        # A value can be a string in double quotes, or a number,
        # or true or false or null, or an object or an array.
        (?<value> (?&ws)
            (?: (?&string) | (?&number) | (?&object) | (?&array) | true | false | null )
        )

        # A string is a sequence of zero or more Unicode characters,
        # wrapped in double quotes, using backslash escapes.
        (?<string>
            "" (?: [^""\\\p{Cc}]++ | \\u[0-9A-Fa-f]{4} | \\ [""\\/bfnrt] )* ""
            # \p{Cc} matches control characters
        )

        # A number is very much like a C or Java number, except that the octal
        # and hexadecimal formats are not used.
        (?<number>
            -? (?: 0 | [1-9][0-9]* ) (?: \. [0-9]+ )? (?: [Ee] [-+]? [0-9]+ )?
        )

        # Whitespace
        (?<ws> \s*+ )
    )

    \A (?&ws) (?&object) (?&ws) \z
";

var regex = new PcreRegex(jsonPattern, PcreOptions.IgnorePatternWhitespace);

const string subject = @"{
    ""hello"": ""world"",
    ""numbers"": [4, 8, 15, 16, 23, 42],
    ""foo"": null,
    ""bar"": -2.42e+17,
    ""baz"": true
}";

var isValidJson = regex.IsMatch(subject);
// result: true
```

## To do ##

- Implement a less verbose managed/native interface
- Include more tests from the PCRE test suite
- Documentation
- XML doc comments
