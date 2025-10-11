#include "pcrenet.h"

c_static_assert(sizeof(uint32_t) <= sizeof(PCRE2_SIZE), "Parameter size must fit into PCRE2_SIZE");

typedef struct
{
    uint16_t* pattern;
    uint32_t pattern_length;
    uint32_t flags;
    uint32_t flags_jit;
    uint32_t new_line;
    uint32_t bsr;
    uint32_t parens_nest_limit;
    uint32_t max_pattern_length;
    uint32_t compile_extra_options;
    uint32_t max_var_lookbehind;
    uint32_t max_pattern_compiled_length;
    uint32_t optimization_directives_count;
    uint32_t* optimization_directives;
} pcrenet_compile_input;

typedef struct
{
    pcre2_code_16* code;
    int32_t error_code;
    uint32_t error_offset;
    uint32_t capture_count;
    uint32_t name_count;
    uint32_t name_entry_size;
    uint16_t* name_entry_table;
} pcrenet_compile_result;

PCRENET_EXPORT(void, compile)(const pcrenet_compile_input* input, pcrenet_compile_result* result)
{
    pcre2_compile_context_16* context = pcre2_compile_context_create_16(NULL);

    if (input->new_line)
        pcre2_set_newline_16(context, input->new_line);

    if (input->bsr)
        pcre2_set_bsr_16(context, input->bsr);

    if (input->parens_nest_limit)
        pcre2_set_parens_nest_limit_16(context, input->parens_nest_limit);

    if (input->max_pattern_length)
        pcre2_set_max_pattern_length_16(context, input->max_pattern_length);

    if (input->max_pattern_compiled_length)
        pcre2_set_max_pattern_compiled_length_16(context, input->max_pattern_compiled_length);

    if (input->max_var_lookbehind != MAX_VARLOOKBEHIND)
        pcre2_set_max_varlookbehind_16(context, input->max_var_lookbehind);

    if (input->compile_extra_options)
        pcre2_set_compile_extra_options_16(context, input->compile_extra_options);

    for (uint32_t i = 0; i < input->optimization_directives_count; ++i)
        pcre2_set_optimize_16(context, input->optimization_directives[i]);

    int error_code;
    PCRE2_SIZE error_offset;

    result->code = pcre2_compile_16(
        input->pattern,
        input->pattern_length,
        input->flags,
        &error_code,
        &error_offset,
        context
    );

    if (result->code)
    {
        result->error_code = 0;

        if (input->flags_jit)
            pcre2_jit_compile_16(result->code, input->flags_jit);

        pcre2_pattern_info_16(result->code, PCRE2_INFO_CAPTURECOUNT, &result->capture_count);
        pcre2_pattern_info_16(result->code, PCRE2_INFO_NAMECOUNT, &result->name_count);
        pcre2_pattern_info_16(result->code, PCRE2_INFO_NAMEENTRYSIZE, &result->name_entry_size);
        pcre2_pattern_info_16(result->code, PCRE2_INFO_NAMETABLE, &result->name_entry_table);
    }
    else
    {
        result->error_code = error_code;
        result->error_offset = (uint32_t)error_offset;
    }

    pcre2_compile_context_free_16(context);
}

PCRENET_EXPORT(void, code_free)(pcre2_code_16* code)
{
    if (code)
        pcre2_code_free_16(code);
}
