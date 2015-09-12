
#include "stdafx.h"
#include "CalloutInfo.h"

using namespace PCRE::Wrapper;

CalloutInfo::CalloutInfo(pcre2_callout_enumerate_block *block)
	: _patternPosition(static_cast<int>(block->pattern_position)),
	_nextItemLength(static_cast<int>(block->next_item_length)),
	_number(static_cast<int>(block->callout_number)),
	_stringOffset(static_cast<int>(block->callout_string_offset)),
	_string(block->callout_string != nullptr
		? msclr::interop::marshal_as<System::String^>(reinterpret_cast<const wchar_t*>(block->callout_string))
		: nullptr)
{
}

static int CalloutEnumerateCallback(pcre2_callout_enumerate_block* block, void* data)
{
	auto list = *static_cast<interior_ptr<System::Collections::Generic::List<CalloutInfo^>^>>(data);
	list->Add(gcnew CalloutInfo(block));
	return 0;
}

System::Collections::Generic::IList<CalloutInfo^>^ CalloutInfo::GetCallouts(pcre2_code* re)
{
	auto list = gcnew System::Collections::Generic::List<CalloutInfo^>();
	pin_ptr<System::Collections::Generic::List<CalloutInfo^>^> pinnedList = &list;

	pcre2_callout_enumerate(re, &CalloutEnumerateCallback, static_cast<void*>(pinnedList));
	return list->AsReadOnly();
}
