
#include "stdafx.h"
#include "PcrePattern.h"

namespace PCRE {
	namespace Wrapper {

		static int TranslateOptions(int patternOptions)
		{
			int result = PCRE_NO_UTF16_CHECK | PCRE_UCP;

			if (patternOptions & 1)
				result |= PCRE_CASELESS;

			if (patternOptions & 2)
				result |= PCRE_MULTILINE;

			if (patternOptions & 4)
				result |= PCRE_NO_AUTO_CAPTURE;

			if (patternOptions & 16)
				result |= PCRE_DOTALL;

			if (patternOptions & 32)
				result |= PCRE_EXTENDED;

			if (patternOptions & 256)
			{
				result |= PCRE_JAVASCRIPT_COMPAT;
				result &= ~PCRE_UCP;
			}

			return result;
		}

		PcrePattern::PcrePattern(String^ pattern, int options)
		{
			const char *errorMessage;
			int errorOffset;

			{
				pin_ptr<const wchar_t> pinnedPattern = PtrToStringChars(pattern);
				_re = pcre16_compile(
					safe_cast<const wchar_t*>(pinnedPattern),
					TranslateOptions(options),
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

			// Study

			if (options & (8 | 4096))
			{
				int studyOptions = 0;

				if (options & 8)
					studyOptions |= PCRE_STUDY_JIT_COMPILE | PCRE_STUDY_JIT_PARTIAL_HARD_COMPILE | PCRE_STUDY_JIT_PARTIAL_SOFT_COMPILE;

				_extra = pcre16_study(_re, studyOptions, &errorMessage);

				if (errorMessage)
					throw gcnew InvalidOperationException(String::Format("Could not study pattern '{0}': {1}", pattern, gcnew String(errorMessage)));
			}
			else
			{
				_extra = nullptr;
			}
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
			auto result = pcre16_exec(_re, _extra, pinnedSubject, subject->Length, 0, PCRE_NO_UTF16_CHECK, nullptr, 0);

			if (result < -1)
				throw gcnew InvalidOperationException();

			return result != -1;
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
