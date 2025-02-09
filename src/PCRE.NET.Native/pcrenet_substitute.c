#include "pcrenet.h"

typedef int (*match_callout_fn)(pcre2_callout_block*, void*);
typedef int (*substitute_callout_fn)(pcre2_substitute_callout_block*, void*);
typedef PCRE2_SIZE (*substitute_case_callout_fn)(PCRE2_SPTR, PCRE2_SIZE, PCRE2_UCHAR*, PCRE2_SIZE, int, void*);

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
    match_callout_fn match_callout;
    substitute_callout_fn substitute_callout;
    substitute_case_callout_fn substitute_case_callout;
    void* callout_data;
} pcrenet_substitute_input;

typedef struct
{
    int32_t result_code;
    uint16_t* output;
    size_t output_length;
    uint8_t output_on_heap;
    uint32_t substitute_call_count;
} pcrenet_substitute_result;

typedef struct
{
    uint8_t* buffer;
    size_t buffer_size;
    size_t count;
    size_t replayed;
} replay_queue;

typedef struct
{
    const pcrenet_substitute_input* input;
    replay_queue match_callout_queue;
    replay_queue substitute_callout_queue;
} substitute_callout_data;

static size_t max_size(const size_t a, const size_t b)
{
    return a > b ? a : b;
}

static void replay_queue_init(replay_queue* queue)
{
    queue->buffer = NULL;
    queue->buffer_size = 0;
    queue->count = 0;
    queue->replayed = 0;
}

static void replay_queue_free(replay_queue* queue)
{
    if (queue->buffer)
        free(queue->buffer);

    replay_queue_init(queue);
}

static void replay_queue_start_replay(replay_queue* queue)
{
    queue->replayed = 0;
}

static int replay_queue_try_dequeue(replay_queue* queue, uint8_t* result)
{
    if (!queue->buffer && queue->buffer_size)
        return 0; // Invalid queue

    if (queue->replayed < queue->count)
    {
        *result = queue->buffer[queue->replayed++];
        return 1;
    }

    return 0;
}

static void replay_queue_try_enqueue(replay_queue* queue, uint8_t result)
{
    if (!queue->buffer && queue->buffer_size)
        return; // Invalid queue

    if (queue->count == queue->buffer_size)
    {
        const size_t new_size = max_size(64, 2 * queue->buffer_size);
        uint8_t* new_buf = realloc(queue->buffer, new_size * sizeof(uint8_t));
        if (!new_buf)
            return;

        queue->buffer = new_buf;
        queue->buffer_size = new_size;
    }

    queue->buffer[queue->count++] = result;
    queue->replayed = queue->count;
}

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

static uint8_t map_callout_result_int_to_byte(int result)
{
    if (result == 0)
        return 0;

    if (result > 0)
        return 1;

    return 2;
}

static int map_callout_result_byte_to_int(uint8_t result)
{
    switch (result)
    {
    case 0:
        return 0;

    case 1:
        return 1;

    default:
        return -1;
    }
}

static int match_callout_handler(pcre2_callout_block* block, void* data_ptr)
{
    substitute_callout_data* data = data_ptr;

    uint8_t result;
    if (replay_queue_try_dequeue(&data->match_callout_queue, &result))
        return map_callout_result_byte_to_int(result);

    result = map_callout_result_int_to_byte(
        data->input->match_callout(block, data->input->callout_data));

    replay_queue_try_enqueue(&data->match_callout_queue, result);
    return result;
}

static int substitute_callout_handler(pcre2_substitute_callout_block* block, void* data_ptr)
{
    substitute_callout_data* data = data_ptr;

    uint8_t result;
    if (replay_queue_try_dequeue(&data->substitute_callout_queue, &result))
        return map_callout_result_byte_to_int(result);

    result = map_callout_result_int_to_byte(
        data->input->substitute_callout(block, data->input->callout_data));

    replay_queue_try_enqueue(&data->substitute_callout_queue, result);
    return result;
}

static void call_substitute(const pcrenet_substitute_input* input,
                            pcrenet_substitute_result* result,
                            PCRE2_SIZE* output_length,
                            pcre2_match_data* match_data,
                            pcre2_match_context* match_context)
{
    // - Always try to get an estimate of the required buffer size
    // - Skip UTF checks if they were already performed in a previous pass

    const uint32_t additional_options = PCRE2_SUBSTITUTE_OVERFLOW_LENGTH
        | (result->substitute_call_count > 0 ? PCRE2_NO_UTF_CHECK : 0);

    result->substitute_call_count++;

    result->result_code = pcre2_substitute(
        input->code,
        input->subject,
        input->subject_length,
        input->start_index,
        input->additional_options | additional_options,
        match_data,
        match_context,
        input->replacement,
        input->replacement_length,
        result->output,
        output_length
    );
}

static void substitute_simple(const pcrenet_substitute_input* input,
                              pcrenet_substitute_result* result,
                              pcre2_match_data* match_data,
                              pcre2_match_context* match_context)
{
    // Try to substitute in one or two passes max using the PCRE2_SUBSTITUTE_OVERFLOW_LENGTH option
    // which is automatically added by call_substitute

    result->output = input->buffer;
    result->output_length = 0;
    result->output_on_heap = 0;

    PCRE2_SIZE output_length = input->buffer_length;

    call_substitute(
        input,
        result,
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

        call_substitute(
            input,
            result,
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

    replay_queue_init(&callout_data.match_callout_queue);
    replay_queue_init(&callout_data.substitute_callout_queue);

    if (input->match_callout)
        pcre2_set_callout(match_context, &match_callout_handler, &callout_data);

    if (input->substitute_callout)
        pcre2_set_substitute_callout(match_context, &substitute_callout_handler, &callout_data);

    if (input->substitute_case_callout)
        pcre2_set_substitute_case_callout(match_context, input->substitute_case_callout, input->callout_data);

    while (1)
    {
        replay_queue_start_replay(&callout_data.match_callout_queue);
        replay_queue_start_replay(&callout_data.substitute_callout_queue);

        call_substitute(
            input,
            result,
            &output_length,
            match_data,
            match_context
        );

        // Success
        if (result->result_code >= 0)
        {
            result->output_length = output_length;
            break;
        }

        // Output buffer is too small
        if (result->result_code == PCRE2_ERROR_NOMEMORY)
        {
            buffer_length = max_size(output_length, max_size(2 * buffer_length, 2 * (size_t)input->subject_length));

            if (!result->output_on_heap)
            {
                result->output = malloc(buffer_length * sizeof(PCRE2_UCHAR));

                if (!result->output)
                    break;

                result->output_on_heap = 1;
            }
            else
            {
                uint16_t* new_buffer = realloc(result->output, buffer_length * sizeof(PCRE2_UCHAR));

                if (!new_buffer)
                {
                    free_result_memory(result);
                    break;
                }

                result->output = new_buffer;
            }

            output_length = buffer_length;
            continue;
        }

        // Error
        free_result_memory(result);
        break;
    }

    replay_queue_free(&callout_data.match_callout_queue);
    replay_queue_free(&callout_data.substitute_callout_queue);
}

PCRENET_EXPORT(void, substitute)(const pcrenet_substitute_input* input, pcrenet_substitute_result* result)
{
    pcre2_match_data* match_data = pcre2_match_data_create_from_pattern(input->code, NULL);
    pcre2_match_context* match_context = pcre2_match_context_create(NULL);

    apply_settings(&input->settings, match_context);

    result->substitute_call_count = 0;

    if (!input->match_callout && !input->substitute_callout && !input->substitute_case_callout)
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
