using PCRE.Internal;
using PCRE.Support;

namespace PCRE
{
    internal interface IInternalRegexWrapper
    {
        RegexKey Key { get; }
        InternalRegex InternalRegex { get; }
    }
}
