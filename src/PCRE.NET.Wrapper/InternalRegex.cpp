
#include "stdafx.h"
#include "InternalRegex.h"
#include "MatchData.h"
#include "MatchException.h"
#include "CalloutInfo.h"
#include "Support.h"

using namespace System;
using namespace PCRE::Wrapper;

static void __clrcall AfterMatch(MatchData^);

InternalRegex::InternalRegex(CompileContext^ context)
{
	int errorCode;
	PCRE2_SIZE errorOffset;
	
	pin_ptr<const PCRE2_UCHAR> pinnedPattern = GetPtrToString(context->Pattern);

	_re = pcre2_compile(
		pinnedPattern,
		context->Pattern->Length,
		static_cast<uint32_t>(context->Options),
		&errorCode,
		&errorOffset,
		context->Context);

	pinnedPattern = nullptr;

	if (!_re)
		throw gcnew ArgumentException(String::Format("Invalid pattern '{0}': {1} at offset {2}", context->Pattern, GetPcreErrorMessage(errorCode), errorOffset));

	if (context->JitCompileOptions != JitCompileOptions::None)
		pcre2_jit_compile(_re, static_cast<uint32_t>(context->JitCompileOptions));

	_captureCount = GetInfoUInt32(InfoKey::CaptureCount);

	uint32_t nameCount = GetInfoUInt32(InfoKey::NameCount);
	if (nameCount)
	{
		uint32_t nameEntrySize = GetInfoUInt32(InfoKey::NameEntrySize);

		_captureNames = gcnew System::Collections::Generic::Dictionary<String^, array<int>^>(nameCount, StringComparer::Ordinal);

		wchar_t *nameEntryTable;
		int errorCode = pcre2_pattern_info(_re, PCRE2_INFO_NAMETABLE, &nameEntryTable);
		if (errorCode || !nameEntryTable)
			throw gcnew InvalidOperationException(String::Format("Could not get name table, code: {0}", errorCode));

		wchar_t *item = nameEntryTable;
		for (uint32_t i = 0; i < nameCount; ++i)
		{
			int groupIndex = static_cast<short>(*item);
			String^ groupName = gcnew String(item + 1);
			array<int>^ indexes = nullptr;

			if (_captureNames->TryGetValue(groupName, indexes))
			{
				Array::Resize(indexes, indexes->Length + 1);
				indexes[indexes->Length - 1] = groupIndex;
				_captureNames[groupName] = indexes;
			}
			else
			{
				_captureNames->Add(groupName, gcnew array<int> { groupIndex });
			}
					
			item += nameEntrySize;
		}
	}
}

InternalRegex::~InternalRegex()
{
	this->!InternalRegex();
}

InternalRegex::!InternalRegex()
{
	if (_re)
	{
		pcre2_code_free(_re);
		_re = nullptr;
	}
}

MatchData^ __clrcall InternalRegex::Match(MatchContext^ context)
{
	auto matchData = gcnew MatchData(this, context->Subject);
	context->Match = matchData;

	pin_ptr<MatchContext^> pinnedContext;
	pin_ptr<const PCRE2_UCHAR> pinnedSubject = GetPtrToString(context->Subject);

	if (context->CalloutHandler)
	{
		pinnedContext = &context;
		context->EnableCallout(pinnedContext);
	}

	int result = pcre2_match(
		_re,
		pinnedSubject,
		context->Subject->Length,
		context->StartIndex,
		static_cast<int>(context->AdditionalOptions),
		matchData->Block,
		context->Context);

	matchData->RawResultCode = result;
	AfterMatch(matchData);

	return matchData;
}

MatchData^ __clrcall InternalRegex::DfaMatch(MatchContext^ context)
{
	auto matchData = gcnew MatchData(this, context->Subject, Math::Max(1u, context->DfaMaxResults));
	context->Match = matchData;

	pin_ptr<MatchContext^> pinnedContext;
	pin_ptr<const PCRE2_UCHAR> pinnedSubject = GetPtrToString(context->Subject);

	if (context->CalloutHandler)
	{
		pinnedContext = &context;
		context->EnableCallout(pinnedContext);
	}
			
	auto workspace = gcnew array<int>(Math::Max(20u, context->DfaWorkspaceSize));
	pin_ptr<int> pinnedWorkspace = &workspace[0];

	int result = pcre2_dfa_match(
		_re,
		pinnedSubject,
		context->Subject->Length,
		context->StartIndex,
		static_cast<int>(context->AdditionalOptions),
		matchData->Block,
		context->Context,
		pinnedWorkspace,
		workspace->Length);

	matchData->RawResultCode = result;
	AfterMatch(matchData);

	return matchData;
}

static void __clrcall AfterMatch(MatchData^ matchData)
{
	matchData->ResultCode = static_cast<MatchResultCode>(matchData->RawResultCode);

	switch (matchData->ResultCode)
	{
	case MatchResultCode::NoMatch:
	case MatchResultCode::Partial:
		break;

	case MatchResultCode::Callout:
		throw gcnew MatchException(
			matchData,
			String::Format("An exception was thrown by the callout: {0}", matchData->CalloutException ? matchData->CalloutException->Message : nullptr),
			matchData->CalloutException);

	default:
		auto intResult = static_cast<int>(matchData->ResultCode);
		if (intResult >= 0)
		{
			matchData->ResultCode = MatchResultCode::Success;
			break;
		}

		throw gcnew MatchException(matchData, GetPcreErrorMessage(intResult), nullptr);
	}
}

template<typename T>
T __clrcall InternalRegex::GetInfo(InfoKey key)
{
	T result;
	int errorCode = pcre2_pattern_info(_re, static_cast<int>(key), &result);

	if (errorCode)
		throw gcnew InvalidOperationException(String::Format("Error in pcre2_pattern_info: {0}", GetPcreErrorMessage(errorCode)));

	return result;
}

System::Collections::Generic::IList<CalloutInfo^>^ InternalRegex::Callouts::get()
{
	if (_callouts == nullptr)
	{
		auto callouts = CalloutInfo::GetCallouts(_re);
		System::Threading::Volatile::Write(_callouts, callouts);
		return callouts;
	}
	
	return _callouts;
}

CalloutInfo^ InternalRegex::GetCalloutInfoByPatternPosition(int position)
{
	if (_calloutInfoByPatternPosition == nullptr)
	{
		auto dict = gcnew System::Collections::Generic::Dictionary<int, CalloutInfo^>();
		for each(auto info in Callouts)
			dict->Add(info->PatternPosition, info);
		System::Threading::Volatile::Write(_calloutInfoByPatternPosition, dict);
	}

	CalloutInfo^ result;
	_calloutInfoByPatternPosition->TryGetValue(position, result);
	return result;
}
