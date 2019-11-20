
#include "pcrenet_match.h"
#include <memory>
#include <algorithm>

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

static int callout_handler(pcre2_callout_block* block, void* data)
{
    const auto typedData = static_cast<callout_stack_data*>(data);
    return typedData->callout(block, typedData->data);
}

pcre2_match_context* pcrenet_match_input::create_match_context(callout_stack_data& calloutStackData) const
{
    const auto context = pcre2_match_context_create(nullptr);

    if (match_limit)
        pcre2_set_match_limit(context, match_limit);

    if (depth_limit)
        pcre2_set_depth_limit(context, depth_limit);

    if (heap_limit)
        pcre2_set_heap_limit(context, heap_limit);

    if (offset_limit)
        pcre2_set_offset_limit(context, offset_limit);

    if (callout)
    {
        calloutStackData = { callout, callout_data };
        pcre2_set_callout(context, &callout_handler, &calloutStackData);
    }

    if (jit_stack)
        pcre2_jit_stack_assign(context, nullptr, jit_stack);

    return context;
}

PCRENET_EXPORT(void, match)(const pcrenet_match_input* input, pcrenet_match_result* result)
{
    callout_stack_data calloutStackData;

    const auto context = input->create_match_context(calloutStackData);
    const auto matchData = pcre2_match_data_create_from_pattern(input->code, nullptr);

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
        const auto oVector = pcre2_get_ovector_pointer(matchData);
        const auto itemCount = pcre2_get_ovector_count(matchData) * 2;

        for (uint32_t i = 0; i < itemCount; ++i)
            input->output_vector[i] = static_cast<uint32_t>(oVector[i]);
    }

    result->mark = pcre2_get_mark(matchData);

    pcre2_match_data_free(matchData);
    pcre2_match_context_free(context);
}

PCRENET_EXPORT(void, dfa_match)(const pcrenet_dfa_match_input* input, pcrenet_match_result* result)
{
    const auto matchData = pcre2_match_data_create(input->max_results, nullptr);
    const auto context = pcre2_match_context_create(nullptr);
    callout_stack_data calloutStackData;

    if (input->callout)
    {
        calloutStackData = { input->callout, input->callout_data };
        pcre2_set_callout(context, &callout_handler, &calloutStackData);
    }

    const auto workspaceSize = std::max(20u, input->workspace_size);
    auto workspace = std::make_unique<int[]>(workspaceSize);

    result->result_code = pcre2_dfa_match(
        input->code,
        input->subject,
        input->subject_length,
        input->start_index,
        input->additional_options,
        matchData,
        context,
        workspace.get(),
        workspaceSize
    );

    if (input->output_vector)
    {
        const auto oVector = pcre2_get_ovector_pointer(matchData);
        const auto itemCount = pcre2_get_ovector_count(matchData) * 2;

        for (uint32_t i = 0; i < itemCount; ++i)
            input->output_vector[i] = static_cast<uint32_t>(oVector[i]);
    }

    pcre2_match_context_free(context);
    pcre2_match_data_free(matchData);
}
