# PCRE.NET                    <picture><source media="(prefers-color-scheme: dark)" srcset="icon-dark.png"><img src="icon.png" align="right" alt="Logo"></picture>

**Perl Compatible Regular Expressions for .NET**

[![Build](https://github.com/ltrzesniewski/pcre-net/workflows/Build/badge.svg)](https://github.com/ltrzesniewski/pcre-net/actions?query=workflow%3ABuild)
[![NuGet Package](https://img.shields.io/nuget/v/PCRE.NET.svg?logo=NuGet)](https://www.nuget.org/packages/PCRE.NET)
[![GitHub release](https://img.shields.io/github/release/ltrzesniewski/pcre-net.svg?logo=GitHub)](https://github.com/ltrzesniewski/pcre-net/releases)
[![PCRE2](https://img.shields.io/badge/pcre2-v10.44-blue.svg)](https://github.com/PCRE2Project/pcre2)
[![License](https://img.shields.io/badge/license-BSD-blue.svg)](https://github.com/ltrzesniewski/pcre-net/blob/master/LICENCE)
<br clear="right" />

PCRE.NET is a .NET wrapper for the [PCRE2 library](https://github.com/PCRE2Project/pcre2).

The following systems are supported:

 - Windows x64
 - Windows x86
 - Linux x64
 - macOS arm64
 - macOS x64

## API Types

### The classic API

This is a friendly API that is very similar to .NET's `System.Text.RegularExpressions`. It works on `string` objects, and supports the following operations:

- NFA matching and substring extraction:
  - `Matches`
  - `Match`
  - `IsMatch`
- Matched string replacement:
  - Using `Replace`, the PCRE.NET API:
    - Callbacks: `Func<PcreMatch, string>`
    - Replacement strings with placeholders: ``$n ${name} $& $_ $` $' $+``
  - Using `Substitute`, the PCRE2 API:
      - Replacement strings with placeholders: ``$n ${n} $$ $*MARK ${*MARK}``
      - Callouts for matches and substitutions
- String splitting on matches: `Split`

### The Span API

`PcreRegex` objects provide overloads which take a `ReadOnlySpan<char>` parameter for the following methods:

- `Matches`
- `Match`
- `IsMatch`
- `Substitute`

These methods return a `ref struct` type when possible, but are otherwise similar to the classic API.

### The zero-allocation API

This is the fastest matching API the library provides.

Call the `CreateMatchBuffer` method on a `PcreRegex` instance to create the necessary data structures up-front, then use the returned _match buffer_ for subsequent match operations. Performing a match through this buffer will not allocate further memory, reducing GC pressure and optimizing the process.

The downside of this approach is that the returned match buffer is _not_ thread-safe and _not_ reentrant: you _cannot_ perform a match operation with a buffer which is already being used - match operations need to be sequential.

It is also counter-productive to allocate a match buffer to perform a single match operation. Use this API if you need to match a pattern against many subject strings.

`PcreMatchBuffer` objects are disposable (and finalizable in case they're not disposed). They provide an API for matching against `ReadOnlySpan<char>` subjects.

If you're looking for maximum speed, consider using the following options:

- `PcreOptions.Compiled` at compile time to enable the JIT compiler, which will improve matching speed.
- `PcreMatchOptions.NoUtfCheck` at match time to skip the Unicode validity check: by default PCRE2 scans the entire input string to make sure it's valid Unicode.
- `PcreOptions.MatchInvalidUtf` at compile time if you plan to use `PcreMatchOptions.NoUtfCheck` and your subject strings may contain invalid Unicode sequences.

### The DFA matching API

This API provides regex matching in O(_subject length_) time. It is accessible through the `Dfa` property on a `PcreRegex` instance:

- `Dfa.Matches`
- `Dfa.Match`

You can read more about its features in [the PCRE2 documentation](https://pcre2project.github.io/pcre2/doc/html/pcre2matching.html), where it's described as the _alternative matching algorithm_.

## Library highlights

- Support for compiled patterns (x86/x64 JIT)
- Support for partial matching (when the subject is too short to match the pattern)
- Callout support (numbered and string-based)
- Mark retrieval support
- Conversion from POSIX BRE, POSIX ERE and glob patterns (`PcreConvert` class)

## Example usage

- Extract all words except those within parentheses:

```C#
var matches = PcreRegex.Matches("(foo) bar (baz) 42", @"\(\w+\)(*SKIP)(*FAIL)|\w+")
                       .Select(m => m.Value)
                       .ToList();
// result: "bar", "42"
```

- Enclose a series of punctuation characters within angle brackets using `Replace` (the PCRE.NET API):

```C#
var result = PcreRegex.Replace("hello, world!!!", @"\p{P}+", "<$&>");
// result: "hello<,> world<!!!>"
```

- Enclose a series of punctuation characters within angle brackets using `Substitute` (the PCRE2 API):

```C#
var result = PcreRegex.Substitute("hello, world!!!", @"\p{P}+", "<$0>", PcreOptions.None, PcreSubstituteOptions.SubstituteGlobal);
Assert.That(result, Is.EqualTo("hello<,> world<!!!>"));
```

- Partial matching:

```C#
var regex = new PcreRegex(@"(?<=abc)123");
var match = regex.Match("xyzabc12", PcreMatchOptions.PartialSoft);
// result: match.IsPartialMatch == true
```

- Validate a JSON string:

```C#
const string jsonPattern = """
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
            " (?: [^"\\\p{Cc}]++ | \\u[0-9A-Fa-f]{4} | \\ ["\\/bfnrt] )* "
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
    """;

var regex = new PcreRegex(jsonPattern, PcreOptions.IgnorePatternWhitespace);

const string subject = """
    {
        "hello": "world",
        "numbers": [4, 8, 15, 16, 23, 42],
        "foo": null,
        "bar": -2.42e+17,
        "baz": true
    }
    """;

var isValidJson = regex.IsMatch(subject);
// result: true
```
