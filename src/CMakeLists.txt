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
        PCRE/src/config.h
        PCRE/src/pcre2.h
        PCRE/src/pcre2_auto_possess.c
        PCRE/src/pcre2_chartables.c
        PCRE/src/pcre2_chkdint.c
        PCRE/src/pcre2_compile.c
        PCRE/src/pcre2_compile_class.c
        PCRE/src/pcre2_config.c
        PCRE/src/pcre2_context.c
        PCRE/src/pcre2_convert.c
        PCRE/src/pcre2_dfa_match.c
        PCRE/src/pcre2_error.c
        PCRE/src/pcre2_extuni.c
        PCRE/src/pcre2_find_bracket.c
        PCRE/src/pcre2_internal.h
        PCRE/src/pcre2_intmodedep.h
        PCRE/src/pcre2_jit_compile.c
        PCRE/src/pcre2_maketables.c
        PCRE/src/pcre2_match.c
        PCRE/src/pcre2_match_data.c
        PCRE/src/pcre2_newline.c
        PCRE/src/pcre2_ord2utf.c
        PCRE/src/pcre2_pattern_info.c
        PCRE/src/pcre2_script_run.c
        PCRE/src/pcre2_string_utils.c
        PCRE/src/pcre2_study.c
        PCRE/src/pcre2_substitute.c
        PCRE/src/pcre2_substring.c
        PCRE/src/pcre2_tables.c
        PCRE/src/pcre2_ucd.c
        PCRE/src/pcre2_ucp.h
        PCRE/src/pcre2_valid_utf.c
        PCRE/src/pcre2_xclass.c
        PCRE.NET.Native/pcrenet.h
        PCRE.NET.Native/pcrenet_compile.c
        PCRE.NET.Native/pcrenet_convert.c
        PCRE.NET.Native/pcrenet_info.c
        PCRE.NET.Native/pcrenet_match.c
        PCRE.NET.Native/pcrenet_substitute.c
)

add_compile_definitions(
        HAVE_CONFIG_H
        PCRE2_CODE_UNIT_WIDTH=16
)
