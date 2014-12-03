
PCRE.NET
=========

**Perl Compatible Regular Expressions for .NET**

[NuGet package](https://www.nuget.org/packages/PCRE.NET)

PCRE.NET is a .NET wrapper for the [PCRE library](http://www.pcre.org/). The goal of this project is to bring most of PCRE's features for use from .NET applications with as little overhead as possible.

**Status:** Version **0.2** is released, the most important features are available. Feedback is welcome.  
As it is still at major version 0, the API may change anytime.

The currently wrapped PCRE version is **8.36**.

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

Library highlights:

- No marshaling - uses a mixed mode assembly to maximize performance
- Support for studied patterns
- Support for compiled patterns (x86/x64 JIT)
- Lazy evaluation whenever possible (for instance `PcreRegex.Matches` returns `IEnumerable<PcreMatch>`)
- The API is similar to .NET's `System.Text.RegularExpressions`
- Callout support
- Mark retrieval support

## To do ##

- Expose more PCRE features:
  - DFA matching
  - Partial matching
- Include more tests from the PCRE test suite
- Documentation
- XML doc comments
