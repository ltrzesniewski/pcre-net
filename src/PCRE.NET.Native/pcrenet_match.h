#pragma once

#include "pcrenet.h"

typedef int (*callout_fn)(pcre2_callout_block*, void*);

typedef struct
{
    callout_fn callout;
    void* data;
} callout_stack_data;

typedef struct
{
    pcre2_code* code;
    uint16_t* subject;
    uint32_t subject_length;
    uint32_t start_index;
    uint32_t additional_options;
    uint32_t match_limit;
    uint32_t depth_limit;
    uint32_t heap_limit;
    uint32_t offset_limit;
    uint32_t* output_vector;
    callout_fn callout;
    void* callout_data;
    pcre2_jit_stack* jit_stack;

    pcre2_match_context* create_match_context(callout_stack_data& calloutStackData) const;
} pcrenet_match_input;
