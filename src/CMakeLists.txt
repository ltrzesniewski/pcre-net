cmake_minimum_required(VERSION 3.15 FATAL_ERROR)
project(PCRE.NET.Native C)

set(CMAKE_C_STANDARD 99)
set(CMAKE_C_STANDARD_REQUIRED TRUE)
set(CMAKE_C_VISIBILITY_PRESET hidden)

set(CMAKE_SHARED_LIBRARY_PREFIX "")
set(CMAKE_SKIP_INSTALL_RULES true)

if(CMAKE_BUILD_TYPE STREQUAL "Release")
    set(CMAKE_INTERPROCEDURAL_OPTIMIZATION true)
endif()

if(CMAKE_C_COMPILER_ID STREQUAL "GNU")
    string(APPEND CMAKE_SHARED_LINKER_FLAGS " -Wl,--no-undefined")
endif()

include_directories(PCRE/src)
include_directories(PCRE.NET.Native)

add_library(PCRE.NET.Native SHARED
    PCRE.NET.Native/compile/pcre2_auto_possess.8bit.c
    PCRE.NET.Native/compile/pcre2_auto_possess.16bit.c
    PCRE.NET.Native/compile/pcre2_chartables.8bit.c
    PCRE.NET.Native/compile/pcre2_chartables.16bit.c
    PCRE.NET.Native/compile/pcre2_chkdint.8bit.c
    PCRE.NET.Native/compile/pcre2_chkdint.16bit.c
    PCRE.NET.Native/compile/pcre2_compile.8bit.c
    PCRE.NET.Native/compile/pcre2_compile.16bit.c
    PCRE.NET.Native/compile/pcre2_compile_class.8bit.c
    PCRE.NET.Native/compile/pcre2_compile_class.16bit.c
    PCRE.NET.Native/compile/pcre2_config.8bit.c
    PCRE.NET.Native/compile/pcre2_config.16bit.c
    PCRE.NET.Native/compile/pcre2_context.8bit.c
    PCRE.NET.Native/compile/pcre2_context.16bit.c
    PCRE.NET.Native/compile/pcre2_convert.8bit.c
    PCRE.NET.Native/compile/pcre2_convert.16bit.c
    PCRE.NET.Native/compile/pcre2_dfa_match.8bit.c
    PCRE.NET.Native/compile/pcre2_dfa_match.16bit.c
    PCRE.NET.Native/compile/pcre2_error.8bit.c
    PCRE.NET.Native/compile/pcre2_error.16bit.c
    PCRE.NET.Native/compile/pcre2_extuni.8bit.c
    PCRE.NET.Native/compile/pcre2_extuni.16bit.c
    PCRE.NET.Native/compile/pcre2_find_bracket.8bit.c
    PCRE.NET.Native/compile/pcre2_find_bracket.16bit.c
    PCRE.NET.Native/compile/pcre2_jit_compile.8bit.c
    PCRE.NET.Native/compile/pcre2_jit_compile.16bit.c
    PCRE.NET.Native/compile/pcre2_maketables.8bit.c
    PCRE.NET.Native/compile/pcre2_maketables.16bit.c
    PCRE.NET.Native/compile/pcre2_match.8bit.c
    PCRE.NET.Native/compile/pcre2_match.16bit.c
    PCRE.NET.Native/compile/pcre2_match_data.8bit.c
    PCRE.NET.Native/compile/pcre2_match_data.16bit.c
    PCRE.NET.Native/compile/pcre2_newline.8bit.c
    PCRE.NET.Native/compile/pcre2_newline.16bit.c
    PCRE.NET.Native/compile/pcre2_ord2utf.8bit.c
    PCRE.NET.Native/compile/pcre2_ord2utf.16bit.c
    PCRE.NET.Native/compile/pcre2_pattern_info.8bit.c
    PCRE.NET.Native/compile/pcre2_pattern_info.16bit.c
    PCRE.NET.Native/compile/pcre2_script_run.8bit.c
    PCRE.NET.Native/compile/pcre2_script_run.16bit.c
    PCRE.NET.Native/compile/pcre2_string_utils.8bit.c
    PCRE.NET.Native/compile/pcre2_string_utils.16bit.c
    PCRE.NET.Native/compile/pcre2_study.8bit.c
    PCRE.NET.Native/compile/pcre2_study.16bit.c
    PCRE.NET.Native/compile/pcre2_substitute.8bit.c
    PCRE.NET.Native/compile/pcre2_substitute.16bit.c
    PCRE.NET.Native/compile/pcre2_substring.8bit.c
    PCRE.NET.Native/compile/pcre2_substring.16bit.c
    PCRE.NET.Native/compile/pcre2_tables.8bit.c
    PCRE.NET.Native/compile/pcre2_tables.16bit.c
    PCRE.NET.Native/compile/pcre2_ucd.8bit.c
    PCRE.NET.Native/compile/pcre2_ucd.16bit.c
    PCRE.NET.Native/compile/pcre2_valid_utf.8bit.c
    PCRE.NET.Native/compile/pcre2_valid_utf.16bit.c
    PCRE.NET.Native/compile/pcre2_xclass.8bit.c
    PCRE.NET.Native/compile/pcre2_xclass.16bit.c
    PCRE.NET.Native/compile/pcrenet_compile.8bit.c
    PCRE.NET.Native/compile/pcrenet_compile.16bit.c
    PCRE.NET.Native/compile/pcrenet_convert.8bit.c
    PCRE.NET.Native/compile/pcrenet_convert.16bit.c
    PCRE.NET.Native/compile/pcrenet_info.8bit.c
    PCRE.NET.Native/compile/pcrenet_info.16bit.c
    PCRE.NET.Native/compile/pcrenet_match.8bit.c
    PCRE.NET.Native/compile/pcrenet_match.16bit.c
    PCRE.NET.Native/compile/pcrenet_substitute.8bit.c
    PCRE.NET.Native/compile/pcrenet_substitute.16bit.c
)
