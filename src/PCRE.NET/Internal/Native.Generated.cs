﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;
using System.Security;

#nullable enable

namespace PCRE.Internal
{
    unsafe partial class Native
    {
        public static int get_error_message(int errorCode, char* errorBuffer, uint bufferSize)
            => _impl.get_error_message(errorCode, errorBuffer, bufferSize);

        public static void compile(ref compile_input input, out compile_result result)
            => _impl.compile(ref input, out result);

        public static void code_free(IntPtr code)
            => _impl.code_free(code);

        public static int pattern_info(IntPtr code, uint key, void* data)
            => _impl.pattern_info(code, key, data);

        public static int config(uint key, void* data)
            => _impl.config(key, data);

        public static void match(ref match_input input, out match_result result)
            => _impl.match(ref input, out result);

        public static void dfa_match(ref dfa_match_input input, out match_result result)
            => _impl.dfa_match(ref input, out result);

        public static uint get_callout_count(IntPtr code)
            => _impl.get_callout_count(code);

        public static void get_callouts(IntPtr code, pcre2_callout_enumerate_block* data)
            => _impl.get_callouts(code, data);

        public static IntPtr jit_stack_create(uint startSize, uint maxSize)
            => _impl.jit_stack_create(startSize, maxSize);

        public static void jit_stack_free(IntPtr stack)
            => _impl.jit_stack_free(stack);

        public static int convert(ref convert_input input, out convert_result result)
            => _impl.convert(ref input, out result);

        public static int convert_result_free(char* str)
            => _impl.convert_result_free(str);

        private abstract class LibImpl
        {
            public abstract int get_error_message(int errorCode, char* errorBuffer, uint bufferSize);
            public abstract void compile(ref compile_input input, out compile_result result);
            public abstract void code_free(IntPtr code);
            public abstract int pattern_info(IntPtr code, uint key, void* data);
            public abstract int config(uint key, void* data);
            public abstract void match(ref match_input input, out match_result result);
            public abstract void dfa_match(ref dfa_match_input input, out match_result result);
            public abstract uint get_callout_count(IntPtr code);
            public abstract void get_callouts(IntPtr code, pcre2_callout_enumerate_block* data);
            public abstract IntPtr jit_stack_create(uint startSize, uint maxSize);
            public abstract void jit_stack_free(IntPtr stack);
            public abstract int convert(ref convert_input input, out convert_result result);
            public abstract int convert_result_free(char* str);
        }

        [SuppressUnmanagedCodeSecurity]
        private sealed class WinImpl : LibImpl
        {
            public override int get_error_message(int errorCode, char* errorBuffer, uint bufferSize)
                => pcrenet_get_error_message(errorCode, errorBuffer, bufferSize);

#if NETCOREAPP
            [SuppressGCTransition]
#endif
            [DllImport("PCRE.NET.Native.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern int pcrenet_get_error_message(int errorCode, char* errorBuffer, uint bufferSize);

            public override void compile(ref compile_input input, out compile_result result)
                => pcrenet_compile(ref input, out result);

            [DllImport("PCRE.NET.Native.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern void pcrenet_compile(ref compile_input input, out compile_result result);

            public override void code_free(IntPtr code)
                => pcrenet_code_free(code);

            [DllImport("PCRE.NET.Native.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern void pcrenet_code_free(IntPtr code);

            public override int pattern_info(IntPtr code, uint key, void* data)
                => pcrenet_pattern_info(code, key, data);

#if NETCOREAPP
            [SuppressGCTransition]
#endif
            [DllImport("PCRE.NET.Native.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern int pcrenet_pattern_info(IntPtr code, uint key, void* data);

            public override int config(uint key, void* data)
                => pcrenet_config(key, data);

#if NETCOREAPP
            [SuppressGCTransition]
#endif
            [DllImport("PCRE.NET.Native.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern int pcrenet_config(uint key, void* data);

            public override void match(ref match_input input, out match_result result)
                => pcrenet_match(ref input, out result);

            [DllImport("PCRE.NET.Native.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern void pcrenet_match(ref match_input input, out match_result result);

            public override void dfa_match(ref dfa_match_input input, out match_result result)
                => pcrenet_dfa_match(ref input, out result);

            [DllImport("PCRE.NET.Native.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern void pcrenet_dfa_match(ref dfa_match_input input, out match_result result);

            public override uint get_callout_count(IntPtr code)
                => pcrenet_get_callout_count(code);

#if NETCOREAPP
            [SuppressGCTransition]
#endif
            [DllImport("PCRE.NET.Native.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern uint pcrenet_get_callout_count(IntPtr code);

            public override void get_callouts(IntPtr code, pcre2_callout_enumerate_block* data)
                => pcrenet_get_callouts(code, data);

#if NETCOREAPP
            [SuppressGCTransition]
#endif
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

            public override int convert(ref convert_input input, out convert_result result)
                => pcrenet_convert(ref input, out result);

            [DllImport("PCRE.NET.Native.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern int pcrenet_convert(ref convert_input input, out convert_result result);

            public override int convert_result_free(char* str)
                => pcrenet_convert_result_free(str);

            [DllImport("PCRE.NET.Native.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern int pcrenet_convert_result_free(char* str);

        }

        [SuppressUnmanagedCodeSecurity]
        private sealed class Win32Impl : LibImpl
        {
            public override int get_error_message(int errorCode, char* errorBuffer, uint bufferSize)
                => pcrenet_get_error_message(errorCode, errorBuffer, bufferSize);

#if NETCOREAPP
            [SuppressGCTransition]
#endif
            [DllImport("PCRE.NET.Native.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern int pcrenet_get_error_message(int errorCode, char* errorBuffer, uint bufferSize);

            public override void compile(ref compile_input input, out compile_result result)
                => pcrenet_compile(ref input, out result);

            [DllImport("PCRE.NET.Native.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern void pcrenet_compile(ref compile_input input, out compile_result result);

            public override void code_free(IntPtr code)
                => pcrenet_code_free(code);

            [DllImport("PCRE.NET.Native.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern void pcrenet_code_free(IntPtr code);

            public override int pattern_info(IntPtr code, uint key, void* data)
                => pcrenet_pattern_info(code, key, data);

#if NETCOREAPP
            [SuppressGCTransition]
#endif
            [DllImport("PCRE.NET.Native.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern int pcrenet_pattern_info(IntPtr code, uint key, void* data);

            public override int config(uint key, void* data)
                => pcrenet_config(key, data);

#if NETCOREAPP
            [SuppressGCTransition]
#endif
            [DllImport("PCRE.NET.Native.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern int pcrenet_config(uint key, void* data);

            public override void match(ref match_input input, out match_result result)
                => pcrenet_match(ref input, out result);

            [DllImport("PCRE.NET.Native.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern void pcrenet_match(ref match_input input, out match_result result);

            public override void dfa_match(ref dfa_match_input input, out match_result result)
                => pcrenet_dfa_match(ref input, out result);

            [DllImport("PCRE.NET.Native.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern void pcrenet_dfa_match(ref dfa_match_input input, out match_result result);

            public override uint get_callout_count(IntPtr code)
                => pcrenet_get_callout_count(code);

#if NETCOREAPP
            [SuppressGCTransition]
#endif
            [DllImport("PCRE.NET.Native.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern uint pcrenet_get_callout_count(IntPtr code);

            public override void get_callouts(IntPtr code, pcre2_callout_enumerate_block* data)
                => pcrenet_get_callouts(code, data);

#if NETCOREAPP
            [SuppressGCTransition]
#endif
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

            public override int convert(ref convert_input input, out convert_result result)
                => pcrenet_convert(ref input, out result);

            [DllImport("PCRE.NET.Native.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern int pcrenet_convert(ref convert_input input, out convert_result result);

            public override int convert_result_free(char* str)
                => pcrenet_convert_result_free(str);

            [DllImport("PCRE.NET.Native.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern int pcrenet_convert_result_free(char* str);

        }

        [SuppressUnmanagedCodeSecurity]
        private sealed class Win64Impl : LibImpl
        {
            public override int get_error_message(int errorCode, char* errorBuffer, uint bufferSize)
                => pcrenet_get_error_message(errorCode, errorBuffer, bufferSize);

#if NETCOREAPP
            [SuppressGCTransition]
#endif
            [DllImport("PCRE.NET.Native.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern int pcrenet_get_error_message(int errorCode, char* errorBuffer, uint bufferSize);

            public override void compile(ref compile_input input, out compile_result result)
                => pcrenet_compile(ref input, out result);

            [DllImport("PCRE.NET.Native.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern void pcrenet_compile(ref compile_input input, out compile_result result);

            public override void code_free(IntPtr code)
                => pcrenet_code_free(code);

            [DllImport("PCRE.NET.Native.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern void pcrenet_code_free(IntPtr code);

            public override int pattern_info(IntPtr code, uint key, void* data)
                => pcrenet_pattern_info(code, key, data);

#if NETCOREAPP
            [SuppressGCTransition]
#endif
            [DllImport("PCRE.NET.Native.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern int pcrenet_pattern_info(IntPtr code, uint key, void* data);

            public override int config(uint key, void* data)
                => pcrenet_config(key, data);

#if NETCOREAPP
            [SuppressGCTransition]
#endif
            [DllImport("PCRE.NET.Native.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern int pcrenet_config(uint key, void* data);

            public override void match(ref match_input input, out match_result result)
                => pcrenet_match(ref input, out result);

            [DllImport("PCRE.NET.Native.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern void pcrenet_match(ref match_input input, out match_result result);

            public override void dfa_match(ref dfa_match_input input, out match_result result)
                => pcrenet_dfa_match(ref input, out result);

            [DllImport("PCRE.NET.Native.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern void pcrenet_dfa_match(ref dfa_match_input input, out match_result result);

            public override uint get_callout_count(IntPtr code)
                => pcrenet_get_callout_count(code);

#if NETCOREAPP
            [SuppressGCTransition]
#endif
            [DllImport("PCRE.NET.Native.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern uint pcrenet_get_callout_count(IntPtr code);

            public override void get_callouts(IntPtr code, pcre2_callout_enumerate_block* data)
                => pcrenet_get_callouts(code, data);

#if NETCOREAPP
            [SuppressGCTransition]
#endif
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

            public override int convert(ref convert_input input, out convert_result result)
                => pcrenet_convert(ref input, out result);

            [DllImport("PCRE.NET.Native.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern int pcrenet_convert(ref convert_input input, out convert_result result);

            public override int convert_result_free(char* str)
                => pcrenet_convert_result_free(str);

            [DllImport("PCRE.NET.Native.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern int pcrenet_convert_result_free(char* str);

        }

        [SuppressUnmanagedCodeSecurity]
        private sealed class LinuxImpl : LibImpl
        {
            public override int get_error_message(int errorCode, char* errorBuffer, uint bufferSize)
                => pcrenet_get_error_message(errorCode, errorBuffer, bufferSize);

#if NETCOREAPP
            [SuppressGCTransition]
#endif
            [DllImport("PCRE.NET.Native.so", CallingConvention = CallingConvention.Cdecl)]
            private static extern int pcrenet_get_error_message(int errorCode, char* errorBuffer, uint bufferSize);

            public override void compile(ref compile_input input, out compile_result result)
                => pcrenet_compile(ref input, out result);

            [DllImport("PCRE.NET.Native.so", CallingConvention = CallingConvention.Cdecl)]
            private static extern void pcrenet_compile(ref compile_input input, out compile_result result);

            public override void code_free(IntPtr code)
                => pcrenet_code_free(code);

            [DllImport("PCRE.NET.Native.so", CallingConvention = CallingConvention.Cdecl)]
            private static extern void pcrenet_code_free(IntPtr code);

            public override int pattern_info(IntPtr code, uint key, void* data)
                => pcrenet_pattern_info(code, key, data);

#if NETCOREAPP
            [SuppressGCTransition]
#endif
            [DllImport("PCRE.NET.Native.so", CallingConvention = CallingConvention.Cdecl)]
            private static extern int pcrenet_pattern_info(IntPtr code, uint key, void* data);

            public override int config(uint key, void* data)
                => pcrenet_config(key, data);

#if NETCOREAPP
            [SuppressGCTransition]
#endif
            [DllImport("PCRE.NET.Native.so", CallingConvention = CallingConvention.Cdecl)]
            private static extern int pcrenet_config(uint key, void* data);

            public override void match(ref match_input input, out match_result result)
                => pcrenet_match(ref input, out result);

            [DllImport("PCRE.NET.Native.so", CallingConvention = CallingConvention.Cdecl)]
            private static extern void pcrenet_match(ref match_input input, out match_result result);

            public override void dfa_match(ref dfa_match_input input, out match_result result)
                => pcrenet_dfa_match(ref input, out result);

            [DllImport("PCRE.NET.Native.so", CallingConvention = CallingConvention.Cdecl)]
            private static extern void pcrenet_dfa_match(ref dfa_match_input input, out match_result result);

            public override uint get_callout_count(IntPtr code)
                => pcrenet_get_callout_count(code);

#if NETCOREAPP
            [SuppressGCTransition]
#endif
            [DllImport("PCRE.NET.Native.so", CallingConvention = CallingConvention.Cdecl)]
            private static extern uint pcrenet_get_callout_count(IntPtr code);

            public override void get_callouts(IntPtr code, pcre2_callout_enumerate_block* data)
                => pcrenet_get_callouts(code, data);

#if NETCOREAPP
            [SuppressGCTransition]
#endif
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

            public override int convert(ref convert_input input, out convert_result result)
                => pcrenet_convert(ref input, out result);

            [DllImport("PCRE.NET.Native.so", CallingConvention = CallingConvention.Cdecl)]
            private static extern int pcrenet_convert(ref convert_input input, out convert_result result);

            public override int convert_result_free(char* str)
                => pcrenet_convert_result_free(str);

            [DllImport("PCRE.NET.Native.so", CallingConvention = CallingConvention.Cdecl)]
            private static extern int pcrenet_convert_result_free(char* str);

        }

        [SuppressUnmanagedCodeSecurity]
        private sealed class OSXImpl : LibImpl
        {
            public override int get_error_message(int errorCode, char* errorBuffer, uint bufferSize)
                => pcrenet_get_error_message(errorCode, errorBuffer, bufferSize);

#if NETCOREAPP
            [SuppressGCTransition]
#endif
            [DllImport("PCRE.NET.Native.dylib", CallingConvention = CallingConvention.Cdecl)]
            private static extern int pcrenet_get_error_message(int errorCode, char* errorBuffer, uint bufferSize);

            public override void compile(ref compile_input input, out compile_result result)
                => pcrenet_compile(ref input, out result);

            [DllImport("PCRE.NET.Native.dylib", CallingConvention = CallingConvention.Cdecl)]
            private static extern void pcrenet_compile(ref compile_input input, out compile_result result);

            public override void code_free(IntPtr code)
                => pcrenet_code_free(code);

            [DllImport("PCRE.NET.Native.dylib", CallingConvention = CallingConvention.Cdecl)]
            private static extern void pcrenet_code_free(IntPtr code);

            public override int pattern_info(IntPtr code, uint key, void* data)
                => pcrenet_pattern_info(code, key, data);

#if NETCOREAPP
            [SuppressGCTransition]
#endif
            [DllImport("PCRE.NET.Native.dylib", CallingConvention = CallingConvention.Cdecl)]
            private static extern int pcrenet_pattern_info(IntPtr code, uint key, void* data);

            public override int config(uint key, void* data)
                => pcrenet_config(key, data);

#if NETCOREAPP
            [SuppressGCTransition]
#endif
            [DllImport("PCRE.NET.Native.dylib", CallingConvention = CallingConvention.Cdecl)]
            private static extern int pcrenet_config(uint key, void* data);

            public override void match(ref match_input input, out match_result result)
                => pcrenet_match(ref input, out result);

            [DllImport("PCRE.NET.Native.dylib", CallingConvention = CallingConvention.Cdecl)]
            private static extern void pcrenet_match(ref match_input input, out match_result result);

            public override void dfa_match(ref dfa_match_input input, out match_result result)
                => pcrenet_dfa_match(ref input, out result);

            [DllImport("PCRE.NET.Native.dylib", CallingConvention = CallingConvention.Cdecl)]
            private static extern void pcrenet_dfa_match(ref dfa_match_input input, out match_result result);

            public override uint get_callout_count(IntPtr code)
                => pcrenet_get_callout_count(code);

#if NETCOREAPP
            [SuppressGCTransition]
#endif
            [DllImport("PCRE.NET.Native.dylib", CallingConvention = CallingConvention.Cdecl)]
            private static extern uint pcrenet_get_callout_count(IntPtr code);

            public override void get_callouts(IntPtr code, pcre2_callout_enumerate_block* data)
                => pcrenet_get_callouts(code, data);

#if NETCOREAPP
            [SuppressGCTransition]
#endif
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

            public override int convert(ref convert_input input, out convert_result result)
                => pcrenet_convert(ref input, out result);

            [DllImport("PCRE.NET.Native.dylib", CallingConvention = CallingConvention.Cdecl)]
            private static extern int pcrenet_convert(ref convert_input input, out convert_result result);

            public override int convert_result_free(char* str)
                => pcrenet_convert_result_free(str);

            [DllImport("PCRE.NET.Native.dylib", CallingConvention = CallingConvention.Cdecl)]
            private static extern int pcrenet_convert_result_free(char* str);

        }

    }
}
