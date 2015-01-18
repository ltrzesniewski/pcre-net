using PCRE.Support;
using PCRE.Wrapper;

namespace PCRE
{
    internal interface IInternalRegexWrapper
    {
        RegexKey Key { get; }
        InternalRegex InternalRegex { get; }
    }
}
