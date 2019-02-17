
#include "pcrenet.h"

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
} pcrenet_match_input;

typedef struct
{
    int32_t result_code;
} pcrenet_match_result;

PCRENET_EXPORT(void, match)(const pcrenet_match_input* input, pcrenet_match_result* result)
{
    const auto matchData = pcre2_match_data_create_from_pattern(input->code, nullptr);
    const auto context = pcre2_match_context_create(nullptr);

    if (input->match_limit)
        pcre2_set_match_limit(context, input->match_limit);

    if (input->depth_limit)
        pcre2_set_depth_limit(context, input->depth_limit);

    if (input->heap_limit)
        pcre2_set_heap_limit(context, input->heap_limit);

    if (input->offset_limit)
        pcre2_set_offset_limit(context, input->offset_limit);

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

    pcre2_match_context_free(context);
    pcre2_match_data_free(matchData);
}
