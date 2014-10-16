
#pragma once

using namespace System;
using namespace System::Diagnostics::Contracts;

namespace PCRE {
	namespace Wrapper {

		public value struct MatchOffsets
		{
		public:
			MatchOffsets(int captureCount);

			property bool IsMatch {
				public: bool get() { return _isMatch; }
				internal: void set(bool value) { _isMatch = value; }
			}

			[Pure]
			int GetStartOffset(int index);

			[Pure]
			int GetEndOffset(int index);

		internal:
			array<int>^ _offsets;

		private:
			bool _isMatch;
		};
	}
}
