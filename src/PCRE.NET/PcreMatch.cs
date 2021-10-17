using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using PCRE.Internal;

namespace PCRE
{
    /// <summary>
    /// The result of a match.
    /// </summary>
    public sealed unsafe class PcreMatch : IPcreGroup, IPcreGroupList
    {
        private readonly InternalRegex _regex;
        private readonly int _resultCode;
        private readonly uint[] _oVector;
        private readonly char* _markPtr;

        private PcreGroup?[]? _groups;
        private string? _mark;

        internal PcreMatch(InternalRegex regex)
        {
            // No match

            Subject = string.Empty;
            _regex = regex;
            _oVector = Array.Empty<uint>();
            _resultCode = PcreConstants.ERROR_NOMATCH;
        }

        internal PcreMatch(string subject, InternalRegex regex, ref Native.match_result result, uint[] oVector)
        {
            // Real match

            Subject = subject;
            _regex = regex;
            _oVector = oVector;
            _markPtr = result.mark;

            _resultCode = result.result_code;
        }

        internal PcreMatch(string subject, InternalRegex regex, uint[] oVector, char* mark)
        {
            // Callout

            Subject = subject;
            _regex = regex;
            _oVector = oVector;
            _markPtr = mark;

            _resultCode = oVector.Length / 2;
        }

        /// <summary>
        /// The number of captured groups.
        /// </summary>
        public int CaptureCount => _regex.CaptureCount;

        /// <summary>
        /// Returns the capturing group at a given index.
        /// </summary>
        /// <param name="index">The index of the capturing group.</param>
        public PcreGroup this[int index] => GetGroup(index) ?? PcreGroup.Undefined;

        /// <summary>
        /// Returns the capturing group of a given name.
        /// </summary>
        /// <param name="name">The name of the capturing group.</param>
        public PcreGroup this[string name] => GetGroup(name) ?? PcreGroup.Undefined;

        internal string Subject { get; }

        /// <summary>
        /// The start index of the match.
        /// </summary>
        public int Index => this[0].Index;

        /// <summary>
        /// The end index of the match.
        /// </summary>
        public int EndIndex => this[0].EndIndex;

        /// <summary>
        /// The length of the match.
        /// </summary>
        public int Length => this[0].Length;

        /// <summary>
        /// The matched substring.
        /// </summary>
        public string Value => this[0].Value;

        /// <summary>
        /// Indicates of the match was successful.
        /// </summary>
        public bool Success => _resultCode > 0;

        /// <summary>
        /// Returns the last mark name encountered on the matching path through the pattern.
        /// </summary>
        /// <remarks>
        /// Marks are defined with <c>(*MARK)</c>.
        /// </remarks>
        public string? Mark
        {
            get
            {
                if (_mark == null && _markPtr != null)
                    _mark = new string(_markPtr);

                return _mark;
            }
        }

        /// <summary>
        /// Returns the list of capturing groups.
        /// </summary>
        public IPcreGroupList Groups => this;

        /// <summary>
        /// Indicates if the match is partial. See <see cref="PcreMatchOptions.PartialSoft"/>/<see cref="PcreMatchOptions.PartialHard"/>.
        /// </summary>
        public bool IsPartialMatch => _resultCode == PcreConstants.ERROR_PARTIAL;

        /// <summary>
        /// Returns an enumerator through the capturing groups.
        /// </summary>
        public IEnumerator<PcreGroup> GetEnumerator()
            => GetAllGroups().GetEnumerator();

        private IEnumerable<PcreGroup> GetAllGroups()
        {
            for (var i = 0; i <= CaptureCount; ++i)
                yield return this[i];
        }

        /// <summary>
        /// Tries to get a group by index.
        /// </summary>
        /// <param name="index">The index of the group to retrieve.</param>
        /// <param name="result">The resulting group.</param>
        /// <returns>True if the group exists.</returns>
        public bool TryGetGroup(int index, [MaybeNullWhen(false)] out PcreGroup result)
        {
            result = GetGroup(index);
            return result != null;
        }

        /// <summary>
        /// Tries to get a group by name.
        /// </summary>
        /// <param name="name">The name of the group to retrieve.</param>
        /// <param name="result">The resulting group.</param>
        /// <returns>True if the group exists.</returns>
        public bool TryGetGroup(string name, [MaybeNullWhen(false)] out PcreGroup result)
        {
            result = GetGroup(name);
            return result != null;
        }

        private PcreGroup? GetGroup(int index)
        {
            if (index < 0 || index > CaptureCount)
                return null;

            var groups = _groups;

            if (groups is null)
            {
                if (_oVector.Length == 0)
                    return PcreGroup.Empty; // No match

                groups = _groups = new PcreGroup?[_regex.CaptureCount + 1];
            }

            var group = groups[index];
            if (group is null)
                groups[index] = group = CreateGroup(index);

            return group;
        }

        private PcreGroup CreateGroup(int index)
        {
            var isAvailable = index < _resultCode || IsPartialMatch && index == 0;

            if (!isAvailable)
                return PcreGroup.Empty;

            index *= 2;
            if (index >= _oVector.Length)
                return PcreGroup.Empty;

            var startOffset = (int)_oVector[index];
            if (startOffset < 0)
                return PcreGroup.Empty;

            var endOffset = (int)_oVector[index + 1];

            return new PcreGroup(Subject, startOffset, endOffset);
        }

        private PcreGroup? GetGroup(string name)
        {
            if (!_regex.CaptureNames.TryGetValue(name, out var indexes))
                return null;

            if (indexes.Length == 1)
                return GetGroup(indexes[0]);

            foreach (var index in indexes)
            {
                var group = GetGroup(index);
                if (group != null && group.Success)
                    return group;
            }

            return PcreGroup.Empty;
        }

        /// <summary>
        /// Gets the groups with duplicated names.
        /// </summary>
        /// <param name="name">The group name to retrieve.</param>
        public IEnumerable<PcreGroup> GetDuplicateNamedGroups(string name)
        {
            if (!_regex.CaptureNames.TryGetValue(name, out var indexes))
                yield break;

            foreach (var index in indexes)
            {
                var group = GetGroup(index);
                if (group != null)
                    yield return group;
            }
        }

        internal int GetStartOfNextMatchIndex()
        {
            // It's possible to have EndIndex < Index
            // when the pattern contains \K in a lookahead
            return Math.Max(Index, EndIndex);
        }

        /// <summary>
        /// Returns the matched substring.
        /// </summary>
        public override string ToString()
            => Value;

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        int IReadOnlyCollection<PcreGroup>.Count => CaptureCount + 1;
    }
}
