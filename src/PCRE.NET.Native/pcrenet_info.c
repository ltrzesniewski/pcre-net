
#include "pcrenet.h"

PCRENET_EXPORT(int32_t, get_error_message)(const int32_t error_code, uint16_t* error_buffer, const uint32_t buffer_size)
{
    return pcre2_get_error_message_16(error_code, error_buffer, buffer_size);
}

PCRENET_EXPORT(int32_t, pattern_info)(const pcre2_code_16* code, const uint32_t key, void* data)
{
    return pcre2_pattern_info_16(code, key, data);
}

PCRENET_EXPORT(int32_t, config)(const uint32_t key, void* data)
{
    return pcre2_config_16(key, data);
}

// ReSharper disable once CppParameterNeverUsed
static int get_callout_count_handler(pcre2_callout_enumerate_block_16* block, void* data)
{
    uint32_t* count = data;
    ++*count;
    return 0;
}

PCRENET_EXPORT(uint32_t, get_callout_count)(const pcre2_code_16* code)
{
    uint32_t count = 0;
    pcre2_callout_enumerate_16(code, &get_callout_count_handler, &count);
    return count;
}

// ReSharper disable once CppParameterMayBeConstPtrOrRef
static int get_callouts_handler(pcre2_callout_enumerate_block_16* block, void* data)
{
    pcre2_callout_enumerate_block_16** blocks = data;
    **blocks = *block;
    ++*blocks;
    return 0;
}

PCRENET_EXPORT(void, get_callouts)(const pcre2_code_16* code, pcre2_callout_enumerate_block_16* blocks)
{
    pcre2_callout_enumerate_16(code, &get_callouts_handler, &blocks);
}

PCRENET_EXPORT(pcre2_jit_stack_16*, jit_stack_create)(const uint32_t start_size, const uint32_t max_size)
{
    return pcre2_jit_stack_create_16(start_size, max_size, NULL);
}

PCRENET_EXPORT(void, jit_stack_free)(pcre2_jit_stack_16* stack)
{
    pcre2_jit_stack_free_16(stack);
}
