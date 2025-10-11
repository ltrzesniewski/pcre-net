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
    PCRE.NET.Native/wrappers/pcre2_auto_possess.utf8.c
    PCRE.NET.Native/wrappers/pcre2_auto_possess.utf16.c
    PCRE.NET.Native/wrappers/pcre2_chartables.utf8.c
    PCRE.NET.Native/wrappers/pcre2_chartables.utf16.c
    PCRE.NET.Native/wrappers/pcre2_chkdint.utf8.c
    PCRE.NET.Native/wrappers/pcre2_chkdint.utf16.c
    PCRE.NET.Native/wrappers/pcre2_compile.utf8.c
    PCRE.NET.Native/wrappers/pcre2_compile.utf16.c
    PCRE.NET.Native/wrappers/pcre2_compile_class.utf8.c
    PCRE.NET.Native/wrappers/pcre2_compile_class.utf16.c
    PCRE.NET.Native/wrappers/pcre2_config.utf8.c
    PCRE.NET.Native/wrappers/pcre2_config.utf16.c
    PCRE.NET.Native/wrappers/pcre2_context.utf8.c
    PCRE.NET.Native/wrappers/pcre2_context.utf16.c
    PCRE.NET.Native/wrappers/pcre2_convert.utf8.c
    PCRE.NET.Native/wrappers/pcre2_convert.utf16.c
    PCRE.NET.Native/wrappers/pcre2_dfa_match.utf8.c
    PCRE.NET.Native/wrappers/pcre2_dfa_match.utf16.c
    PCRE.NET.Native/wrappers/pcre2_error.utf8.c
    PCRE.NET.Native/wrappers/pcre2_error.utf16.c
    PCRE.NET.Native/wrappers/pcre2_extuni.utf8.c
    PCRE.NET.Native/wrappers/pcre2_extuni.utf16.c
    PCRE.NET.Native/wrappers/pcre2_find_bracket.utf8.c
    PCRE.NET.Native/wrappers/pcre2_find_bracket.utf16.c
    PCRE.NET.Native/wrappers/pcre2_jit_compile.utf8.c
    PCRE.NET.Native/wrappers/pcre2_jit_compile.utf16.c
    PCRE.NET.Native/wrappers/pcre2_maketables.utf8.c
    PCRE.NET.Native/wrappers/pcre2_maketables.utf16.c
    PCRE.NET.Native/wrappers/pcre2_match.utf8.c
    PCRE.NET.Native/wrappers/pcre2_match.utf16.c
    PCRE.NET.Native/wrappers/pcre2_match_data.utf8.c
    PCRE.NET.Native/wrappers/pcre2_match_data.utf16.c
    PCRE.NET.Native/wrappers/pcre2_newline.utf8.c
    PCRE.NET.Native/wrappers/pcre2_newline.utf16.c
    PCRE.NET.Native/wrappers/pcre2_ord2utf.utf8.c
    PCRE.NET.Native/wrappers/pcre2_ord2utf.utf16.c
    PCRE.NET.Native/wrappers/pcre2_pattern_info.utf8.c
    PCRE.NET.Native/wrappers/pcre2_pattern_info.utf16.c
    PCRE.NET.Native/wrappers/pcre2_script_run.utf8.c
    PCRE.NET.Native/wrappers/pcre2_script_run.utf16.c
    PCRE.NET.Native/wrappers/pcre2_string_utils.utf8.c
    PCRE.NET.Native/wrappers/pcre2_string_utils.utf16.c
    PCRE.NET.Native/wrappers/pcre2_study.utf8.c
    PCRE.NET.Native/wrappers/pcre2_study.utf16.c
    PCRE.NET.Native/wrappers/pcre2_substitute.utf8.c
    PCRE.NET.Native/wrappers/pcre2_substitute.utf16.c
    PCRE.NET.Native/wrappers/pcre2_substring.utf8.c
    PCRE.NET.Native/wrappers/pcre2_substring.utf16.c
    PCRE.NET.Native/wrappers/pcre2_tables.utf8.c
    PCRE.NET.Native/wrappers/pcre2_tables.utf16.c
    PCRE.NET.Native/wrappers/pcre2_ucd.utf8.c
    PCRE.NET.Native/wrappers/pcre2_ucd.utf16.c
    PCRE.NET.Native/wrappers/pcre2_valid_utf.utf8.c
    PCRE.NET.Native/wrappers/pcre2_valid_utf.utf16.c
    PCRE.NET.Native/wrappers/pcre2_xclass.utf8.c
    PCRE.NET.Native/wrappers/pcre2_xclass.utf16.c
    PCRE.NET.Native/pcrenet.h
    PCRE.NET.Native/pcrenet_compile.c
    PCRE.NET.Native/pcrenet_convert.c
    PCRE.NET.Native/pcrenet_info.c
    PCRE.NET.Native/pcrenet_match.c
    PCRE.NET.Native/pcrenet_substitute.c
)
