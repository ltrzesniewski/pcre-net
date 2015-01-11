namespace PCRE
{
    public interface IPcreGroup
    {
        int Index { get; }
        int EndIndex { get; }
        int Length { get; }
        string Value { get; }
        bool Success { get; }
    }
}
