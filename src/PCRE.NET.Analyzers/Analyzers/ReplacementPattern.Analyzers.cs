using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using PCRE.Analyzers;

// ReSharper disable once CheckNamespace
namespace PCRE.Internal;

internal partial class ReplacementPattern
{
    public static PatternModel? Parse(string? replacementPattern)
        => TryParse(replacementPattern, out var parts) ? new PatternModel(parts) : null;

    public sealed class PatternModel
    {
        private readonly List<ReplacementPart> _parts;

        public IReadOnlyList<ReplacementPart> Parts => _parts;
        public bool NeedsSubject { get; }

        public PatternModel(List<ReplacementPart> parts)
        {
            _parts = parts;
            NeedsSubject = _parts.Any(static i => i.NeedsSubject);
        }

        public string GetLambdaType()
            => NeedsSubject
                ? "global::System.Func<global::PCRE.PcreMatch, string, string>"
                : "global::System.Func<global::PCRE.PcreMatch, string>";

        public string GetLambda()
        {
            if (_parts.TrueForAll(static part => part is LiteralPart))
                return $"static _ => {SymbolDisplay.FormatLiteral(string.Concat(_parts.Cast<LiteralPart>().Select(static i => i.LiteralText)), true)}";

            var writer = new CodeWriter();

            writer.Append("static ")
                  .Append(NeedsSubject ? "(match, subject)" : "match")
                  .Append(" => $\"");

            foreach (var part in _parts)
                part.AppendCode(writer);

            return writer.Append('"').ToString();
        }
    }

    internal partial class ReplacementPart
    {
        public virtual bool NeedsSubject => false;

        public abstract void AppendCode(CodeWriter writer);
        public virtual void AppendHelpers(CodeWriter writer, ref AppendedHelpers appendedHelpers) { }

        protected static bool CanAppendHelper(AppendedHelpers helper, ref AppendedHelpers appendedHelpers)
        {
            if ((appendedHelpers & helper) != 0)
                return false;

            appendedHelpers |= helper;
            return true;
        }
    }

    internal partial class LiteralPart
    {
        public override void AppendCode(CodeWriter writer)
        {
            // NOTE: With `quote: false`, SymbolDisplay.FormatLiteral does not escape double quotes.
            var quotedValue = SymbolDisplay.FormatLiteral(LiteralText, true);

            writer.Append(
                quotedValue.Substring(1, quotedValue.Length - 2)
                           .Replace("{", "{{")
                           .Replace("}", "}}")
            );
        }
    }

    internal partial class IndexedGroupPart
    {
        public override void AppendCode(CodeWriter writer)
        {
            writer.Append($$"""{GetGroup(match, {{_index}})""");

            if (_fallback?.Length > 0)
                writer.Append(" ?? ").Append(SymbolDisplay.FormatLiteral(_fallback, true));

            writer.Append("}");
        }

        public override void AppendHelpers(CodeWriter writer, ref AppendedHelpers appendedHelpers)
        {
            if (!CanAppendHelper(AppendedHelpers.GetGroupByIndex, ref appendedHelpers))
                return;

            writer.AppendLine(
                """
                private static string? GetGroup(global::PCRE.PcreMatch match, int index)
                    => match.TryGetGroup(index, out var group) ? group.Value : null;

                """
            );
        }
    }

    internal partial class NamedGroupPart
    {
        public override void AppendCode(CodeWriter writer)
        {
            writer.Append(
                _index >= 0
                    ? $$"""{GetGroup(match, {{SymbolDisplay.FormatLiteral(_name, true)}}, {{_index}})"""
                    : $$"""{GetGroup(match, {{SymbolDisplay.FormatLiteral(_name, true)}})"""
            );

            if (_fallback?.Length > 0)
                writer.Append(" ?? ").Append(SymbolDisplay.FormatLiteral(_fallback, true));

            writer.Append("}");
        }

        public override void AppendHelpers(CodeWriter writer, ref AppendedHelpers appendedHelpers)
        {
            if (_index >= 0)
            {
                if (CanAppendHelper(AppendedHelpers.GetGroupByNameAndIndex, ref appendedHelpers))
                {
                    writer.AppendLine(
                        """
                         private static string? GetGroup(global::PCRE.PcreMatch match, string name, int index)
                             => match.TryGetGroup(name, out var group) || match.TryGetGroup(index, out group) ? group.Value : null;

                         """
                    );
                }
            }
            else
            {
                if (CanAppendHelper(AppendedHelpers.GetGroupByName, ref appendedHelpers))
                {
                    writer.AppendLine(
                        """
                        private static string? GetGroup(global::PCRE.PcreMatch match, string name)
                            => match.TryGetGroup(name, out var group) ? group.Value : null;

                        """
                    );
                }
            }
        }
    }

    internal partial class PreMatchPart
    {
        public override bool NeedsSubject => true;

        public override void AppendCode(CodeWriter writer)
            => writer.Append("{subject.Substring(0, match.Index)}");
    }

    internal partial class PostMatchPart
    {
        public override bool NeedsSubject => true;

        public override void AppendCode(CodeWriter writer)
            => writer.Append("{subject.Substring(match.EndIndex)}");
    }

    internal partial class FullInputPart
    {
        public override bool NeedsSubject => true;

        public override void AppendCode(CodeWriter writer)
            => writer.Append("{subject}");
    }

    internal partial class LastMatchedGroupPart
    {
        public override void AppendCode(CodeWriter writer)
            => writer.Append("{GetLastMatchedGroup(match)}");

        public override void AppendHelpers(CodeWriter writer, ref AppendedHelpers appendedHelpers)
        {
            if (!CanAppendHelper(AppendedHelpers.GetLastMatchedGroup, ref appendedHelpers))
                return;

            writer.AppendLine(
                """
                private static string GetLastMatchedGroup(global::PCRE.PcreMatch match)
                {
                    for (var i = match.CaptureCount; i > 0; --i)
                    {
                        if (match.TryGetGroup(i, out var group) && group.Success)
                            return group.Value;
                    }

                    return string.Empty;
                }

                """
            );
        }
    }

    [Flags]
    internal enum AppendedHelpers
    {
        None = 0,
        GetGroupByIndex = 1 << 0,
        GetGroupByName = 1 << 1,
        GetGroupByNameAndIndex = 1 << 2,
        GetLastMatchedGroup = 1 << 3
    }
}
