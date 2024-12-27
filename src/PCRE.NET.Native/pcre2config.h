/*
 * This file is the PCRE2 build config as used in PCRE.NET,
 * it is meant to be included in PCRE2's config.h
 */

/* ReSharper disable All */

#define HAVE_ASSERT_H 1
#define HAVE_INTTYPES_H 1
#define HAVE_LIMITS_H 1
#define HAVE_MEMMOVE 1
#define HAVE_STDINT_H 1
#define HAVE_STDIO_H 1
#define HAVE_STDLIB_H 1
#define HAVE_STRING_H 1
#define HAVE_SYS_STAT_H 1
#define HAVE_SYS_TYPES_H 1
#define HAVE_WCHAR_H 1

#define NEWLINE_DEFAULT 5

#define PCRE2_STATIC 1
#define STDC_HEADERS 1
#define SUPPORT_JIT 1
#define SUPPORT_UNICODE 1

#if _MSC_VER

#define HAVE_BUILTIN_ASSUME
#define HAVE_WINDOWS_H 1
#define _CRT_SECURE_NO_WARNINGS

#elif __GNUC__

#define HAVE_BUILTIN_MUL_OVERFLOW
#define HAVE_BUILTIN_UNREACHABLE

#elif __clang__

#define HAVE_ATTRIBUTE_UNINITIALIZED
#define HAVE_BUILTIN_UNREACHABLE

#else

#error "Unexpected compiler"

#endif
