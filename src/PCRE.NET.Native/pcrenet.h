#pragma once

#include <stdint.h>
#include <assert.h>

#include "../PCRE/src/config.h"
#include "../PCRE/src/pcre2.h"

#if __GNUC__
#   define PCRENET_EXPORT(type, name) __attribute__((visibility("default"))) type pcrenet_##name
#else
#   define PCRENET_EXPORT(type, name) __declspec(dllexport) type __cdecl pcrenet_##name
#endif

#ifndef __has_extension
#define __has_extension(...) 0
#endif

#ifdef static_assert
#define c_static_assert(e, msg) static_assert((e), msg)
#elif __has_extension(c_static_assert)
#define c_static_assert(e, msg) _Static_assert((e), msg)
#else
#define c_static_assert(e, msg) typedef char __c_static_assert__[(e)?1:-1]
#endif
