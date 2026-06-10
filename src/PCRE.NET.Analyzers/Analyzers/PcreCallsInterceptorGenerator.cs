using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace PCRE.Analyzers;

[Generator(LanguageNames.CSharp)]
public sealed class PcreCallsInterceptorGenerator : IIncrementalGenerator
{
    [SuppressMessage("ReSharper", "UseCollectionExpression")]
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var invocations = context.SyntaxProvider
                                 .CreateSyntaxProvider(SyntaxPredicate, SyntaxTransform);

        var isEnabled = context.ParseOptionsProvider
                               .Select(static (parseOptions, _) => parseOptions.Features.TryGetValue("InterceptorsNamespaces", out var value) ? value : string.Empty)
                               .Select(static (namespaces, _) => namespaces.Split(';').Any(static i => i.Trim() == "PCRE.Generated"));

        var languageVersion = context.CompilationProvider
                                     .Select(static (compilation, _) => ((CSharpCompilation)compilation).LanguageVersion);

        var pipeline = invocations.Combine(isEnabled)
                                  .SelectMany(static (pair, _) =>
                                  {
                                      var (invocation, isEnabled) = pair;
                                      return isEnabled && invocation is not null
                                          ? ImmutableArray.Create(invocation)
                                          : ImmutableArray<InvocationModel>.Empty;
                                  })
                                  .Collect()
                                  .Combine(languageVersion);

        context.RegisterImplementationSourceOutput(
            pipeline,
            static (context, pair) =>
            {
                var (invocations, languageVersion) = pair;

                if (invocations.Length == 0)
                    return;

                var generator = new Generator(languageVersion);

                context.AddSource(
                    "PcreCallsInterceptor.g.cs",
                    generator.Generate(invocations)
                );
            }
        );
    }

    private static bool SyntaxPredicate(SyntaxNode node, CancellationToken cancellationToken)
    {
        if (!node.IsKind(SyntaxKind.InvocationExpression))
            return false;

        var methodName = (node as InvocationExpressionSyntax)?.Expression switch
        {
            MemberAccessExpressionSyntax i => i.Name.Identifier.ValueText,
            IdentifierNameSyntax i         => i.Identifier.ValueText,
            _                              => null
        };

        return methodName is "Match" or "IsMatch" or "Matches" or "Replace" or "Split" or "Substitute";
    }

    private static InvocationModel? SyntaxTransform(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        if (context.Node is InvocationExpressionSyntax invocationSyntax
            && context.SemanticModel.GetOperation(invocationSyntax, cancellationToken) is IInvocationOperation
            {
                TargetMethod:
                {
                    ContainingType.Name: "PcreRegex"
                } method
            } invocation
            && context.SemanticModel.GetInterceptableLocation(invocationSyntax, cancellationToken) is { } location)
        {
            if (method.IsStatic)
            {
                if (invocation.GetArgument("pattern")?.Value.ConstantValue is { HasValue: true, Value: string pattern }
                    && invocation.GetArgument("options") is var optionsArg and (null or { Value.ConstantValue.HasValue: true }))
                {
                    return new StaticInvocationModel(
                        method,
                        location,
                        pattern,
                        optionsArg?.Value.ConstantValue.Value as long? ?? 0,
                        method.Name is "Replace" ? invocation.GetArgument("replacement")?.Value.ConstantValue.Value as string : null
                    );
                }
            }
            else
            {
                if (method.Name is "Replace"
                    && invocation.GetArgument("replacement")?.Value.ConstantValue is { HasValue: true, Value: string replacement })
                {
                    return new InstanceReplaceInvocationModel(
                        method,
                        location,
                        replacement
                    );
                }
            }
        }

        return null;
    }

    private class Generator
    {
        private static readonly SymbolDisplayFormat _parametersFormat
            = new(
                globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
                genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
                memberOptions: SymbolDisplayMemberOptions.IncludeParameters,
                parameterOptions: SymbolDisplayParameterOptions.IncludeName
                                  | SymbolDisplayParameterOptions.IncludeType
                                  | SymbolDisplayParameterOptions.IncludeModifiers,
                miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers
                                      | SymbolDisplayMiscellaneousOptions.UseSpecialTypes
                                      | SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier
            );

        private readonly CodeWriter _writer = new();
        private readonly LanguageVersion _languageVersion;

        private string TypeModifier => _languageVersion.SupportsFileModifier ? "file" : "internal";
        private string NullableSuffix => _languageVersion.SupportsNullableReferenceTypes ? "?" : string.Empty;

        public Generator(LanguageVersion languageVersion)
        {
            _languageVersion = languageVersion;
        }

        public string Generate(ImmutableArray<InvocationModel> invocations)
        {
            _writer.Clear();
            AppendHeader(_languageVersion);

            using (_writer.WriteBlock("namespace PCRE.Generated"))
            {
                _writer.AppendLine("[global::Microsoft.CodeAnalysis.Embedded]");

                using (_writer.WriteBlock($"{TypeModifier} static class PcreCallsInterceptor"))
                {
                    GenerateInterceptors(invocations);
                }
            }

            return _writer.ToString();
        }

        private void AppendHeader(LanguageVersion languageVersion)
        {
            _writer.AppendLine("// <auto-generated />")
                   .AppendLine();

            if (languageVersion.SupportsNullableReferenceTypes)
            {
                _writer.AppendLine("#nullable enable")
                       .AppendLine();
            }

            _writer.AppendLine(
                $$"""
                namespace System.Runtime.CompilerServices
                {
                    [global::Microsoft.CodeAnalysis.Embedded]
                    [global::System.Diagnostics.Conditional("PCRE_GENERATED")]
                    [global::System.AttributeUsage(global::System.AttributeTargets.Method, AllowMultiple = true)]
                    {{TypeModifier}} sealed class InterceptsLocationAttribute : global::System.Attribute
                    {
                        public InterceptsLocationAttribute(int version, string data)
                        { }
                    }
                }

                namespace Microsoft.CodeAnalysis
                {
                    internal sealed partial class EmbeddedAttribute : global::System.Attribute
                    { }
                }

                """
            );
        }

        private void AppendInterceptsLocationAttributeLine(InterceptableLocation location)
        {
            _writer.Append(location.GetInterceptsLocationAttributeSyntax())
#if DEBUG
                   .Append(" // ").Append(location.GetDisplayLocation())
#endif
                   .AppendLine();
        }

        private void GenerateInterceptors(ImmutableArray<InvocationModel> invocations)
        {
            var replacementPatterns = new ReplacementPatternSet();

            GenerateStaticCalls(
                replacementPatterns,
                invocations.OfType<StaticInvocationModel>()
            );

            GenerateInstanceReplaceCalls(
                replacementPatterns,
                invocations.OfType<InstanceReplaceInvocationModel>()
            );

            replacementPatterns.AppendFields(_writer, _languageVersion);
            replacementPatterns.AppendHelpers(_writer, _languageVersion);
        }

        private void GenerateStaticCalls(ReplacementPatternSet replacementPatterns, IEnumerable<StaticInvocationModel> invocations)
        {
            var regexCounter = 0;

            foreach (var regexGroup in invocations.GroupBy(i => (i.Pattern, i.Options)))
            {
                var (pattern, options) = regexGroup.Key;

                var assignment = _languageVersion.SupportsCoalescingAssignments
                    ? "??="
                    : $"!= null ? _regex{regexCounter} : _regex{regexCounter} =";

                _writer.AppendLine(
                    $"""
                    private static global::PCRE.PcreRegex{NullableSuffix} _regex{regexCounter};
                    private static global::PCRE.PcreRegex Regex{regexCounter} => _regex{regexCounter} {assignment} new global::PCRE.PcreRegex(
                        {SymbolDisplay.FormatLiteral(pattern, true)},
                        (global::PCRE.PcreOptions){SymbolDisplay.FormatPrimitive(options, false, true)}
                    );

                    """
                );

                var callCounter = 0;

                foreach (var methodGroup in regexGroup.GroupBy(i => i.Method, SymbolEqualityComparer.Default))
                {
                    var method = (IMethodSymbol)methodGroup.Key!;

                    foreach (var replacementPatternGroup in methodGroup.GroupBy(i => i.ReplacementPattern))
                    {
                        var replacementPattern = replacementPatternGroup.Key;

                        foreach (var interceptedMethod in replacementPatternGroup)
                            AppendInterceptsLocationAttributeLine(interceptedMethod.Location);

                        _writer.Append(
                            $"""
                            public static {method.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} Regex{regexCounter}_Call{callCounter}_{method.ToDisplayString(_parametersFormat)}
                                => Regex{regexCounter}.{method.Name}(
                            """
                        );

                        foreach (var param in method.Parameters)
                        {
                            if (param.Name is "pattern" or "options")
                                continue;

                            if (param.Name is "replacement" && method.Name is "Replace" && replacementPatterns.GetOrAdd(replacementPattern) is { } replacement)
                            {
                                _writer.Append($"replacementFunc: {replacement.GetReplacementFuncCall()}, ");
                                continue;
                            }

                            _writer.Append($"{param.Name}: {param.Name}, ");
                        }

                        _writer.TrimComma()
                               .AppendLine(");")
                               .AppendLine();

                        ++callCounter;
                    }
                }

                ++regexCounter;
            }
        }

        private void GenerateInstanceReplaceCalls(ReplacementPatternSet replacementPatterns, IEnumerable<InstanceReplaceInvocationModel> invocations)
        {
            foreach (var replacementPatternGroup in invocations.GroupBy(i => i.ReplacementPattern))
            {
                if (replacementPatterns.GetOrAdd(replacementPatternGroup.Key) is not { } replacement)
                    continue;

                var callCounter = 0;

                foreach (var methodGroup in replacementPatternGroup.GroupBy(i => i.Method, SymbolEqualityComparer.Default))
                {
                    var method = (IMethodSymbol)methodGroup.Key!;

                    var paramsSignature = method.ToDisplayString(_parametersFormat);

                    const string prefix = "Replace(";
                    if (!paramsSignature.StartsWith(prefix))
                        continue; // Shouldn't happen

                    paramsSignature = $"(this global::PCRE.PcreRegex regex, {paramsSignature.Substring(prefix.Length)}";

                    foreach (var interceptedMethod in methodGroup)
                        AppendInterceptsLocationAttributeLine(interceptedMethod.Location);

                    _writer.Append(
                        $"""
                        public static {method.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} Replace{replacement.PatternId}_Call{callCounter}{paramsSignature}
                            => regex.{method.Name}(
                        """
                    );

                    foreach (var param in method.Parameters)
                    {
                        if (param.Name is "replacement")
                        {
                            _writer.Append($"replacementFunc: {replacement.GetReplacementFuncCall()}, ");
                            continue;
                        }

                        _writer.Append($"{param.Name}: {param.Name}, ");
                    }

                    _writer.TrimComma()
                           .AppendLine(");")
                           .AppendLine();

                    ++callCounter;
                }
            }
        }
    }

    private abstract record InvocationModel(
        IMethodSymbol Method,
        InterceptableLocation Location
    );

    private sealed record StaticInvocationModel(
        IMethodSymbol Method,
        InterceptableLocation Location,
        string Pattern,
        long Options,
        string? ReplacementPattern
    ) : InvocationModel(Method, Location);

    private sealed record InstanceReplaceInvocationModel(
        IMethodSymbol Method,
        InterceptableLocation Location,
        string ReplacementPattern
    ) : InvocationModel(Method, Location);
}
