
#pragma once

#include "Stdafx.h"

namespace PCRE {
	namespace Wrapper {

		public ref class JitStack sealed
		{
		public:
			__clrcall JitStack(uint32_t startSize, uint32_t maxSize);
			__clrcall ~JitStack();
			__clrcall !JitStack();

		internal:
			property pcre2_jit_stack* Stack {
				pcre2_jit_stack* __clrcall get() { return _stack; }
			}

		private:
			pcre2_jit_stack* _stack;
		};
	}
}
