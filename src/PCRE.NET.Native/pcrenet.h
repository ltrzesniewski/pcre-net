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

// Common types and functions

typedef struct
{
    uint32_t match_limit;
    uint32_t depth_limit;
    uint32_t heap_limit;
    uint32_t offset_limit;
    pcre2_jit_stack* jit_stack;
} match_settings;

void apply_settings(const match_settings* settings, pcre2_match_context* context);
