
#include "stdafx.h"
#include "PcrePattern.h"

namespace PCRE {
	namespace Wrapper {

		PcrePattern::PcrePattern(String^ pattern, PcrePatternOptions options, Nullable<PcreStudyOptions> studyOptions)
		{
			const char *errorMessage;
			int errorOffset;

			{
				pin_ptr<const wchar_t> pinnedPattern = PtrToStringChars(pattern);
				_re = pcre16_compile(
					safe_cast<const wchar_t*>(pinnedPattern),
					static_cast<int>(options),
					&errorMessage,
					&errorOffset,
					nullptr);
			}

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

			_captureCount = GetInfoInt32(PatternInfoKey::CaptureCount);
		}

		PcrePattern::~PcrePattern()
		{
			this->!PcrePattern();
		}

		PcrePattern::!PcrePattern()
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

		bool PcrePattern::IsMatch(String^ subject)
		{
			pin_ptr<const wchar_t> pinnedSubject = PtrToStringChars(subject);
			auto result = pcre16_exec(_re, _extra, pinnedSubject, subject->Length, 0, 0, nullptr, 0);

			if (result == PCRE_ERROR_NOMATCH)
				return false;

			if (result < 0)
				throw gcnew InvalidOperationException();

			return true;
		}

		MatchOffsets PcrePattern::FirstMatch(String^ subject)
		{
			return DoMatch(subject, 0, 0);
		}

		MatchOffsets PcrePattern::NextMatch(String^ subject, int startOffset)
		{
			if (startOffset == subject->Length)
				return MatchOffsets();

			return DoMatch(subject, startOffset, PCRE_NOTEMPTY_ATSTART);
		}

		MatchOffsets PcrePattern::DoMatch(String^ subject, int startOffset, int options)
		{
			auto match = MatchOffsets(_captureCount);

			pin_ptr<int> offsets = &match._offsets[0];
			pin_ptr<const wchar_t> pinnedSubject = PtrToStringChars(subject);

			auto result = pcre16_exec(_re, _extra, pinnedSubject, subject->Length, startOffset, options, offsets, match._offsets->Length);

			if (result < -1)
				throw gcnew InvalidOperationException();

			if (result == PCRE_ERROR_NOMATCH)
			{
				match.IsMatch = false;
				return match;
			}

			if (result < 0)
				throw gcnew InvalidOperationException();

			match.IsMatch = true;
			return match;
		}

		int PcrePattern::GetInfoInt32(PatternInfoKey key)
		{
			int result;
			auto errorCode = pcre16_fullinfo(_re, _extra, static_cast<int>(key), &result);

			if (errorCode)
				throw gcnew InvalidOperationException(String::Format("Error in pcre16_fullinfo, code: {0}", errorCode));

			return result;
		}

	}
}
