
#include "stdafx.h"
#include <memory.h>
#include "InternalRegex.h"
#include "MatchData.h"
#include "MatchContext.h"
#include "InfoKey.h"
#include "PatternOptions.h"
#include "CalloutData.h"

namespace PCRE {
	namespace Wrapper {

		//int GlobalCalloutCallback(pcre16_callout_block *block);
		//
		//static InternalRegex::InternalRegex()
		//{
		//	pcre16_callout = &GlobalCalloutCallback;
		//}

		interior_ptr<const PCRE2_SPTR16> GetPtrToString(String^ string)
		{
			return reinterpret_cast<interior_ptr<const PCRE2_SPTR16>>(PtrToStringChars(string));
		}

		InternalRegex::InternalRegex(String^ pattern, PatternOptions options, JitCompileOptions jitCompileOptions)
		{
			int errorCode;
			PCRE2_SIZE errorOffset;
	
			pin_ptr<const PCRE2_SPTR16> pinnedPattern = GetPtrToString(pattern);

			_re = pcre2_compile(
				*pinnedPattern,
				pattern->Length,
				static_cast<int>(options),
				&errorCode,
				&errorOffset,
				nullptr);

			pinnedPattern = nullptr;

			if (!_re)
			{
				PCRE2_UCHAR16 errorBuffer[256];
				auto getErrorResult = pcre2_get_error_message(errorCode, errorBuffer, sizeof(errorBuffer));

				auto errorMessage = getErrorResult >= 0
					? gcnew String(reinterpret_cast<const wchar_t*>(errorBuffer))
					: "Invalid pattern";

				throw gcnew ArgumentException(String::Format("Invalid pattern '{0}': {1} at offset {2}", pattern, errorMessage, errorOffset));
			}

			//if (studyOptions.HasValue)
			//{
			//	_extra = pcre16_study(_re, static_cast<int>(studyOptions.Value) | PCRE_STUDY_EXTRA_NEEDED, &errorMessage);
			//
			//	if (errorMessage)
			//		throw gcnew InvalidOperationException(String::Format("Could not study pattern '{0}': {1}", pattern, gcnew String(errorMessage)));
			//}
			//else
			//{
			//	_extra = (pcre16_extra*)pcre16_malloc(sizeof(pcre16_extra));
			//	_extra->flags = 0;
			//}

			_captureCount = GetInfoInt32(InfoKey::CaptureCount);

			int nameCount = GetInfoInt32(InfoKey::NameCount);
			if (nameCount)
			{
				int nameEntrySize = GetInfoInt32(InfoKey::NameEntrySize);
				bool allowDuplicateNames = (options & PatternOptions::DupNames) != PatternOptions::None;

				_captureNames = gcnew Dictionary<String^, array<int>^>(nameCount, StringComparer::Ordinal);

				wchar_t *nameEntryTable;
				int errorCode = pcre2_pattern_info(_re, PCRE2_INFO_NAMETABLE, &nameEntryTable);
				if (errorCode || !nameEntryTable)
					throw gcnew InvalidOperationException(String::Format("Could not get name table, code: {0}", errorCode));

				wchar_t *item = nameEntryTable;
				for (int i = 0; i < nameCount; ++i)
				{
					int groupIndex = static_cast<short>(*item);
					String^ groupName = gcnew String(item + 1);
					array<int>^ indexes = nullptr;

					if (allowDuplicateNames && _captureNames->TryGetValue(groupName, indexes))
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

		//bool InternalRegex::IsMatch(String^ subject, int startOffset)
		//{
		//	pin_ptr<const wchar_t> pinnedSubject = PtrToStringChars(subject);
		//	auto result = pcre16_exec(_re, _extra, pinnedSubject, subject->Length, startOffset, 0, nullptr, 0);
		//	
		//	if (result == PCRE_ERROR_NOMATCH)
		//		return false;
		//	
		//	if (result < 0)
		//		throw gcnew InvalidOperationException(String::Format("Match error, code: {0}", result));
		//	
		//	return true;
		//}

		MatchData^ InternalRegex::Match(String^ subject, int startOffset, PatternOptions additionalOptions, Func<CalloutData^, CalloutResult>^ calloutCallback)
		{
			auto matchData = gcnew MatchData(this, subject);
			auto matchContext = gcnew MatchContext();
			//pin_ptr<MatchResult^> pinnedMatch;

			pin_ptr<const PCRE2_SPTR16> pinnedSubject = GetPtrToString(subject);

			//auto extra = *_extra;
			//PCRE_UCHAR16 *mark;
			//extra.mark = &mark;
			//extra.flags |= PCRE_EXTRA_MARK;

			//if (calloutCallback)
			//{
			//	pinnedMatch = &match;
			//	match->OnCallout = calloutCallback;
			//	extra.callout_data = pinnedMatch;
			//	extra.flags |= PCRE_EXTRA_CALLOUT_DATA;
			//
			//	// Initialize all offsets to -1 so we can tell which groups didn't match when in a callout
			//	memset(offsets, -1, sizeof(int) * 2 * (CaptureCount + 1));
			//}

			auto result = pcre2_match(_re, *pinnedSubject, subject->Length, startOffset, (int)additionalOptions, matchData->Block, matchContext->Context);
			//match->SetMark(mark);

			if (result >= 0)
			{
				matchData->ResultCode = MatchResultCode::Success;
			}
			else
			{
				matchData->ResultCode = static_cast<MatchResultCode>(result);

				switch (result)
				{
				case PCRE2_ERROR_NOMATCH:
				case PCRE2_ERROR_PARTIAL:
					break;

				case PCRE2_ERROR_CALLOUT:
					throw gcnew InvalidOperationException(String::Format("An exception was thrown by the callout: {0}", matchData->CalloutException ? matchData->CalloutException->Message : nullptr), matchData->CalloutException);
					break;

				default:
					throw gcnew InvalidOperationException(String::Format("Match error: {0}", matchData->ResultCode));
				}
			}

			return matchData;
		}

		int InternalRegex::GetInfoInt32(InfoKey key)
		{
			int result;
			int errorCode = pcre2_pattern_info(_re, static_cast<int>(key), &result);

			if (errorCode)
				throw gcnew InvalidOperationException(String::Format("Error in pcre2_pattern_info, code: {0}", errorCode));

			return result;
		}

		//static int GlobalCalloutCallback(pcre16_callout_block *block)
		//{
		//	if (!block->callout_data)
		//		return 0;

		//	auto match = *static_cast<interior_ptr<MatchResult^>>(block->callout_data);
		//	if (!match->OnCallout)
		//		return 0;

		//	try
		//	{
		//		match->CalloutException = nullptr;
		//		return static_cast<int>(match->OnCallout(gcnew CalloutData(match, block)));
		//	}
		//	catch (Exception^ ex)
		//	{
		//		match->CalloutException = ex;
		//		return PCRE_ERROR_CALLOUT;
		//	}
		//}
	}
}
