
#include "stdafx.h"
#include <memory.h>
#include "InternalRegex.h"
#include "MatchData.h"
#include "MatchContext.h"
#include "InfoKey.h"
#include "PatternOptions.h"
#include "CalloutData.h"
#include "MatchException.h"

namespace PCRE {
	namespace Wrapper {

		static inline interior_ptr<const PCRE2_UCHAR> GetPtrToString(String^ string)
		{
			return reinterpret_cast<interior_ptr<const PCRE2_UCHAR>>(PtrToStringChars(string));
		}

		InternalRegex::InternalRegex(CompileContext^ context)
		{
			int errorCode;
			PCRE2_SIZE errorOffset;
	
			pin_ptr<const PCRE2_UCHAR> pinnedPattern = GetPtrToString(context->Pattern);

			_re = pcre2_compile(
				pinnedPattern,
				context->Pattern->Length,
				static_cast<int>(context->Options),
				&errorCode,
				&errorOffset,
				nullptr);

			pinnedPattern = nullptr;

			if (!_re)
			{
				PCRE2_UCHAR16 errorBuffer[256];
				auto errorMessage = pcre2_get_error_message(errorCode, errorBuffer, sizeof(errorBuffer)) >= 0
					? gcnew String(reinterpret_cast<const wchar_t*>(errorBuffer))
					: "Unknown error";

				throw gcnew ArgumentException(String::Format("Invalid pattern '{0}': {1} at offset {2}", context->Pattern, errorMessage, errorOffset));
			}

			if (context->JitCompileOptions != JitCompileOptions::None)
				pcre2_jit_compile(_re, static_cast<uint32_t>(context->JitCompileOptions));

			_captureCount = GetInfoInt32(InfoKey::CaptureCount);

			int nameCount = GetInfoInt32(InfoKey::NameCount);
			if (nameCount)
			{
				int nameEntrySize = GetInfoInt32(InfoKey::NameEntrySize);
				int effectiveOptions = GetInfoInt32(InfoKey::AllOptions);
				bool allowDuplicateNames = (effectiveOptions & PCRE2_DUPNAMES) != 0;

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

		MatchData^ InternalRegex::Match(MatchContext^ context)
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
					throw gcnew MatchException(matchData, String::Format("An exception was thrown by the callout: {0}", matchData->CalloutException ? matchData->CalloutException->Message : nullptr), matchData->CalloutException);
					break;

				default:
					{
						PCRE2_UCHAR16 errorBuffer[256];
						auto errorMessage = pcre2_get_error_message(result, errorBuffer, sizeof(errorBuffer)) >= 0
							? gcnew String(reinterpret_cast<const wchar_t*>(errorBuffer))
							: "Match error, code: " + result;
						throw gcnew MatchException(matchData, errorMessage, nullptr);
					}
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
	}
}
