
#include "stdafx.h"
#include "CompileContext.h"

namespace PCRE {
	namespace Wrapper {

		CompileContext::CompileContext()
		{
			_ctx = pcre2_compile_context_create(nullptr);
		}

		CompileContext::~CompileContext()
		{
			this->!CompileContext();
		}

		CompileContext::!CompileContext()
		{
			if (_ctx)
			{
				pcre2_compile_context_free(_ctx);
				_ctx = nullptr;
			}
		}
	}
}
