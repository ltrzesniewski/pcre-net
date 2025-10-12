
#include "pcrenet.h"

PCRENET_EXPORT(int32_t, get_error_message)(const int32_t error_code, PCRE2_UCHAR* error_buffer, const uint32_t buffer_size)
{
    return pcre2_get_error_message(error_code, error_buffer, buffer_size);
}

PCRENET_EXPORT(int32_t, pattern_info)(const pcre2_code* code, const uint32_t key, void* data)
{
    return pcre2_pattern_info(code, key, data);
}

PCRENET_EXPORT(int32_t, config)(const uint32_t key, void* data)
{
    return pcre2_config(key, data);
}

// ReSharper disable once CppParameterNeverUsed
static int get_callout_count_handler(pcre2_callout_enumerate_block* block, void* data)
{
    uint32_t* count = data;
    ++*count;
    return 0;
}

PCRENET_EXPORT(uint32_t, get_callout_count)(const pcre2_code* code)
{
    uint32_t count = 0;
    pcre2_callout_enumerate(code, &get_callout_count_handler, &count);
    return count;
}

// ReSharper disable once CppParameterMayBeConstPtrOrRef
static int get_callouts_handler(pcre2_callout_enumerate_block* block, void* data)
{
    pcre2_callout_enumerate_block** blocks = data;
    **blocks = *block;
    ++*blocks;
    return 0;
}

PCRENET_EXPORT(void, get_callouts)(const pcre2_code* code, pcre2_callout_enumerate_block* blocks)
{
    pcre2_callout_enumerate(code, &get_callouts_handler, &blocks);
}

PCRENET_EXPORT(pcre2_jit_stack*, jit_stack_create)(const uint32_t start_size, const uint32_t max_size)
{
    return pcre2_jit_stack_create(start_size, max_size, NULL);
}

PCRENET_EXPORT(void, jit_stack_free)(pcre2_jit_stack* stack)
{
    pcre2_jit_stack_free(stack);
}
