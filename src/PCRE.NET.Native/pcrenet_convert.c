
#include "pcrenet.h"

typedef struct
{
    uint16_t* pattern;
    uint32_t pattern_length;
    uint32_t options;
    uint32_t glob_escape;
    uint32_t glob_separator;
} convert_input;

typedef struct
{
    uint16_t* output;
    uint32_t output_length;
} convert_result;

PCRENET_EXPORT(int32_t, convert)(const convert_input* input, convert_result* result)
{
    pcre2_convert_context_16* context = pcre2_convert_context_create_16(NULL);

    pcre2_set_glob_escape_16(context, input->glob_escape);
    pcre2_set_glob_separator_16(context, input->glob_separator);

    PCRE2_UCHAR16* buffer = NULL;
    PCRE2_SIZE buffer_length = 0;

    const int error_code = pcre2_pattern_convert_16(
        input->pattern,
        input->pattern_length,
        input->options,
        &buffer,
        &buffer_length,
        context
    );

    result->output = buffer;
    result->output_length = (uint32_t)buffer_length;

    pcre2_convert_context_free_16(context);
    return error_code;
}

PCRENET_EXPORT(void, convert_result_free)(uint16_t* str)
{
    if (str)
        pcre2_converted_pattern_free_16(str);
}
