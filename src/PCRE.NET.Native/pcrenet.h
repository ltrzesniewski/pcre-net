#pragma once

#include <cstdint>
#include "../PCRE/src/pcre2.h"

#define PCRENET_EXPORT(type, name) extern "C" __declspec(dllexport) type __stdcall name
