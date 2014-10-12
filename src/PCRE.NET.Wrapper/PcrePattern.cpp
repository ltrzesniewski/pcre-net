
#include "stdafx.h"
#include "PcrePattern.h"

namespace PCRE {
	namespace Wrapper {

		PcrePattern::PcrePattern(String^ pattern, int options)
		{
			const char *errorMessage;
			int errorOffset;

			{
				pin_ptr<const wchar_t> pinnedPattern = PtrToStringChars(pattern);
				_re = pcre16_compile(
					safe_cast<const wchar_t*>(pinnedPattern),
					options,
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

		}

		PcrePattern::~PcrePattern()
		{
			this->!PcrePattern();
		}

		PcrePattern::!PcrePattern()
		{
			if (_re)
			{
				pcre16_free(_re);
				_re = nullptr;
			}
		}

		bool PcrePattern::IsMatch(String^ subject)
		{
			pin_ptr<const wchar_t> pinnedSubject = PtrToStringChars(subject);
			auto result = pcre16_exec(_re, nullptr, pinnedSubject, subject->Length, 0, 0, nullptr, 0);

			if (result < -1)
				throw gcnew InvalidOperationException();

			return result != -1;
		}

	}
}
