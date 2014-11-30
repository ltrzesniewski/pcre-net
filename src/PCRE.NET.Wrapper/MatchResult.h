
#pragma once

#include "CalloutData.h"

using namespace System;
using namespace System::Diagnostics::Contracts;

namespace PCRE {
	namespace Wrapper {

		ref class InternalRegex;

		public ref class MatchResult sealed
		{
		public:
			MatchResult(InternalRegex^ const re, String^ subject);
			MatchResult(MatchResult^ result);

			property bool IsMatch {
				public: bool get() { return _isMatch; }
				internal: void set(bool value) { _isMatch = value; }
			}

			property InternalRegex^ Regex { InternalRegex^ get() { return _re; } }
			property String^ Subject { String^ get() { return _subject; } }
			property String^ Mark { String^ get(); }

			[Pure]
			int GetStartOffset(int index);

			[Pure]
			int GetEndOffset(int index);

		internal:
			property Func<CalloutData^, CalloutResult>^ OnCallout;
			property Exception^ CalloutException;
			void SetMark(const PCRE_UCHAR16 *mark);
			
			array<int>^ _offsets;			

		private:
			initonly InternalRegex^ _re;
			initonly String^ _subject;
			bool _isMatch;
			String^ _mark;
			const PCRE_UCHAR16 *_markPtr;
		};
	}
}
