#include "pcrenet.h"

typedef struct
{
    pcre2_code* code;
    uint16_t* subject;
    uint32_t subject_length;
    uint32_t start_index;
    uint32_t additional_options;
    uint16_t* replacement;
    uint32_t replacement_length;
} pcrenet_substitute_input;

typedef struct
{
    int32_t result_code;
    uint16_t* output;
    uint32_t output_length;
} pcrenet_substitute_result;

PCRENET_EXPORT(void, substitute)(const pcrenet_substitute_input* input, pcrenet_substitute_result* result)
{
    pcre2_match_data* match_data = pcre2_match_data_create_from_pattern(input->code, NULL);
    pcre2_match_context* context = pcre2_match_context_create(NULL);

    PCRE2_SIZE output_length = 0;

    // Treat result as invalid
    result->output = NULL;
    result->output_length = 0;

    result->result_code = pcre2_substitute(
        input->code,
        input->subject,
        input->subject_length,
        input->start_index,
        input->additional_options | PCRE2_SUBSTITUTE_OVERFLOW_LENGTH,
        match_data,
        context,
        input->replacement,
        input->replacement_length,
        NULL,
        &output_length
    );

    if (result->result_code == PCRE2_ERROR_NOMEMORY)
    {
        result->output = malloc(output_length * sizeof(PCRE2_UCHAR));

        result->result_code = pcre2_substitute(
            input->code,
            input->subject,
            input->subject_length,
            input->start_index,
            input->additional_options | PCRE2_NO_UTF_CHECK,
            match_data,
            context,
            input->replacement,
            input->replacement_length,
            result->output,
            &output_length
        );

        result->output_length = result->result_code >= 0 ? output_length : 0;
    }

    pcre2_match_context_free(context);
    pcre2_match_data_free(match_data);
}

PCRENET_EXPORT(void, substitute_result_free)(pcrenet_substitute_result* result)
{
    if (!result)
        return;

    if (result->output)
    {
        free(result->output);
        result->output = NULL;
    }

    result->output_length = 0;
}
