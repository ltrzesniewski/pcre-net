#include "pcrenet.h"

typedef int (*substitute_callout_fn)(pcre2_substitute_callout_block*, void*);

typedef struct
{
    pcre2_code* code;
    uint16_t* subject;
    uint32_t subject_length;
    uint32_t start_index;
    uint32_t additional_options;
    match_settings settings;
    uint16_t* replacement;
    uint32_t replacement_length;
    uint16_t* buffer;
    uint32_t buffer_length;
    substitute_callout_fn callout;
    void* callout_data;
} pcrenet_substitute_input;

typedef struct
{
    int32_t result_code;
    uint16_t* output;
    size_t output_length;
    uint8_t output_on_heap;
} pcrenet_substitute_result;

typedef struct
{
    const pcrenet_substitute_input* input;
} substitute_callout_data;

static void free_result_memory(pcrenet_substitute_result* result)
{
    if (!result)
        return;

    if (result->output && result->output_on_heap)
        free(result->output);

    result->output = NULL;
    result->output_length = 0;
    result->output_on_heap = 0;
}

static int substitute_callout_handler(pcre2_substitute_callout_block* block, void* data_ptr)
{
    const substitute_callout_data* data = (substitute_callout_data*)data_ptr;
    return data->input->callout(block, data->input->callout_data);
}

static int call_substitute(const pcrenet_substitute_input* input,
                           const uint32_t additional_options,
                           PCRE2_UCHAR* output,
                           PCRE2_SIZE* output_length,
                           pcre2_match_data* match_data,
                           pcre2_match_context* match_context)
{
    return pcre2_substitute(
        input->code,
        input->subject,
        input->subject_length,
        input->start_index,
        input->additional_options | additional_options,
        match_data,
        match_context,
        input->replacement,
        input->replacement_length,
        output,
        output_length
    );
}

static void substitute_simple(const pcrenet_substitute_input* input,
                              pcrenet_substitute_result* result,
                              pcre2_match_data* match_data,
                              pcre2_match_context* match_context)
{
    // Try to substitute in one or two passes max using the PCRE2_SUBSTITUTE_OVERFLOW_LENGTH option

    result->output = input->buffer;
    result->output_length = 0;
    result->output_on_heap = 0;

    PCRE2_SIZE output_length = input->buffer_length;

    result->result_code = call_substitute(
        input,
        PCRE2_SUBSTITUTE_OVERFLOW_LENGTH,
        result->output,
        &output_length,
        match_data,
        match_context
    );

    // Success on first pass
    if (result->result_code >= 0)
    {
        result->output_length = output_length;
        return;
    }

    // Second pass required
    if (result->result_code == PCRE2_ERROR_NOMEMORY)
    {
        result->output = malloc(output_length * sizeof(PCRE2_UCHAR));

        if (!result->output)
            return;

        result->output_on_heap = 1;

        result->result_code = call_substitute(
            input,
            PCRE2_NO_UTF_CHECK,
            result->output,
            &output_length,
            match_data,
            match_context
        );

        // Success on second pass
        if (result->result_code >= 0)
        {
            result->output_length = output_length;
            return;
        }
    }

    // Error
    free_result_memory(result);
}

static void substitute_with_callout(const pcrenet_substitute_input* input,
                                    pcrenet_substitute_result* result,
                                    pcre2_match_data* match_data,
                                    pcre2_match_context* match_context)
{
    result->output = input->buffer;
    result->output_length = 0;
    result->output_on_heap = 0;

    // Available buffer size, in characters
    PCRE2_SIZE buffer_length = input->buffer_length;

    // Same as buffer_length, but will be overwritten by pcre2_substitute
    PCRE2_SIZE output_length = buffer_length;

    substitute_callout_data callout_data = {
        .input = input
    };

    pcre2_set_substitute_callout(match_context, &substitute_callout_handler, &callout_data);

    while (1)
    {
        result->result_code = call_substitute(
            input,
            0,
            result->output,
            &output_length,
            match_data,
            match_context
        );

        // Success
        if (result->result_code >= 0)
        {
            result->output_length = output_length;
            return;
        }

        // Output buffer is too small
        if (result->result_code == PCRE2_ERROR_NOMEMORY)
        {
            buffer_length *= 2;

            if (!result->output_on_heap)
            {
                result->output = malloc(buffer_length * sizeof(PCRE2_UCHAR));

                if (!result->output)
                    return;

                result->output_on_heap = 1;
            }
            else
            {
                uint16_t* new_buffer = realloc(result->output, buffer_length * sizeof(PCRE2_UCHAR));

                if (!new_buffer)
                {
                    free_result_memory(result);
                    return;
                }

                result->output = new_buffer;
            }

            output_length = buffer_length;
            continue;
        }

        // Error
        free_result_memory(result);
        return;
    }
}

PCRENET_EXPORT(void, substitute)(const pcrenet_substitute_input* input, pcrenet_substitute_result* result)
{
    pcre2_match_data* match_data = pcre2_match_data_create_from_pattern(input->code, NULL);
    pcre2_match_context* match_context = pcre2_match_context_create(NULL);

    apply_settings(&input->settings, match_context);

    if (!input->callout)
        substitute_simple(input, result, match_data, match_context);
    else
        substitute_with_callout(input, result, match_data, match_context);

    pcre2_match_context_free(match_context);
    pcre2_match_data_free(match_data);
}

PCRENET_EXPORT(void, substitute_result_free)(pcrenet_substitute_result* result)
{
    free_result_memory(result);
}
