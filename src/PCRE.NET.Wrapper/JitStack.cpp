#include "stdafx.h"
#include "JitStack.h"

using namespace System;
using namespace PCRE::Wrapper;

JitStack::JitStack(uint32_t startSize, uint32_t maxSize)
{
	_stack = pcre2_jit_stack_create(startSize, maxSize, nullptr);
	if (!_stack)
		throw gcnew InvalidOperationException("Could not allocate JIT stack");
}

JitStack::~JitStack()
{
	this->!JitStack();
}

JitStack::!JitStack()
{
	if (_stack)
	{
		pcre2_jit_stack_free(_stack);
		_stack = nullptr;
	}
}
