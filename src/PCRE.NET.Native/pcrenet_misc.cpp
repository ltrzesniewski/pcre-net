
#include "pcrenet.h"

PCRENET_EXPORT(int32_t, get_error_message)(int32_t errorCode, uint16_t* errorBuffer, uint32_t bufferSize)
{
    return pcre2_get_error_message(errorCode, errorBuffer, bufferSize);
}

PCRENET_EXPORT(int32_t, pattern_info)(const pcre2_code* code, uint32_t key, void* data)
{
    return pcre2_pattern_info(code, key, data);
}

PCRENET_EXPORT(int32_t, config)(uint32_t key, void* data)
{
    return pcre2_config(key, data);
}
