
#pragma once

#include "InfoKey.h"
#include "PatternOptions.h"
#include "JitCompileOptions.h"

using namespace System;

namespace PCRE {
	namespace Wrapper {

		public ref class CompileContext sealed
		{
		public:
			CompileContext(String^ pattern);
			~CompileContext();
			!CompileContext();

			property String^ Pattern { String^ get() { return _pattern; } }
			property PatternOptions Options;
			property JitCompileOptions JitCompileOptions;

			property NewLine NewLine { void set(PCRE::Wrapper::NewLine); }
			property BackslashR BackslashR { void set(PCRE::Wrapper::BackslashR); }
			property uint32_t ParensNestLimit { void set(uint32_t); }

		internal:
			property pcre2_compile_context* Context {
				pcre2_compile_context* get() { return _ctx; }
			}

		private:
			initonly String^ _pattern;
			pcre2_compile_context* _ctx;
		};
	}
}
