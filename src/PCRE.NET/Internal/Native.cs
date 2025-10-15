using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace PCRE.Internal;

[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
internal static unsafe partial class Native8Bit
{
    public static string GetErrorMessage(int errorCode)
    {
        const int bufferSize = 512;
        var buffer = stackalloc byte[bufferSize];
        var messageLength = get_error_message(errorCode, buffer, bufferSize);
        return messageLength >= 0
            ? Encoding.ASCII.GetString(buffer, messageLength)
            : $"Unknown error, code: {errorCode}";
    }
}

[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
internal static unsafe partial class Native16Bit
{
    public static string GetErrorMessage(int errorCode)
    {
        const int bufferSize = 256;
        var buffer = stackalloc char[bufferSize];
        var messageLength = get_error_message(errorCode, buffer, bufferSize);
        return messageLength >= 0
            ? new string(buffer, 0, messageLength)
            : $"Unknown error, code: {errorCode}";
    }
}
