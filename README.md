
PCRE.NET
=========

**Perl Compatible Regular Expressions for .NET**

[![Build](https://github.com/ltrzesniewski/pcre-net/workflows/Build/badge.svg)](https://github.com/ltrzesniewski/pcre-net/actions?query=workflow%3ABuild)
[![NuGet Package](https://img.shields.io/nuget/v/PCRE.NET.svg?logo=NuGet)](https://www.nuget.org/packages/PCRE.NET)
[![GitHub release](https://img.shields.io/github/release/ltrzesniewski/pcre-net.svg?logo=GitHub)](https://github.com/ltrzesniewski/pcre-net/releases)
[![PCRE](https://img.shields.io/badge/pcre-v10.35-blue.svg)](http://www.pcre.org/)
[![License](https://img.shields.io/badge/license-BSD-blue.svg)](https://github.com/ltrzesniewski/pcre-net/blob/master/LICENCE)

PCRE.NET is a .NET wrapper for the [PCRE library](http://www.pcre.org/). The goal of this project is to bring most of PCRE's features for use from .NET applications with as little overhead as possible.

The current version is **0.13.1**, based on PCRE **10.35**. It targets .NET Standard 2.0 and supports the following systems:

 - Windows x64
 - Windows x86
 - Linux x64
 - macOS x64

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

- Works on the .NET Framework and on .NET Core (.NET Standard 2.0 target)
- Support for compiled patterns (x86/x64 JIT)
- Lazy evaluation whenever possible (for instance `PcreRegex.Matches` returns `IEnumerable<PcreMatch>`)
- The API is similar to .NET's `System.Text.RegularExpressions`
- Callout support (numbered and string-based)
- Mark retrieval support
- Conversion from POSIX BRE, POSIX ERE and glob patterns (`PcreConvert` class)
- Additional API for `ReadOnlySpan<char>` matching, based on `ref struct` types

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

- Include more tests from the PCRE test suite
- Documentation
- XML doc comments
