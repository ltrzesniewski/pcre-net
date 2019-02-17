
#include "pcrenet.h"

PCRENET_EXPORT(int32_t, get_error_message)(int32_t errorCode, uint16_t* errorBuffer, uint32_t bufferSize)
{
    return pcre2_get_error_message(errorCode, errorBuffer, bufferSize);
}

PCRENET_EXPORT(int32_t, pattern_info)(pcre2_code* code, uint32_t key, void* data)
{
    return pcre2_pattern_info(code, key, data);
}

PCRENET_EXPORT(int32_t, config)(uint32_t key, void* data)
{
    return pcre2_config(key, data);
}

static int get_callout_count_handler(pcre2_callout_enumerate_block* block, void* data)
{
    const auto count = static_cast<uint32_t*>(data);
    ++*count;
    return 0;
}

PCRENET_EXPORT(uint32_t, get_callout_count)(pcre2_code* code)
{
    uint32_t count = 0;
    pcre2_callout_enumerate(code, &get_callout_count_handler, &count);
    return count;
}

static int get_callouts_handler(pcre2_callout_enumerate_block* block, void* data)
{
    const auto blocks = static_cast<pcre2_callout_enumerate_block**>(data);
    **blocks = *block;
    ++*blocks;
    return 0;
}

PCRENET_EXPORT(void, get_callouts)(pcre2_code* code, pcre2_callout_enumerate_block* blocks)
{
    pcre2_callout_enumerate(code, &get_callouts_handler, &blocks);
}

PCRENET_EXPORT(pcre2_jit_stack*, jit_stack_create)(uint32_t startSize, uint32_t maxSize)
{
    return pcre2_jit_stack_create(startSize, maxSize, nullptr);
}

PCRENET_EXPORT(void, jit_stack_free)(pcre2_jit_stack* stack)
{
    pcre2_jit_stack_free(stack);
}
