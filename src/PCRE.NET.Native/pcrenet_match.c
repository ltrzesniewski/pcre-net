
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
    pcre2_code* code;
    uint16_t* subject;
    uint32_t subject_length;
    uint32_t start_index;
    uint32_t additional_options;
    match_settings settings;
    uint32_t* output_vector;
    callout_fn callout;
    void* callout_data;
} pcrenet_match_input;

typedef struct
{
    pcre2_code* code;
    uint16_t* subject;
    uint32_t subject_length;
    uint32_t start_index;
    uint32_t additional_options;
    uint32_t* output_vector;
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

        for (uint32_t i = 0; i < itemCount; ++i)
            input->output_vector[i] = (uint32_t)oVector[i];
    }

    result->mark = pcre2_get_mark(matchData);

    pcre2_match_context_free(context);
    pcre2_match_data_free(matchData);
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

        for (uint32_t i = 0; i < itemCount; ++i)
            input->output_vector[i] = (uint32_t)oVector[i];
    }

    free(workspace);
    pcre2_match_context_free(context);
    pcre2_match_data_free(matchData);
}
