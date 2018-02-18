
#pragma once

#include "InfoKey.h"
#include "PatternOptions.h"
#include "JitCompileOptions.h"

namespace PCRE {
	namespace Wrapper {

		public ref class CompileContext sealed
		{
		public:
			CompileContext(System::String^ pattern);
			~CompileContext();
			!CompileContext();

			property System::String^ Pattern { System::String^ get() { return _pattern; } }
			property PatternOptions Options;
			property JitCompileOptions JitCompileOptions;

			property NewLine NewLine { void set(PCRE::Wrapper::NewLine); }
			property BackslashR BackslashR { void set(PCRE::Wrapper::BackslashR); }
			property uint32_t ParensNestLimit { void set(uint32_t); }
			property uint32_t MaxPatternLength { void set(uint32_t); }
			property ExtraCompileOptions ExtraCompileOptions { void set(PCRE::Wrapper::ExtraCompileOptions); }

		internal:
			property pcre2_compile_context* Context {
				pcre2_compile_context* get() { return _ctx; }
			}

		private:
			initonly System::String^ _pattern;
			pcre2_compile_context* _ctx;
		};
	}
}
