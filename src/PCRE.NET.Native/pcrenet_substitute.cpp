
#include "pcrenet_match.h"
#include <limits>

typedef struct
{
    pcrenet_match_input* match_input;
    uint16_t* replacement;
    uint32_t replacement_length;
} pcrenet_substitute_input;

typedef struct
{
    int32_t result_code;
    uint16_t* result;
    uint32_t result_length;
} pcrenet_substitute_result;

PCRENET_EXPORT(void, substitute)(const pcrenet_substitute_input* input, pcrenet_substitute_result* result)
{
    callout_stack_data calloutStackData;

    const auto matchInput = input->match_input;
    const auto context = matchInput->create_match_context(calloutStackData);
    const auto matchData = pcre2_match_data_create_from_pattern(matchInput->code, nullptr);

    PCRE2_SIZE outputBufferSize = 0;

    result->result_code = pcre2_substitute(
        matchInput->code,
        matchInput->subject,
        matchInput->subject_length,
        matchInput->start_index,
        matchInput->additional_options | PCRE2_SUBSTITUTE_OVERFLOW_LENGTH,
        matchData,
        context,
        input->replacement,
        input->replacement_length,
        nullptr,
        &outputBufferSize
    );

    if (result->result_code == PCRE2_ERROR_NOMEMORY && outputBufferSize < std::numeric_limits<int32_t>::max())
    {
        result->result = new uint16_t[outputBufferSize];

        result->result_code = pcre2_substitute(
            matchInput->code,
            matchInput->subject,
            matchInput->subject_length,
            matchInput->start_index,
            matchInput->additional_options,
            matchData,
            context,
            input->replacement,
            input->replacement_length,
            result->result,
            &outputBufferSize
        );

        result->result_length = result->result_code >= 0 ? outputBufferSize : 0;
    }

    pcre2_match_data_free(matchData);
    pcre2_match_context_free(context);
}

PCRENET_EXPORT(void, substitute_result_free)(pcrenet_substitute_result* result)
{
    if (!result)
        return;

    if (result->result)
    {
        delete[] result->result;
        result->result = nullptr;
        result->result_length = 0;
    }
}
