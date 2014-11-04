
PCRE.NET
=========

**Perl Compatible Regular Expressions for .NET**

PCRE.NET is a .NET wrapper for the [PCRE library](http://www.pcre.org/). The goal of this project is to bring most of PCRE's features for use from .NET applications with as little overhead as possible.

**Status:** In development, but the most important features are available.  
As it is still at version 0, the API may change anytime.

The currently wrapped PCRE version is **8.36**.

## Features ##

The base regex operations are supported:

- NFA matching and substring extraction
  - `PcreRegex.Matches`
  - `PcreRegex.Match`
  - `PcreRegex.IsMatch`
- Matched string replacement: `PcreRegex.Replace`
  - Callbacks: `Func<PcreMatch, string>`
  - Replacement strings with placeholders: ``$n ${name} $& $_ $` $' $+``
- String splitting on matches: `PcreRegex.Split`

Library highlights:

- No marshaling - uses a mixed mode assembly to maximize performance
- Support for studied patterns
- Support for compiled patterns (x86/x64 JIT)
- Lazy evaluation whenever possible (for instance `PcreRegex.Matches` returns `IEnumerable<PcreMatch>`)

## To do ##

- AnyCPU (x86/x64) build
- NuGet
- Expose more PCRE features
- DFA matching
- Partial matching
- Run the PCRE test suite
- Documentation
- XML doc comments

And *maybe*:

- Use of the JIT fast path (with mandatory custom JIT stack)
- Callouts
- Stackless variant
