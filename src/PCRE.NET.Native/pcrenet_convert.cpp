
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

PCRENET_EXPORT(int32_t, convert)(convert_input* input, convert_result* result)
{
    const auto context = pcre2_convert_context_create(nullptr);

    pcre2_set_glob_escape(context, input->glob_escape);
    pcre2_set_glob_separator(context, input->glob_separator);

    PCRE2_UCHAR* buffer = nullptr;
    PCRE2_SIZE bufferLength = 0;

    const auto errorCode = pcre2_pattern_convert(
        input->pattern,
        input->pattern_length,
        input->options,
        &buffer,
        &bufferLength,
        context
    );

    result->output = buffer;
    result->output_length = bufferLength;

    pcre2_convert_context_free(context);
    return errorCode;
}

PCRENET_EXPORT(void, convert_result_free)(uint16_t* str)
{
    if (str)
        pcre2_converted_pattern_free(str);
}
