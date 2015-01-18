
#include "stdafx.h"
#include <memory.h>
#include "InternalRegex.h"
#include "MatchResult.h"
#include "InfoKey.h"
#include "PatternOptions.h"
#include "CalloutData.h"

namespace PCRE {
	namespace Wrapper {

		int GlobalCalloutCallback(pcre16_callout_block *block);

		static InternalRegex::InternalRegex()
		{
			pcre16_callout = &GlobalCalloutCallback;
		}

		InternalRegex::InternalRegex(String^ pattern, PatternOptions options, Nullable<StudyOptions> studyOptions)
		{
			const char *errorMessage;
			int errorOffset;
	
			pin_ptr<const wchar_t> pinnedPattern = PtrToStringChars(pattern);

			_re = pcre16_compile(
				safe_cast<const wchar_t*>(pinnedPattern),
				static_cast<int>(options),
				&errorMessage,
				&errorOffset,
				nullptr);

			pinnedPattern = nullptr;

			if (!_re)
			{
				if (!errorMessage)
					errorMessage = "Invalid pattern";

				throw gcnew ArgumentException(String::Format("Invalid pattern '{0}': {1} at offset {2}", pattern, gcnew String(errorMessage), errorOffset));
			}

			if (studyOptions.HasValue)
			{
				_extra = pcre16_study(_re, static_cast<int>(studyOptions.Value) | PCRE_STUDY_EXTRA_NEEDED, &errorMessage);

				if (errorMessage)
					throw gcnew InvalidOperationException(String::Format("Could not study pattern '{0}': {1}", pattern, gcnew String(errorMessage)));
			}
			else
			{
				_extra = (pcre16_extra*)pcre16_malloc(sizeof(pcre16_extra));
				_extra->flags = 0;
			}

			_captureCount = GetInfoInt32(InfoKey::CaptureCount);

			int nameCount = GetInfoInt32(InfoKey::NameCount);
			if (nameCount)
			{
				int nameEntrySize = GetInfoInt32(InfoKey::NameEntrySize);
				bool allowDuplicateNames = (options & PatternOptions::DupNames) != PatternOptions::None;

				_captureNames = gcnew Dictionary<String^, array<int>^>(nameCount, StringComparer::Ordinal);

				wchar_t *nameEntryTable;
				int errorCode = pcre16_fullinfo(_re, _extra, PCRE_INFO_NAMETABLE, &nameEntryTable);
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
			if (_extra)
			{
				pcre16_free_study(_extra);
				_extra = nullptr;
			}

			if (_re)
			{
				pcre16_free(_re);
				_re = nullptr;
			}
		}

		bool InternalRegex::IsMatch(String^ subject, int startOffset)
		{
			pin_ptr<const wchar_t> pinnedSubject = PtrToStringChars(subject);
			auto result = pcre16_exec(_re, _extra, pinnedSubject, subject->Length, startOffset, 0, nullptr, 0);

			if (result == PCRE_ERROR_NOMATCH)
				return false;

			if (result < 0)
				throw gcnew InvalidOperationException(String::Format("Match error, code: {0}", result));

			return true;
		}

		MatchResult^ InternalRegex::Match(String^ subject, int startOffset, PatternOptions additionalOptions, Func<CalloutData^, CalloutResult>^ calloutCallback)
		{
			auto match = gcnew MatchResult(this, subject);
			pin_ptr<MatchResult^> pinnedMatch;

			pin_ptr<int> offsets = &match->_offsets[0];
			pin_ptr<const wchar_t> pinnedSubject = PtrToStringChars(subject);

			auto extra = *_extra;
			PCRE_UCHAR16 *mark;
			extra.mark = &mark;
			extra.flags |= PCRE_EXTRA_MARK;

			if (calloutCallback)
			{
				pinnedMatch = &match;
				match->OnCallout = calloutCallback;
				extra.callout_data = pinnedMatch;
				extra.flags |= PCRE_EXTRA_CALLOUT_DATA;

				// Initialize all offsets to -1 so we can tell which groups didn't match when in a callout
				memset(offsets, -1, sizeof(int) * 2 * (CaptureCount + 1));
			}

			auto result = pcre16_exec(_re, &extra, pinnedSubject, subject->Length, startOffset, (int)additionalOptions, offsets, match->_offsets->Length);
			match->SetMark(mark);

			if (result >= 0)
			{
				match->ResultCode = MatchResultCode::Success;	
				match->ResultCount = result;
			}
			else
			{
				match->ResultCode = static_cast<MatchResultCode>(result);

				switch (result)
				{
				case PCRE_ERROR_NOMATCH:
				case PCRE_ERROR_PARTIAL:
					break;

				case PCRE_ERROR_CALLOUT:
					throw gcnew InvalidOperationException(String::Format("An exception was thrown by the callout: {0}", match->CalloutException ? match->CalloutException->Message : nullptr), match->CalloutException);
					break;

				default:
					throw gcnew InvalidOperationException(String::Format("Match error: {0}", match->ResultCode));
				}
			}

			return match;
		}

		MatchResult^ InternalRegex::DfaMatch(String^ subject, int startOffset, int maxMatches, PatternOptions additionalOptions, Func<CalloutData^, CalloutResult>^ calloutCallback)
		{
			// TODO : For now this is mostly copy/pasted to make experimenting easier. Refactor it.

			auto match = gcnew MatchResult(this, subject, maxMatches * 2);
			pin_ptr<MatchResult^> pinnedMatch;

			pin_ptr<int> offsets = &match->_offsets[0];
			pin_ptr<const wchar_t> pinnedSubject = PtrToStringChars(subject);

			auto extra = *_extra;
			PCRE_UCHAR16 *mark;
			extra.mark = &mark;
			extra.flags |= PCRE_EXTRA_MARK;

			if (calloutCallback)
			{
				pinnedMatch = &match;
				match->OnCallout = calloutCallback;
				extra.callout_data = pinnedMatch;
				extra.flags |= PCRE_EXTRA_CALLOUT_DATA;

				// Initialize all offsets to -1 so we can tell which groups didn't match when in a callout
				memset(offsets, -1, sizeof(int) * 2 * (CaptureCount + 1));
			}

			const int wspaceSize = 256;
			int wspace[wspaceSize];

			auto result = pcre16_dfa_exec(_re, &extra, pinnedSubject, subject->Length, startOffset, (int)additionalOptions, offsets, match->_offsets->Length, wspace, wspaceSize);
			match->SetMark(mark);

			if (result >= 0)
			{
				match->ResultCode = MatchResultCode::Success;
				match->ResultCount = result;
			}
			else
			{
				match->ResultCode = static_cast<MatchResultCode>(result);

				switch (result)
				{
				case PCRE_ERROR_NOMATCH:
				case PCRE_ERROR_PARTIAL:
					break;

				case PCRE_ERROR_CALLOUT:
					throw gcnew InvalidOperationException(String::Format("An exception was thrown by the callout: {0}", match->CalloutException ? match->CalloutException->Message : nullptr), match->CalloutException);
					break;

				default:
					throw gcnew InvalidOperationException(String::Format("Match error: {0}", match->ResultCode));
				}
			}

			return match;
		}

		int InternalRegex::GetInfoInt32(InfoKey key)
		{
			int result;
			int errorCode = pcre16_fullinfo(_re, _extra, static_cast<int>(key), &result);

			if (errorCode)
				throw gcnew InvalidOperationException(String::Format("Error in pcre16_fullinfo, code: {0}", errorCode));

			return result;
		}

		static int GlobalCalloutCallback(pcre16_callout_block *block)
		{
			if (!block->callout_data)
				return 0;

			auto match = *static_cast<interior_ptr<MatchResult^>>(block->callout_data);
			if (!match->OnCallout)
				return 0;

			try
			{
				match->CalloutException = nullptr;
				return static_cast<int>(match->OnCallout(gcnew CalloutData(match, block)));
			}
			catch (Exception^ ex)
			{
				match->CalloutException = ex;
				return PCRE_ERROR_CALLOUT;
			}
		}
	}
}
