using System.Collections;
using System.Collections.Generic;
using PCRE.Internal;

namespace PCRE.Dfa;

/// <summary>
/// Represents the result of one execution of the DFA algorithm. This contains several matches
/// that start at the same index in the subject string. The longest match is returned first.
/// </summary>
public sealed class PcreDfaMatchResult : IReadOnlyList<PcreDfaMatch>
{
    private readonly nuint[] _oVector;
    private readonly int _resultCode;
    private readonly PcreDfaMatch?[] _matches;

    internal string Subject { get; }

    internal PcreDfaMatchResult(string subject, ref Native16Bit.match_result result, nuint[] oVector)
    {
        // Real match

        Subject = subject;
        _oVector = oVector;

        _resultCode = result.result_code;

        _matches = _resultCode switch
        {
            > 0 => new PcreDfaMatch?[_resultCode],
            0   => new PcreDfaMatch?[_oVector.Length / 2],
            _   => []
        };
    }

    private PcreDfaMatch GetMatch(int index)
    {
        if (index < 0 || index >= Count)
            return PcreDfaMatch.Empty;

        var match = _matches[index];
        if (match == null)
            _matches[index] = match = CreateMatch(index);

        return match;
    }

    private PcreDfaMatch CreateMatch(int index)
    {
        index *= 2;
        if (index >= _oVector.Length)
            return PcreDfaMatch.Empty;

        var startOffset = (int)_oVector[index];
        if (startOffset < 0)
            return PcreDfaMatch.Empty;

        var endOffset = (int)_oVector[index + 1];

        return new PcreDfaMatch(Subject, startOffset, endOffset);
    }

    private IEnumerable<PcreDfaMatch> GetMatches()
    {
        for (var i = 0; i < Count; ++i)
            yield return GetMatch(i);
    }

    /// <summary>
    /// Enumerates the matches, from longest to shortest.
    /// </summary>
    /// <returns></returns>
    public IEnumerator<PcreDfaMatch> GetEnumerator()
        => GetMatches().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    /// <summary>
    /// Returns the match at a given index.
    /// </summary>
    /// <param name="index">The index of the match.</param>
    public PcreDfaMatch this[int index]
        => GetMatch(index);

    /// <summary>
    /// The available match count.
    /// </summary>
    public int Count => _matches.Length;

    /// <summary>
    /// Indicates if the match was successful.
    /// </summary>
    public bool Success => _resultCode >= 0;

    /// <summary>
    /// The starting index of the matches.
    /// </summary>
    public int Index => LongestMatch.Index;

    /// <summary>
    /// Returns the longest match.
    /// </summary>
    public PcreDfaMatch LongestMatch => GetMatch(0);

    /// <summary>
    /// Returns the shortest match.
    /// </summary>
    public PcreDfaMatch ShortestMatch => GetMatch(Count - 1);

    /// <summary>
    /// Returns the substring of the longest match in the subject string.
    /// </summary>
    public override string ToString()
        => LongestMatch.Value;
}
