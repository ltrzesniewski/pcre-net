
#pragma once

using namespace System;

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

		internal:
			array<int>^ _offsets;

		private:
			bool _isMatch;
		};
	}
}
