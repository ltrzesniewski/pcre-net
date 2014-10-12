
#include "stdafx.h"

#include "PCRE.NET.Wrapper.h"

namespace PCRE {
	namespace Wrapper {

		String^ PcreWrapper::GetVersionString()
		{
			return gcnew String(pcre16_version());
		}

	}
}
