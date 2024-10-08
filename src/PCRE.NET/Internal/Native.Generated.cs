﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;
using System.Security;

#nullable enable

namespace PCRE.Internal;

unsafe partial class Native
{
#if NET

    [SuppressGCTransition]
    [DllImport("PCRE.NET.Native", EntryPoint = "pcrenet_get_error_message", CallingConvention = CallingConvention.Cdecl)]
    public static extern int get_error_message(int errorCode, char* errorBuffer, uint bufferSize);

    [DllImport("PCRE.NET.Native", EntryPoint = "pcrenet_compile", CallingConvention = CallingConvention.Cdecl)]
    public static extern void compile(compile_input* input, compile_result* result);

    [DllImport("PCRE.NET.Native", EntryPoint = "pcrenet_code_free", CallingConvention = CallingConvention.Cdecl)]
    public static extern void code_free(IntPtr code);

    [SuppressGCTransition]
    [DllImport("PCRE.NET.Native", EntryPoint = "pcrenet_pattern_info", CallingConvention = CallingConvention.Cdecl)]
    public static extern int pattern_info(IntPtr code, uint key, void* data);

    [SuppressGCTransition]
    [DllImport("PCRE.NET.Native", EntryPoint = "pcrenet_config", CallingConvention = CallingConvention.Cdecl)]
    public static extern int config(uint key, void* data);

    [DllImport("PCRE.NET.Native", EntryPoint = "pcrenet_match", CallingConvention = CallingConvention.Cdecl)]
    public static extern void match(match_input* input, match_result* result);

    [DllImport("PCRE.NET.Native", EntryPoint = "pcrenet_buffer_match", CallingConvention = CallingConvention.Cdecl)]
    public static extern void buffer_match(buffer_match_input* input, match_result* result);

    [DllImport("PCRE.NET.Native", EntryPoint = "pcrenet_dfa_match", CallingConvention = CallingConvention.Cdecl)]
    public static extern void dfa_match(dfa_match_input* input, match_result* result);

    [DllImport("PCRE.NET.Native", EntryPoint = "pcrenet_substitute", CallingConvention = CallingConvention.Cdecl)]
    public static extern void substitute(substitute_input* input, substitute_result* result);

    [DllImport("PCRE.NET.Native", EntryPoint = "pcrenet_substitute_result_free", CallingConvention = CallingConvention.Cdecl)]
    public static extern void substitute_result_free(substitute_result* result);

    [DllImport("PCRE.NET.Native", EntryPoint = "pcrenet_create_match_buffer", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr create_match_buffer(match_buffer_info* info);

    [DllImport("PCRE.NET.Native", EntryPoint = "pcrenet_free_match_buffer", CallingConvention = CallingConvention.Cdecl)]
    public static extern void free_match_buffer(IntPtr buffer);

    [SuppressGCTransition]
    [DllImport("PCRE.NET.Native", EntryPoint = "pcrenet_get_callout_count", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint get_callout_count(IntPtr code);

    [SuppressGCTransition]
    [DllImport("PCRE.NET.Native", EntryPoint = "pcrenet_get_callouts", CallingConvention = CallingConvention.Cdecl)]
    public static extern void get_callouts(IntPtr code, pcre2_callout_enumerate_block* data);

    [DllImport("PCRE.NET.Native", EntryPoint = "pcrenet_jit_stack_create", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr jit_stack_create(uint startSize, uint maxSize);

    [DllImport("PCRE.NET.Native", EntryPoint = "pcrenet_jit_stack_free", CallingConvention = CallingConvention.Cdecl)]
    public static extern void jit_stack_free(IntPtr stack);

    [DllImport("PCRE.NET.Native", EntryPoint = "pcrenet_convert", CallingConvention = CallingConvention.Cdecl)]
    public static extern int convert(convert_input* input, convert_result* result);

    [DllImport("PCRE.NET.Native", EntryPoint = "pcrenet_convert_result_free", CallingConvention = CallingConvention.Cdecl)]
    public static extern void convert_result_free(char* str);

#else

    public static int get_error_message(int errorCode, char* errorBuffer, uint bufferSize)
        => _impl.get_error_message(errorCode, errorBuffer, bufferSize);

    public static void compile(compile_input* input, compile_result* result)
        => _impl.compile(input, result);

    public static void code_free(IntPtr code)
        => _impl.code_free(code);

    public static int pattern_info(IntPtr code, uint key, void* data)
        => _impl.pattern_info(code, key, data);

    public static int config(uint key, void* data)
        => _impl.config(key, data);

    public static void match(match_input* input, match_result* result)
        => _impl.match(input, result);

    public static void buffer_match(buffer_match_input* input, match_result* result)
        => _impl.buffer_match(input, result);

    public static void dfa_match(dfa_match_input* input, match_result* result)
        => _impl.dfa_match(input, result);

    public static void substitute(substitute_input* input, substitute_result* result)
        => _impl.substitute(input, result);

    public static void substitute_result_free(substitute_result* result)
        => _impl.substitute_result_free(result);

    public static IntPtr create_match_buffer(match_buffer_info* info)
        => _impl.create_match_buffer(info);

    public static void free_match_buffer(IntPtr buffer)
        => _impl.free_match_buffer(buffer);

    public static uint get_callout_count(IntPtr code)
        => _impl.get_callout_count(code);

    public static void get_callouts(IntPtr code, pcre2_callout_enumerate_block* data)
        => _impl.get_callouts(code, data);

    public static IntPtr jit_stack_create(uint startSize, uint maxSize)
        => _impl.jit_stack_create(startSize, maxSize);

    public static void jit_stack_free(IntPtr stack)
        => _impl.jit_stack_free(stack);

    public static int convert(convert_input* input, convert_result* result)
        => _impl.convert(input, result);

    public static void convert_result_free(char* str)
        => _impl.convert_result_free(str);

    private abstract class LibImpl
    {
        public abstract int get_error_message(int errorCode, char* errorBuffer, uint bufferSize);
        public abstract void compile(compile_input* input, compile_result* result);
        public abstract void code_free(IntPtr code);
        public abstract int pattern_info(IntPtr code, uint key, void* data);
        public abstract int config(uint key, void* data);
        public abstract void match(match_input* input, match_result* result);
        public abstract void buffer_match(buffer_match_input* input, match_result* result);
        public abstract void dfa_match(dfa_match_input* input, match_result* result);
        public abstract void substitute(substitute_input* input, substitute_result* result);
        public abstract void substitute_result_free(substitute_result* result);
        public abstract IntPtr create_match_buffer(match_buffer_info* info);
        public abstract void free_match_buffer(IntPtr buffer);
        public abstract uint get_callout_count(IntPtr code);
        public abstract void get_callouts(IntPtr code, pcre2_callout_enumerate_block* data);
        public abstract IntPtr jit_stack_create(uint startSize, uint maxSize);
        public abstract void jit_stack_free(IntPtr stack);
        public abstract int convert(convert_input* input, convert_result* result);
        public abstract void convert_result_free(char* str);
    }

    [SuppressUnmanagedCodeSecurity]
    private sealed class WinImpl : LibImpl
    {
        public override int get_error_message(int errorCode, char* errorBuffer, uint bufferSize)
            => pcrenet_get_error_message(errorCode, errorBuffer, bufferSize);

        [DllImport("PCRE.NET.Native.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int pcrenet_get_error_message(int errorCode, char* errorBuffer, uint bufferSize);

        public override void compile(compile_input* input, compile_result* result)
            => pcrenet_compile(input, result);

        [DllImport("PCRE.NET.Native.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_compile(compile_input* input, compile_result* result);

        public override void code_free(IntPtr code)
            => pcrenet_code_free(code);

        [DllImport("PCRE.NET.Native.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_code_free(IntPtr code);

        public override int pattern_info(IntPtr code, uint key, void* data)
            => pcrenet_pattern_info(code, key, data);

        [DllImport("PCRE.NET.Native.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int pcrenet_pattern_info(IntPtr code, uint key, void* data);

        public override int config(uint key, void* data)
            => pcrenet_config(key, data);

        [DllImport("PCRE.NET.Native.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int pcrenet_config(uint key, void* data);

        public override void match(match_input* input, match_result* result)
            => pcrenet_match(input, result);

        [DllImport("PCRE.NET.Native.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_match(match_input* input, match_result* result);

        public override void buffer_match(buffer_match_input* input, match_result* result)
            => pcrenet_buffer_match(input, result);

        [DllImport("PCRE.NET.Native.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_buffer_match(buffer_match_input* input, match_result* result);

        public override void dfa_match(dfa_match_input* input, match_result* result)
            => pcrenet_dfa_match(input, result);

        [DllImport("PCRE.NET.Native.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_dfa_match(dfa_match_input* input, match_result* result);

        public override void substitute(substitute_input* input, substitute_result* result)
            => pcrenet_substitute(input, result);

        [DllImport("PCRE.NET.Native.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_substitute(substitute_input* input, substitute_result* result);

        public override void substitute_result_free(substitute_result* result)
            => pcrenet_substitute_result_free(result);

        [DllImport("PCRE.NET.Native.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_substitute_result_free(substitute_result* result);

        public override IntPtr create_match_buffer(match_buffer_info* info)
            => pcrenet_create_match_buffer(info);

        [DllImport("PCRE.NET.Native.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr pcrenet_create_match_buffer(match_buffer_info* info);

        public override void free_match_buffer(IntPtr buffer)
            => pcrenet_free_match_buffer(buffer);

        [DllImport("PCRE.NET.Native.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_free_match_buffer(IntPtr buffer);

        public override uint get_callout_count(IntPtr code)
            => pcrenet_get_callout_count(code);

        [DllImport("PCRE.NET.Native.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint pcrenet_get_callout_count(IntPtr code);

        public override void get_callouts(IntPtr code, pcre2_callout_enumerate_block* data)
            => pcrenet_get_callouts(code, data);

        [DllImport("PCRE.NET.Native.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_get_callouts(IntPtr code, pcre2_callout_enumerate_block* data);

        public override IntPtr jit_stack_create(uint startSize, uint maxSize)
            => pcrenet_jit_stack_create(startSize, maxSize);

        [DllImport("PCRE.NET.Native.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr pcrenet_jit_stack_create(uint startSize, uint maxSize);

        public override void jit_stack_free(IntPtr stack)
            => pcrenet_jit_stack_free(stack);

        [DllImport("PCRE.NET.Native.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_jit_stack_free(IntPtr stack);

        public override int convert(convert_input* input, convert_result* result)
            => pcrenet_convert(input, result);

        [DllImport("PCRE.NET.Native.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int pcrenet_convert(convert_input* input, convert_result* result);

        public override void convert_result_free(char* str)
            => pcrenet_convert_result_free(str);

        [DllImport("PCRE.NET.Native.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_convert_result_free(char* str);

    }

    [SuppressUnmanagedCodeSecurity]
    private sealed class Win32Impl : LibImpl
    {
        public override int get_error_message(int errorCode, char* errorBuffer, uint bufferSize)
            => pcrenet_get_error_message(errorCode, errorBuffer, bufferSize);

        [DllImport("PCRE.NET.Native.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int pcrenet_get_error_message(int errorCode, char* errorBuffer, uint bufferSize);

        public override void compile(compile_input* input, compile_result* result)
            => pcrenet_compile(input, result);

        [DllImport("PCRE.NET.Native.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_compile(compile_input* input, compile_result* result);

        public override void code_free(IntPtr code)
            => pcrenet_code_free(code);

        [DllImport("PCRE.NET.Native.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_code_free(IntPtr code);

        public override int pattern_info(IntPtr code, uint key, void* data)
            => pcrenet_pattern_info(code, key, data);

        [DllImport("PCRE.NET.Native.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int pcrenet_pattern_info(IntPtr code, uint key, void* data);

        public override int config(uint key, void* data)
            => pcrenet_config(key, data);

        [DllImport("PCRE.NET.Native.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int pcrenet_config(uint key, void* data);

        public override void match(match_input* input, match_result* result)
            => pcrenet_match(input, result);

        [DllImport("PCRE.NET.Native.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_match(match_input* input, match_result* result);

        public override void buffer_match(buffer_match_input* input, match_result* result)
            => pcrenet_buffer_match(input, result);

        [DllImport("PCRE.NET.Native.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_buffer_match(buffer_match_input* input, match_result* result);

        public override void dfa_match(dfa_match_input* input, match_result* result)
            => pcrenet_dfa_match(input, result);

        [DllImport("PCRE.NET.Native.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_dfa_match(dfa_match_input* input, match_result* result);

        public override void substitute(substitute_input* input, substitute_result* result)
            => pcrenet_substitute(input, result);

        [DllImport("PCRE.NET.Native.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_substitute(substitute_input* input, substitute_result* result);

        public override void substitute_result_free(substitute_result* result)
            => pcrenet_substitute_result_free(result);

        [DllImport("PCRE.NET.Native.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_substitute_result_free(substitute_result* result);

        public override IntPtr create_match_buffer(match_buffer_info* info)
            => pcrenet_create_match_buffer(info);

        [DllImport("PCRE.NET.Native.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr pcrenet_create_match_buffer(match_buffer_info* info);

        public override void free_match_buffer(IntPtr buffer)
            => pcrenet_free_match_buffer(buffer);

        [DllImport("PCRE.NET.Native.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_free_match_buffer(IntPtr buffer);

        public override uint get_callout_count(IntPtr code)
            => pcrenet_get_callout_count(code);

        [DllImport("PCRE.NET.Native.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint pcrenet_get_callout_count(IntPtr code);

        public override void get_callouts(IntPtr code, pcre2_callout_enumerate_block* data)
            => pcrenet_get_callouts(code, data);

        [DllImport("PCRE.NET.Native.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_get_callouts(IntPtr code, pcre2_callout_enumerate_block* data);

        public override IntPtr jit_stack_create(uint startSize, uint maxSize)
            => pcrenet_jit_stack_create(startSize, maxSize);

        [DllImport("PCRE.NET.Native.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr pcrenet_jit_stack_create(uint startSize, uint maxSize);

        public override void jit_stack_free(IntPtr stack)
            => pcrenet_jit_stack_free(stack);

        [DllImport("PCRE.NET.Native.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_jit_stack_free(IntPtr stack);

        public override int convert(convert_input* input, convert_result* result)
            => pcrenet_convert(input, result);

        [DllImport("PCRE.NET.Native.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int pcrenet_convert(convert_input* input, convert_result* result);

        public override void convert_result_free(char* str)
            => pcrenet_convert_result_free(str);

        [DllImport("PCRE.NET.Native.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_convert_result_free(char* str);

    }

    [SuppressUnmanagedCodeSecurity]
    private sealed class Win64Impl : LibImpl
    {
        public override int get_error_message(int errorCode, char* errorBuffer, uint bufferSize)
            => pcrenet_get_error_message(errorCode, errorBuffer, bufferSize);

        [DllImport("PCRE.NET.Native.x64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int pcrenet_get_error_message(int errorCode, char* errorBuffer, uint bufferSize);

        public override void compile(compile_input* input, compile_result* result)
            => pcrenet_compile(input, result);

        [DllImport("PCRE.NET.Native.x64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_compile(compile_input* input, compile_result* result);

        public override void code_free(IntPtr code)
            => pcrenet_code_free(code);

        [DllImport("PCRE.NET.Native.x64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_code_free(IntPtr code);

        public override int pattern_info(IntPtr code, uint key, void* data)
            => pcrenet_pattern_info(code, key, data);

        [DllImport("PCRE.NET.Native.x64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int pcrenet_pattern_info(IntPtr code, uint key, void* data);

        public override int config(uint key, void* data)
            => pcrenet_config(key, data);

        [DllImport("PCRE.NET.Native.x64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int pcrenet_config(uint key, void* data);

        public override void match(match_input* input, match_result* result)
            => pcrenet_match(input, result);

        [DllImport("PCRE.NET.Native.x64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_match(match_input* input, match_result* result);

        public override void buffer_match(buffer_match_input* input, match_result* result)
            => pcrenet_buffer_match(input, result);

        [DllImport("PCRE.NET.Native.x64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_buffer_match(buffer_match_input* input, match_result* result);

        public override void dfa_match(dfa_match_input* input, match_result* result)
            => pcrenet_dfa_match(input, result);

        [DllImport("PCRE.NET.Native.x64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_dfa_match(dfa_match_input* input, match_result* result);

        public override void substitute(substitute_input* input, substitute_result* result)
            => pcrenet_substitute(input, result);

        [DllImport("PCRE.NET.Native.x64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_substitute(substitute_input* input, substitute_result* result);

        public override void substitute_result_free(substitute_result* result)
            => pcrenet_substitute_result_free(result);

        [DllImport("PCRE.NET.Native.x64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_substitute_result_free(substitute_result* result);

        public override IntPtr create_match_buffer(match_buffer_info* info)
            => pcrenet_create_match_buffer(info);

        [DllImport("PCRE.NET.Native.x64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr pcrenet_create_match_buffer(match_buffer_info* info);

        public override void free_match_buffer(IntPtr buffer)
            => pcrenet_free_match_buffer(buffer);

        [DllImport("PCRE.NET.Native.x64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_free_match_buffer(IntPtr buffer);

        public override uint get_callout_count(IntPtr code)
            => pcrenet_get_callout_count(code);

        [DllImport("PCRE.NET.Native.x64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint pcrenet_get_callout_count(IntPtr code);

        public override void get_callouts(IntPtr code, pcre2_callout_enumerate_block* data)
            => pcrenet_get_callouts(code, data);

        [DllImport("PCRE.NET.Native.x64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_get_callouts(IntPtr code, pcre2_callout_enumerate_block* data);

        public override IntPtr jit_stack_create(uint startSize, uint maxSize)
            => pcrenet_jit_stack_create(startSize, maxSize);

        [DllImport("PCRE.NET.Native.x64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr pcrenet_jit_stack_create(uint startSize, uint maxSize);

        public override void jit_stack_free(IntPtr stack)
            => pcrenet_jit_stack_free(stack);

        [DllImport("PCRE.NET.Native.x64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_jit_stack_free(IntPtr stack);

        public override int convert(convert_input* input, convert_result* result)
            => pcrenet_convert(input, result);

        [DllImport("PCRE.NET.Native.x64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int pcrenet_convert(convert_input* input, convert_result* result);

        public override void convert_result_free(char* str)
            => pcrenet_convert_result_free(str);

        [DllImport("PCRE.NET.Native.x64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_convert_result_free(char* str);

    }

    [SuppressUnmanagedCodeSecurity]
    private sealed class LinuxImpl : LibImpl
    {
        public override int get_error_message(int errorCode, char* errorBuffer, uint bufferSize)
            => pcrenet_get_error_message(errorCode, errorBuffer, bufferSize);

        [DllImport("PCRE.NET.Native.so", CallingConvention = CallingConvention.Cdecl)]
        private static extern int pcrenet_get_error_message(int errorCode, char* errorBuffer, uint bufferSize);

        public override void compile(compile_input* input, compile_result* result)
            => pcrenet_compile(input, result);

        [DllImport("PCRE.NET.Native.so", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_compile(compile_input* input, compile_result* result);

        public override void code_free(IntPtr code)
            => pcrenet_code_free(code);

        [DllImport("PCRE.NET.Native.so", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_code_free(IntPtr code);

        public override int pattern_info(IntPtr code, uint key, void* data)
            => pcrenet_pattern_info(code, key, data);

        [DllImport("PCRE.NET.Native.so", CallingConvention = CallingConvention.Cdecl)]
        private static extern int pcrenet_pattern_info(IntPtr code, uint key, void* data);

        public override int config(uint key, void* data)
            => pcrenet_config(key, data);

        [DllImport("PCRE.NET.Native.so", CallingConvention = CallingConvention.Cdecl)]
        private static extern int pcrenet_config(uint key, void* data);

        public override void match(match_input* input, match_result* result)
            => pcrenet_match(input, result);

        [DllImport("PCRE.NET.Native.so", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_match(match_input* input, match_result* result);

        public override void buffer_match(buffer_match_input* input, match_result* result)
            => pcrenet_buffer_match(input, result);

        [DllImport("PCRE.NET.Native.so", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_buffer_match(buffer_match_input* input, match_result* result);

        public override void dfa_match(dfa_match_input* input, match_result* result)
            => pcrenet_dfa_match(input, result);

        [DllImport("PCRE.NET.Native.so", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_dfa_match(dfa_match_input* input, match_result* result);

        public override void substitute(substitute_input* input, substitute_result* result)
            => pcrenet_substitute(input, result);

        [DllImport("PCRE.NET.Native.so", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_substitute(substitute_input* input, substitute_result* result);

        public override void substitute_result_free(substitute_result* result)
            => pcrenet_substitute_result_free(result);

        [DllImport("PCRE.NET.Native.so", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_substitute_result_free(substitute_result* result);

        public override IntPtr create_match_buffer(match_buffer_info* info)
            => pcrenet_create_match_buffer(info);

        [DllImport("PCRE.NET.Native.so", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr pcrenet_create_match_buffer(match_buffer_info* info);

        public override void free_match_buffer(IntPtr buffer)
            => pcrenet_free_match_buffer(buffer);

        [DllImport("PCRE.NET.Native.so", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_free_match_buffer(IntPtr buffer);

        public override uint get_callout_count(IntPtr code)
            => pcrenet_get_callout_count(code);

        [DllImport("PCRE.NET.Native.so", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint pcrenet_get_callout_count(IntPtr code);

        public override void get_callouts(IntPtr code, pcre2_callout_enumerate_block* data)
            => pcrenet_get_callouts(code, data);

        [DllImport("PCRE.NET.Native.so", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_get_callouts(IntPtr code, pcre2_callout_enumerate_block* data);

        public override IntPtr jit_stack_create(uint startSize, uint maxSize)
            => pcrenet_jit_stack_create(startSize, maxSize);

        [DllImport("PCRE.NET.Native.so", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr pcrenet_jit_stack_create(uint startSize, uint maxSize);

        public override void jit_stack_free(IntPtr stack)
            => pcrenet_jit_stack_free(stack);

        [DllImport("PCRE.NET.Native.so", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_jit_stack_free(IntPtr stack);

        public override int convert(convert_input* input, convert_result* result)
            => pcrenet_convert(input, result);

        [DllImport("PCRE.NET.Native.so", CallingConvention = CallingConvention.Cdecl)]
        private static extern int pcrenet_convert(convert_input* input, convert_result* result);

        public override void convert_result_free(char* str)
            => pcrenet_convert_result_free(str);

        [DllImport("PCRE.NET.Native.so", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_convert_result_free(char* str);

    }

    [SuppressUnmanagedCodeSecurity]
    private sealed class OSXImpl : LibImpl
    {
        public override int get_error_message(int errorCode, char* errorBuffer, uint bufferSize)
            => pcrenet_get_error_message(errorCode, errorBuffer, bufferSize);

        [DllImport("PCRE.NET.Native.dylib", CallingConvention = CallingConvention.Cdecl)]
        private static extern int pcrenet_get_error_message(int errorCode, char* errorBuffer, uint bufferSize);

        public override void compile(compile_input* input, compile_result* result)
            => pcrenet_compile(input, result);

        [DllImport("PCRE.NET.Native.dylib", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_compile(compile_input* input, compile_result* result);

        public override void code_free(IntPtr code)
            => pcrenet_code_free(code);

        [DllImport("PCRE.NET.Native.dylib", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_code_free(IntPtr code);

        public override int pattern_info(IntPtr code, uint key, void* data)
            => pcrenet_pattern_info(code, key, data);

        [DllImport("PCRE.NET.Native.dylib", CallingConvention = CallingConvention.Cdecl)]
        private static extern int pcrenet_pattern_info(IntPtr code, uint key, void* data);

        public override int config(uint key, void* data)
            => pcrenet_config(key, data);

        [DllImport("PCRE.NET.Native.dylib", CallingConvention = CallingConvention.Cdecl)]
        private static extern int pcrenet_config(uint key, void* data);

        public override void match(match_input* input, match_result* result)
            => pcrenet_match(input, result);

        [DllImport("PCRE.NET.Native.dylib", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_match(match_input* input, match_result* result);

        public override void buffer_match(buffer_match_input* input, match_result* result)
            => pcrenet_buffer_match(input, result);

        [DllImport("PCRE.NET.Native.dylib", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_buffer_match(buffer_match_input* input, match_result* result);

        public override void dfa_match(dfa_match_input* input, match_result* result)
            => pcrenet_dfa_match(input, result);

        [DllImport("PCRE.NET.Native.dylib", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_dfa_match(dfa_match_input* input, match_result* result);

        public override void substitute(substitute_input* input, substitute_result* result)
            => pcrenet_substitute(input, result);

        [DllImport("PCRE.NET.Native.dylib", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_substitute(substitute_input* input, substitute_result* result);

        public override void substitute_result_free(substitute_result* result)
            => pcrenet_substitute_result_free(result);

        [DllImport("PCRE.NET.Native.dylib", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_substitute_result_free(substitute_result* result);

        public override IntPtr create_match_buffer(match_buffer_info* info)
            => pcrenet_create_match_buffer(info);

        [DllImport("PCRE.NET.Native.dylib", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr pcrenet_create_match_buffer(match_buffer_info* info);

        public override void free_match_buffer(IntPtr buffer)
            => pcrenet_free_match_buffer(buffer);

        [DllImport("PCRE.NET.Native.dylib", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_free_match_buffer(IntPtr buffer);

        public override uint get_callout_count(IntPtr code)
            => pcrenet_get_callout_count(code);

        [DllImport("PCRE.NET.Native.dylib", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint pcrenet_get_callout_count(IntPtr code);

        public override void get_callouts(IntPtr code, pcre2_callout_enumerate_block* data)
            => pcrenet_get_callouts(code, data);

        [DllImport("PCRE.NET.Native.dylib", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_get_callouts(IntPtr code, pcre2_callout_enumerate_block* data);

        public override IntPtr jit_stack_create(uint startSize, uint maxSize)
            => pcrenet_jit_stack_create(startSize, maxSize);

        [DllImport("PCRE.NET.Native.dylib", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr pcrenet_jit_stack_create(uint startSize, uint maxSize);

        public override void jit_stack_free(IntPtr stack)
            => pcrenet_jit_stack_free(stack);

        [DllImport("PCRE.NET.Native.dylib", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_jit_stack_free(IntPtr stack);

        public override int convert(convert_input* input, convert_result* result)
            => pcrenet_convert(input, result);

        [DllImport("PCRE.NET.Native.dylib", CallingConvention = CallingConvention.Cdecl)]
        private static extern int pcrenet_convert(convert_input* input, convert_result* result);

        public override void convert_result_free(char* str)
            => pcrenet_convert_result_free(str);

        [DllImport("PCRE.NET.Native.dylib", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pcrenet_convert_result_free(char* str);

    }

#endif
}
