
#include "pcrenet.h"

typedef struct
{
    PCRE2_SPTR pattern;
    uint32_t pattern_length;
    uint32_t options;
    uint32_t glob_escape;
    uint32_t glob_separator;
} convert_input;

typedef struct
{
    PCRE2_UCHAR* output;
    uint32_t output_length;
} convert_result;

PCRENET_EXPORT(int32_t, convert)(const convert_input* input, convert_result* result)
{
    pcre2_convert_context* context = pcre2_convert_context_create(NULL);

    pcre2_set_glob_escape(context, input->glob_escape);
    pcre2_set_glob_separator(context, input->glob_separator);

    PCRE2_UCHAR* buffer = NULL;
    PCRE2_SIZE buffer_length = 0;

    const int error_code = pcre2_pattern_convert(
        input->pattern,
        input->pattern_length,
        input->options,
        &buffer,
        &buffer_length,
        context
    );

    result->output = buffer;
    result->output_length = (uint32_t)buffer_length;

    pcre2_convert_context_free(context);
    return error_code;
}

PCRENET_EXPORT(void, convert_result_free)(PCRE2_UCHAR* str)
{
    if (str)
        pcre2_converted_pattern_free(str);
}
