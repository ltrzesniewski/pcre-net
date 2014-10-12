
#pragma once

using namespace System;

namespace PCRE {
	namespace Wrapper {

		public ref class PcrePattern sealed
		{
		public:
			PcrePattern(String^ pattern, int options);

		private:
			pcre16* _re;
		};
	}
}
