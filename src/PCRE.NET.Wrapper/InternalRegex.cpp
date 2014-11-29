
#include "stdafx.h"
#include "InternalRegex.h"

namespace PCRE {
	namespace Wrapper {

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

			if (_re == nullptr)
			{
				if (!errorMessage)
					errorMessage = "Invalid pattern";

				throw gcnew ArgumentException(String::Format("Invalid pattern '{0}': {1} at offset {2}", pattern, gcnew String(errorMessage), errorOffset));
			}

			if (studyOptions.HasValue)
			{
				_extra = pcre16_study(_re, static_cast<int>(studyOptions.Value), &errorMessage);

				if (errorMessage)
					throw gcnew InvalidOperationException(String::Format("Could not study pattern '{0}': {1}", pattern, gcnew String(errorMessage)));
			}
			else
			{
				_extra = nullptr;
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

		MatchResult^ InternalRegex::Match(String^ subject, int startOffset, PatternOptions additionalOptions)
		{
			auto match = gcnew MatchResult(_captureCount);

			pin_ptr<int> offsets = &match->_offsets[0];
			pin_ptr<const wchar_t> pinnedSubject = PtrToStringChars(subject);

			auto result = pcre16_exec(_re, _extra, pinnedSubject, subject->Length, startOffset, (int)additionalOptions, offsets, match->_offsets->Length);

			if (result == PCRE_ERROR_NOMATCH)
				return match;

			if (result < 0)
				throw gcnew InvalidOperationException(String::Format("Match error, code: {0}", result));

			match->IsMatch = true;
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

	}
}
