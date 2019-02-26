#pragma once

#include <cstdint>

#include "../PCRE/src/config.h"
#include "../PCRE/src/pcre2.h"

#if __GNUC__
#   define PCRENET_EXPORT(type, name) extern "C" __attribute__((visibility("default"))) type pcrenet_##name
#else
#   define PCRENET_EXPORT(type, name) extern "C" __declspec(dllexport) type __cdecl pcrenet_##name
#endif
