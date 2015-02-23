
#pragma once

#include "CompileContextOptions.h"

using namespace System;

namespace PCRE {
	namespace Wrapper {

		public ref class CompileContext sealed
		{
		public:
			CompileContext();
			~CompileContext();
			!CompileContext();

		internal:
			property pcre2_compile_context* Context {
				pcre2_compile_context* get() { return _ctx; }
			}

		private:
			pcre2_compile_context* _ctx;
		};
	}
}
