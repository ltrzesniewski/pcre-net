
#include <string.h>

#include "pcrenet.h"

typedef int (*callout_fn)(pcre2_callout_block*, void*);

typedef struct
{
    uint32_t match_limit;
    uint32_t depth_limit;
    uint32_t heap_limit;
    uint32_t offset_limit;
    pcre2_jit_stack* jit_stack;
} match_settings;

typedef struct
{
    const pcre2_code* code;
    pcre2_match_data* match_data;
    pcre2_match_context* match_context;
} match_buffer;

typedef struct
{
    pcre2_code* code;
    uint16_t* subject;
    uint32_t subject_length;
    uint32_t start_index;
    uint32_t additional_options;
    match_settings settings;
    size_t* output_vector;
    callout_fn callout;
    void* callout_data;
} pcrenet_match_input;

typedef struct
{
    match_buffer* buffer;
    uint16_t* subject;
    uint32_t subject_length;
    uint32_t start_index;
    uint32_t additional_options;
    size_t* output_vector;
    callout_fn callout;
    void* callout_data;
} pcrenet_buffer_match_input;

typedef struct
{
    pcre2_code* code;
    uint16_t* subject;
    uint32_t subject_length;
    uint32_t start_index;
    uint32_t additional_options;
    size_t* output_vector;
    callout_fn callout;
    void* callout_data;
    uint32_t max_results;
    uint32_t workspace_size;
} pcrenet_dfa_match_input;

typedef struct
{
    int32_t result_code;
    PCRE2_SPTR16 mark;
} pcrenet_match_result;

typedef struct
{
    callout_fn callout;
    void* data;
} callout_data;

static int callout_handler(pcre2_callout_block* block, void* data)
{
    const callout_data* typedData = (callout_data*)data;
    return typedData->callout(block, typedData->data);
}

static void apply_settings(const match_settings* settings, pcre2_match_context* context)
{
    if (settings->match_limit)
        pcre2_set_match_limit(context, settings->match_limit);

    if (settings->depth_limit)
        pcre2_set_depth_limit(context, settings->depth_limit);

    if (settings->heap_limit)
        pcre2_set_heap_limit(context, settings->heap_limit);

    if (settings->offset_limit)
        pcre2_set_offset_limit(context, settings->offset_limit);

    if (settings->jit_stack)
        pcre2_jit_stack_assign(context, NULL, settings->jit_stack);
}

PCRENET_EXPORT(void, match)(const pcrenet_match_input* input, pcrenet_match_result* result)
{
    pcre2_match_data* matchData = pcre2_match_data_create_from_pattern(input->code, NULL);
    pcre2_match_context* context = pcre2_match_context_create(NULL);
    callout_data callout;

    apply_settings(&input->settings, context);

    if (input->callout)
    {
        callout.callout = input->callout;
        callout.data = input->callout_data;
        pcre2_set_callout(context, &callout_handler, &callout);
    }

    result->result_code = pcre2_match(
        input->code,
        input->subject,
        input->subject_length,
        input->start_index,
        input->additional_options,
        matchData,
        context
    );

    if (input->output_vector)
    {
        PCRE2_SIZE* oVector = pcre2_get_ovector_pointer(matchData);
        const uint32_t itemCount = pcre2_get_ovector_count(matchData) * 2;
        memcpy(input->output_vector, oVector, itemCount * sizeof(PCRE2_SIZE));
    }

    result->mark = pcre2_get_mark(matchData);

    pcre2_match_context_free(context);
    pcre2_match_data_free(matchData);
}

PCRENET_EXPORT(void, buffer_match)(const pcrenet_buffer_match_input* input, pcrenet_match_result* result)
{
    callout_data callout;

    if (input->callout)
    {
        callout.callout = input->callout;
        callout.data = input->callout_data;
        pcre2_set_callout(input->buffer->match_context, &callout_handler, &callout);
    }
    else
    {
        pcre2_set_callout(input->buffer->match_context, NULL, NULL);
    }

    result->result_code = pcre2_match(
        input->buffer->code,
        input->subject,
        input->subject_length,
        input->start_index,
        input->additional_options,
        input->buffer->match_data,
        input->buffer->match_context
    );

    if (input->output_vector)
    {
        PCRE2_SIZE* oVector = pcre2_get_ovector_pointer(input->buffer->match_data);
        const uint32_t itemCount = pcre2_get_ovector_count(input->buffer->match_data) * 2;
        memcpy(input->output_vector, oVector, itemCount * sizeof(PCRE2_SIZE));
    }

    result->mark = pcre2_get_mark(input->buffer->match_data);
}


PCRENET_EXPORT(void, dfa_match)(const pcrenet_dfa_match_input* input, pcrenet_match_result* result)
{
    pcre2_match_data* matchData = pcre2_match_data_create(input->max_results, NULL);
    pcre2_match_context* context = pcre2_match_context_create(NULL);
    callout_data callout;

    if (input->callout)
    {
        callout.callout = input->callout;
        callout.data = input->callout_data;
        pcre2_set_callout(context, &callout_handler, &callout);
    }

    const int workspaceSize = 20u > input->workspace_size ? 20u : input->workspace_size;
    int* workspace = malloc(workspaceSize * sizeof(int));;

    result->result_code = pcre2_dfa_match(
        input->code,
        input->subject,
        input->subject_length,
        input->start_index,
        input->additional_options,
        matchData,
        context,
        workspace,
        workspaceSize
    );

    if (input->output_vector)
    {
        PCRE2_SIZE* oVector = pcre2_get_ovector_pointer(matchData);
        const uint32_t itemCount = pcre2_get_ovector_count(matchData) * 2;
        memcpy(input->output_vector, oVector, itemCount * sizeof(PCRE2_SIZE));
    }

    free(workspace);
    pcre2_match_context_free(context);
    pcre2_match_data_free(matchData);
}

PCRENET_EXPORT(match_buffer*, create_match_buffer)(const pcre2_code* code, const match_settings* settings)
{
    if (!code)
        return NULL;

    match_buffer* buffer = malloc(sizeof(match_buffer));
    if (!buffer)
        return NULL;

    buffer->code = code;
    buffer->match_data = pcre2_match_data_create_from_pattern(code, NULL);
    buffer->match_context = pcre2_match_context_create(NULL);

    if (settings)
        apply_settings(settings, buffer->match_context);

    return buffer;
}

PCRENET_EXPORT(void, free_match_buffer)(match_buffer* buffer)
{
    if (!buffer)
        return;

    pcre2_match_context_free(buffer->match_context);
    pcre2_match_data_free(buffer->match_data);

    free(buffer);
}
