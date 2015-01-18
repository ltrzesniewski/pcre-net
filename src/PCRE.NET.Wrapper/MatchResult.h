
#pragma once

#include "CalloutData.h"
#include "MatchResultCode.h"

using namespace System;
using namespace System::Diagnostics::Contracts;

namespace PCRE {
	namespace Wrapper {

		ref class InternalRegex;

		public ref class MatchResult sealed
		{
		public:
			MatchResult(InternalRegex^ const re, String^ subject, int offsetsSize);
			MatchResult(InternalRegex^ const re, String^ subject);
			MatchResult(MatchResult^ result);

			property MatchResultCode ResultCode {
				public: MatchResultCode get() { return _resultCode; }
				internal: void set(MatchResultCode value) { _resultCode = value; }
			}

			property int ResultCount {
				public: int get() { return _resultCount; }
				internal: void set(int value) { _resultCount = value; }
			}

			property InternalRegex^ Regex { InternalRegex^ get() { return _re; } }
			property String^ Subject { String^ get() { return _subject; } }
			property String^ Mark { String^ get(); }

			[Pure]
			int GetStartOffset(int index);

			[Pure]
			int GetEndOffset(int index);

			[Pure]
			int GetPartialStartOffset();

			[Pure]
			int GetPartialEndOffset();

			[Pure]
			int GetPartialScanStartOffset();

		internal:
			property Func<CalloutData^, CalloutResult>^ OnCallout;
			property Exception^ CalloutException;
			void SetMark(const PCRE_UCHAR16 *mark);
			
			array<int>^ _offsets;			

		private:
			initonly InternalRegex^ _re;
			initonly String^ _subject;
			MatchResultCode _resultCode;
			int _resultCount;
			String^ _mark;
			const PCRE_UCHAR16 *_markPtr;
		};
	}
}
